// --------------------------------------------------------------------------------------------------------------------
// <copyright file="commandLineParser.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Catel;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Services;

    public class CommandLineParser : ICommandLineParser
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IOptionDefinitionService _optionDefinitionService;
        private readonly ILanguageService _languageService;
        private readonly ICommandLineService _commandLineService;
        private readonly ITypeFactory _typeFactory;

        public CommandLineParser(IOptionDefinitionService optionDefinitionService, ILanguageService languageService,
            ICommandLineService commandLineService, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => optionDefinitionService);
            Argument.IsNotNull(() => languageService);
            Argument.IsNotNull(() => commandLineService);
            Argument.IsNotNull(() => typeFactory);

            _optionDefinitionService = optionDefinitionService;
            _languageService = languageService;
            _commandLineService = commandLineService;
            _typeFactory = typeFactory;

            DefaultOptions = new CommandLineParseOptions();
        }

        public CommandLineParseOptions DefaultOptions { get; }

        public ICommandLineContext Parse(Type contextType, CommandLineParseOptions options = null)
        {
            return Parse(contextType, _commandLineService.GetCommandLine(), options);
        }

        public ICommandLineContext Parse(Type contextType, string commandLine, CommandLineParseOptions options = null)
        {
            var splitted = new List<string>();

            var regex = CreateRegex(options);
            var matches = regex.Matches(commandLine).Cast<Match>();

            foreach (var match in matches)
            {
                var matchValue = match.Value;

                if (string.IsNullOrWhiteSpace(matchValue))
                {
                    continue;
                }

                splitted.Add(matchValue);
            }

            return Parse(contextType, splitted, options);
        }

        public ICommandLineContext Parse(Type contextType, IEnumerable<string> commandLineArguments, CommandLineParseOptions options = null)
        {
            return Parse(contextType, commandLineArguments.ToList(), options);
        }

        public ICommandLineContext Parse(Type contextType, List<string> commandLineArguments, CommandLineParseOptions options = null)
        {
            options = options ?? DefaultOptions;

            var targetContext = (ICommandLineContext)_typeFactory.CreateInstance(contextType);
            var validationContext = targetContext.ValidationContext;

            var quoteSplitCharacters = options.QuoteSplitCharacters.ToArray();
            targetContext.OriginalCommandLine = string.Join(" ", commandLineArguments);

            var isHelp = commandLineArguments.Any(commandLineArgument => commandLineArgument.IsHelp(quoteSplitCharacters));
            if (isHelp)
            {
                targetContext.IsHelp = true;
                return targetContext;
            }

            var optionDefinitions = _optionDefinitionService.GetOptionDefinitions(targetContext);

            var handledOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            Log.Debug("Parsing command line");

            for (var i = 0; i < commandLineArguments.Count; i++)
            {
                var commandLineArgument = commandLineArguments[i];

                try
                {
                    // Allow the first one to be a non-switch
                    if (i == 0 && !commandLineArguments[i].IsSwitch(quoteSplitCharacters))
                    {
                        var emptyOptionDefinition = (from x in optionDefinitions
                                                     where !x.HasSwitch()
                                                     select x).FirstOrDefault();

                        if (emptyOptionDefinition == null)
                        {
                            var message = string.Format(_languageService.GetString("CommandLine_CannotParseNoEmptySwitch"), commandLineArgument);
                            Log.Debug(message);
                            validationContext.Add(BusinessRuleValidationResult.CreateError(message));
                            continue;
                        }

                        UpdateContext(targetContext, emptyOptionDefinition, commandLineArgument, options);
                        handledOptions.Add(emptyOptionDefinition.ShortName);
                        continue;
                    }

                    if (!commandLineArgument.IsSwitch(quoteSplitCharacters))
                    {
                        var message = string.Format(_languageService.GetString("CommandLine_CannotParseNoSwitch"), commandLineArgument);
                        Log.Debug(message);
                        validationContext.Add(BusinessRuleValidationResult.CreateWarning(message));
                        continue;
                    }

                    var value = string.Empty;

                    var optionDefinition = (from x in optionDefinitions
                                            where x.IsSwitch(commandLineArgument, quoteSplitCharacters)
                                            select x).FirstOrDefault();
                    var isKnownDefinition = (optionDefinition != null);
                    if (!isKnownDefinition)
                    {
                        var message = string.Format(_languageService.GetString("CommandLine_CannotParseSwitchNotRecognized"), commandLineArgument);
                        Log.Debug(message);
                        validationContext.Add(BusinessRuleValidationResult.CreateWarning(message));

                        // Try to read the next value, but keep in mind that some options might 
                        // not have a value passed into it
                        var potentialValue = (i < commandLineArguments.Count - 1) ? commandLineArguments[i + 1] : string.Empty;
                        if (!string.IsNullOrWhiteSpace(potentialValue) && potentialValue.IsSwitch(quoteSplitCharacters))
                        {
                            potentialValue = string.Empty;
                        }

                        value = potentialValue;
                    }

                    targetContext.RawValues[commandLineArgument.TrimSwitchPrefix()] = value;

                    if (!isKnownDefinition)
                    {
                        continue;
                    }

                    if (!optionDefinition.AcceptsValue)
                    {
                        // Assume boolean switch
                        value = "true";
                    }
                    else
                    {
                        if (commandLineArguments.Count <= i + 1)
                        {
                            var message = string.Format(_languageService.GetString("CommandLine_CannotParseValueMissing"), commandLineArgument);
                            Log.Info(message);
                            validationContext.Add(BusinessRuleValidationResult.CreateWarning(message));
                            continue;
                        }

                        value = commandLineArguments[++i];
                    }

                    UpdateContext(targetContext, optionDefinition, value, options);
                    handledOptions.Add(optionDefinition.ShortName);
                }
                catch (Exception ex)
                {
                    validationContext.Add(BusinessRuleValidationResult.CreateError(_languageService.GetString("CommandLine_CannotParseExceptionOccurred"), commandLineArgument, ex.Message));
                }
            }

            ValidateMandatorySwitches(validationContext, optionDefinitions, handledOptions, options);

            Log.Debug("Finishing the context");

            targetContext.Finish();

            return targetContext;
        }

        protected virtual void ValidateMandatorySwitches(IValidationContext validationContext, IEnumerable<OptionDefinition> optionDefinitions, 
            HashSet<string> handledOptions, CommandLineParseOptions options)
        {
            options = options ?? DefaultOptions;

            Log.Debug("Checking if all required options are specified");

            foreach (var optionDefinition in optionDefinitions)
            {
                if (optionDefinition.IsMandatory && !handledOptions.Contains(optionDefinition.ShortName))
                {
                    var message = string.Format(_languageService.GetString("CommandLine_RequiredSwitchNotSpecified"), optionDefinition);

                    if (options.LogMissingMandatoryOptionsAsErrors)
                    {
                        Log.Error(message);
                    }

                    validationContext.Add(FieldValidationResult.CreateError(optionDefinition.GetSwitchDisplay(), message));
                }
            }
        }

        protected virtual Regex CreateRegex(CommandLineParseOptions options = null)
        {
            options = options ?? DefaultOptions;

            // Working
            // "(?<match>[#\s\d\w\:/\\.\-\?]*)"|'(?<match>[#\s\d\w\:/\\.\-\?]*)'|(?<match>[\d\w\:/\\.\-\?]*)
            const string MatchingCharactersRegexPart = @"[#\s\d\w\:/\\.\-\?]*";

            var blocks = new List<string>();

            foreach (var quoteSplitCharacter in options.QuoteSplitCharacters)
            {
                blocks.Add(string.Format(@"{0}(?<match>{1}){0}", quoteSplitCharacter, MatchingCharactersRegexPart));
            }

            // Add support for items without quotes, allow everything except whitespace (\s)
            blocks.Add(string.Format(@"(?<match>{0})", MatchingCharactersRegexPart.Replace(@"\s", string.Empty)));

            var regexString = string.Join("|", blocks);
            var regex = new Regex(regexString);
            return regex;
        }

        private void UpdateContext(ICommandLineContext targetContext, OptionDefinition optionDefinition, string value, CommandLineParseOptions options)
        {
            options = options ?? DefaultOptions;

            var propertyInfo = targetContext.GetType().GetPropertyEx(optionDefinition.PropertyNameOnContext);

            if (optionDefinition.TrimQuotes && !string.IsNullOrWhiteSpace(value))
            {
                value = value.Trim(options.QuoteSplitCharacters.ToArray());
            }

            if (optionDefinition.TrimWhiteSpace && !string.IsNullOrWhiteSpace(value))
            {
                value = value.Trim();
            }

            var finalValue = StringToObjectHelper.ToRightType(propertyInfo.PropertyType, value);

            propertyInfo.SetValue(targetContext, finalValue, null);
        }
    }
}
