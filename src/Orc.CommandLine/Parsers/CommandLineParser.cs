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

    public class CommandLineParser : ICommandLineParser
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IOptionDefinitionService _optionDefinitionService;

        public CommandLineParser(IOptionDefinitionService optionDefinitionService)
        {
            Argument.IsNotNull(() => optionDefinitionService);

            _optionDefinitionService = optionDefinitionService;
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
                                var message = string.Format("Cannot parse '{0}' since there is no empty switch on the context", commandLineArgument);
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
                        var message = string.Format("Cannot parse '{0}' because it is not a switch and it is not the first argument", commandLineArgument);
                        Log.Warning(message);
                        validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateWarning(message));
                        continue;
                    }

                    var optionDefinition = (from x in optionDefinitions
                                            where x.IsSwitch(commandLineArgument)
                                            select x).FirstOrDefault();
                    if (optionDefinition == null)
                    {
                        var message = string.Format("Cannot parse '{0}' because the switch is not recognized", commandLineArgument);
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
                            var message = string.Format("Cannot parse '{0}' because the actual value is not found (command line is too short)", commandLineArgument);
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
                    validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateError("Cannot parse '{0}' because an exception occured: {1}", commandLineArgument, ex.Message));
                }
            }

            Log.Debug("Checking if all required options are specified");

            foreach (var optionDefinition in optionDefinitions)
            {
                if (optionDefinition.IsMandatory)
                {
                    if (!handledOptions.Contains(optionDefinition.ShortName))
                    {
                        var message = string.Format("Required option '{0}' is not specified", optionDefinition);
                        Log.Error(message);
                        validationContext.AddFieldValidationResult(FieldValidationResult.CreateError(optionDefinition.ShortName.ToString(), message));
                    }
                }
            }

            return validationContext;
        }

        private void UpdateContext(IContext targetContext, OptionDefinition optionDefinition, string value)
        {
            var propertyInfo = targetContext.GetType().GetPropertyEx(optionDefinition.PropertyNameOnContext);

            var finalValue = StringToObjectHelper.ToRightType(propertyInfo.PropertyType, value);

            propertyInfo.SetValue(targetContext, finalValue, null);
        }
    }
}