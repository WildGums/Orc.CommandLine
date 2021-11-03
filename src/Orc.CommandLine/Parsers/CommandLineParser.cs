﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="commandLineParser.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Catel;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Services;

    public class CommandLineParser : ICommandLineParser
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IOptionDefinitionService _optionDefinitionService;
        private readonly ILanguageService _languageService;
        private readonly ICommandLineService _commandLineService;

        public CommandLineParser(IOptionDefinitionService optionDefinitionService, ILanguageService languageService,
            ICommandLineService commandLineService)
        {
            Argument.IsNotNull(() => optionDefinitionService);
            Argument.IsNotNull(() => languageService);
            Argument.IsNotNull(() => commandLineService);

            _optionDefinitionService = optionDefinitionService;
            _languageService = languageService;
            _commandLineService = commandLineService;
        }

        public IValidationContext Parse(IContext targetContext)
        {
            return Parse(_commandLineService.GetCommandLine(), targetContext);
        }

        public IValidationContext Parse(string commandLine, IContext targetContext)
        {
            targetContext.CommandLine = commandLine;
            var result = Parse(targetContext.GetType(), ExtractCommandLineArguments(targetContext), targetContext);

            TransferValues(result, targetContext);

            return result.ValidationContext;
        }

        public TResult Parse<TResult>(ICommandLineParsingContext commandLineParsingContext)
            where TResult : IResult
        {
            var splitted = ExtractCommandLineArguments(commandLineParsingContext);

            var ctor = typeof(TResult).GetConstructor(Array.Empty<Type>());
            var result = (TResult)ctor.Invoke(new object[] { });

            return (TResult)Parse(typeof(TResult), splitted, result);
        }

        public IValidationContext Parse(IEnumerable<string> commandLineArguments, IContext targetContext)
        {
            var result = Parse(targetContext.GetType(), commandLineArguments.ToList(), targetContext);

            TransferValues(result, targetContext);

            return result.ValidationContext;
        }

        public IValidationContext Parse(List<string> commandLineArguments, IContext targetContext)
        {
            var result = Parse(targetContext.GetType(), commandLineArguments, targetContext);

            TransferValues(result, targetContext);

            return result.ValidationContext;
        }

        private IResult Parse(Type resultType, List<string> commandLineArguments, ICommandLineParsingContext targetContext)
        {
            var ctor = resultType.GetConstructor(Array.Empty<Type>());
            var result = (IResult)ctor.Invoke(new object[] { });

            var validationContext = new ValidationContext();
            result.ValidationContext = validationContext;

            var quoteSplitCharacters = targetContext.QuoteSplitCharacters.ToArray();
            result.OriginalCommandLine = string.Join(" ", commandLineArguments);

            var isHelp = commandLineArguments.Any(commandLineArgument => commandLineArgument.IsHelp(quoteSplitCharacters));
            if (isHelp)
            {
                result.IsHelp = true;

                return result;
            }

            var optionDefinitions = _optionDefinitionService.GetOptionDefinitions(result);

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

                        if (emptyOptionDefinition is null)
                        {
                            var message = string.Format(_languageService.GetString("CommandLine_CannotParseNoEmptySwitch"), commandLineArgument);
                            Log.Debug(message);
                            validationContext.Add(BusinessRuleValidationResult.CreateError(message));
                            continue;
                        }

                        UpdateContext(result, emptyOptionDefinition, commandLineArgument);
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
                    var isKnownDefinition = (optionDefinition is not null);
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

                    result.RawValues[commandLineArgument.TrimSwitchPrefix()] = value;

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

                    UpdateContext(result, optionDefinition, value);
                    handledOptions.Add(optionDefinition.ShortName);
                }
                catch (Exception ex)
                {
                    validationContext.Add(BusinessRuleValidationResult.CreateError(_languageService.GetString("CommandLine_CannotParseExceptionOccurred"), commandLineArgument, ex.Message));
                }
            }

            ValidateMandatorySwitches(validationContext, optionDefinitions, handledOptions);

            Log.Debug("Finishing the context");

            result.Finish();

            return result;
        }

        private void TransferValues(IResult result, IContext targetContext)
        {
            var resultProperties = result.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            var targetProperties = targetContext.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            foreach (var property in resultProperties)
            {
                var target = targetProperties.FirstOrDefault(x => x.Name == property.Name);
                if (target is not null && target.CanWrite)
                {
                    target.SetValue(targetContext, property.GetValue(result, null), null);
                }
                if (target is not null && !target.CanWrite)
                {
                    targetContext.SetPrivatePropertyValue(property.Name, property.GetValue(result));
                }
            }
        }

        private List<string> ExtractCommandLineArguments(ICommandLineParsingContext commandLineParsingContext)
        {
            var splitted = new List<string>();
            var regex = CreateRegex(commandLineParsingContext);
            var matches = regex.Matches(commandLineParsingContext.CommandLine).Cast<Match>();
            foreach (var match in matches)
            {
                var matchValue = match.Value;

                if (string.IsNullOrWhiteSpace(matchValue))
                {
                    continue;
                }

                splitted.Add(matchValue);
            }

            return splitted;
        }

        protected virtual void ValidateMandatorySwitches(IValidationContext validationContext, IEnumerable<OptionDefinition> optionDefinitions, HashSet<string> handledOptions)
        {
            Log.Debug("Checking if all required options are specified");

            foreach (var optionDefinition in optionDefinitions)
            {
                if (optionDefinition.IsMandatory && !handledOptions.Contains(optionDefinition.ShortName))
                {
                    var message = string.Format(_languageService.GetString("CommandLine_RequiredSwitchNotSpecified"), optionDefinition);
                    Log.Error(message);
                    validationContext.Add(FieldValidationResult.CreateError(optionDefinition.GetSwitchDisplay(), message));
                }
            }
        }

        protected virtual Regex CreateRegex(ICommandLineParsingContext targetContext)
        {
            // Working
            // "(?<match>[#\s\d\w\:/\\.\-\?]*)"|'(?<match>[#\s\d\w\:/\\.\-\?]*)'|(?<match>[\d\w\:/\\.\-\?]*)
            const string MatchingCharactersRegexPart = @"[#\s\d\w\:/\\.\-\?\!\@\#\$\%\^\&\*]*";

            var blocks = new List<string>();

            foreach (var quoteSplitCharacter in targetContext.QuoteSplitCharacters)
            {
                blocks.Add(string.Format(@"{0}(?<match>{1}){0}", quoteSplitCharacter, MatchingCharactersRegexPart));
            }

            // Add support for items without quotes, allow everything except whitespace (\s)
            blocks.Add(string.Format(@"(?<match>{0})", MatchingCharactersRegexPart.Replace(@"\s", string.Empty)));

            var regexString = string.Join("|", blocks);
            var regex = new Regex(regexString);
            return regex;
        }

        private void UpdateContext(IContext targetContext, OptionDefinition optionDefinition, string value)
        {
            var propertyInfo = targetContext.GetType().GetPropertyEx(optionDefinition.PropertyNameOnContext);

            if (optionDefinition.TrimQuotes && !string.IsNullOrWhiteSpace(value))
            {
                value = value.Trim(targetContext.QuoteSplitCharacters.ToArray());
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
