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

        public CommandLineParser(IOptionDefinitionService optionDefinitionService, ILanguageService languageService)
        {
            Argument.IsNotNull(() => optionDefinitionService);
            Argument.IsNotNull(() => languageService);

            _optionDefinitionService = optionDefinitionService;
            _languageService = languageService;
        }

        public IValidationContext Parse(IEnumerable<string> commandLineArguments, IContext targetContext)
        {
            return Parse(commandLineArguments.ToList(), targetContext);
        }

        public IValidationContext Parse(List<string> commandLineArguments, IContext targetContext)
        {
            var validationContext = new ValidationContext();

            targetContext.OriginalCommandLine = string.Join(" ", commandLineArguments);

            var isHelp = commandLineArguments.Any(commandLineArgument => commandLineArgument.IsHelp());
            if (isHelp)
            {
                targetContext.IsHelp = true;
                return validationContext;
            }

            var optionDefinitions = _optionDefinitionService.GetOptionDefinitions(targetContext);

            var handledOptions = new HashSet<char>();

            Log.Debug("Parsing command line");

            for (var i = 0; i < commandLineArguments.Count; i++)
            {
                var commandLineArgument = commandLineArguments[i];

                try
                {
                    // Allow the first one to be a non-switch
                    if (i == 0)
                    {
                        if (!commandLineArguments[i].IsSwitch())
                        {
                            var emptyOptionDefinition = (from x in optionDefinitions
                                                         where !x.HasSwitch()
                                                         select x).FirstOrDefault();

                            if (emptyOptionDefinition == null)
                            {
                                var message = string.Format(_languageService.GetString("CommandLine_CannotParseNoEmptySwitch"), commandLineArgument);
                                Log.Error(message);
                                validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateError(message));
                                continue;
                            }

                            UpdateContext(targetContext, emptyOptionDefinition, commandLineArgument);
                            handledOptions.Add(emptyOptionDefinition.ShortName);
                            continue;
                        }
                    }

                    if (!commandLineArgument.IsSwitch())
                    {
                        var message = string.Format(_languageService.GetString("CommandLine_CannotParseNoSwitch"), commandLineArgument);
                        Log.Warning(message);
                        validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateWarning(message));
                        continue;
                    }

                    var optionDefinition = (from x in optionDefinitions
                                            where x.IsSwitch(commandLineArgument)
                                            select x).FirstOrDefault();
                    if (optionDefinition == null)
                    {
                        var message = string.Format(_languageService.GetString("CommandLine_CannotParseSwitchNotRecognized"), commandLineArgument);
                        Log.Warning(message);
                        validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateWarning(message));
                        continue;
                    }

                    var value = string.Empty;
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
                            Log.Warning(message);
                            validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateWarning(message));
                            continue;
                        }

                        value = commandLineArguments[++i];
                    }

                    UpdateContext(targetContext, optionDefinition, value);
                    handledOptions.Add(optionDefinition.ShortName);
                }
                catch (Exception ex)
                {
                    validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateError(_languageService.GetString("CommandLine_CannotParseExceptionOccurred"), commandLineArgument, ex.Message));
                }
            }

            Log.Debug("Checking if all required options are specified");

            foreach (var optionDefinition in optionDefinitions)
            {
                if (optionDefinition.IsMandatory)
                {
                    if (!handledOptions.Contains(optionDefinition.ShortName))
                    {
                        var message = string.Format(_languageService.GetString("CommandLine_RequiredSwitchNotSpecified"), optionDefinition);
                        Log.Error(message);
                        validationContext.AddFieldValidationResult(FieldValidationResult.CreateError(optionDefinition.GetSwitchDisplay(), message));
                    }
                }
            }

            Log.Debug("Finishing the context");

            targetContext.Finish();

            return validationContext;
        }

        private void UpdateContext(IContext targetContext, OptionDefinition optionDefinition, string value)
        {
            var propertyInfo = targetContext.GetType().GetPropertyEx(optionDefinition.PropertyNameOnContext);

            if (optionDefinition.TrimQuotes)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    value = value.Trim('\"');
                }
            }

            var finalValue = StringToObjectHelper.ToRightType(propertyInfo.PropertyType, value);

            propertyInfo.SetValue(targetContext, finalValue, null);
        }
    }
}