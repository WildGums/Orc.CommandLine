namespace Orc.CommandLine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Catel;
using Catel.Data;
using Catel.Logging;
using Catel.Reflection;
using Catel.Services;
using Properties;

public class CommandLineParser : ICommandLineParser
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    private readonly IOptionDefinitionService _optionDefinitionService;
    private readonly ILanguageService _languageService;
    private readonly ICommandLineService _commandLineService;

    public CommandLineParser(IOptionDefinitionService optionDefinitionService, ILanguageService languageService,
        ICommandLineService commandLineService)
    {
        ArgumentNullException.ThrowIfNull(optionDefinitionService);
        ArgumentNullException.ThrowIfNull(languageService);
        ArgumentNullException.ThrowIfNull(commandLineService);

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
        var splitted = new List<string>();

        var regex = CreateRegex(targetContext);
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

        return Parse(splitted, targetContext);
    }

    public IValidationContext Parse(IEnumerable<string> commandLineArguments, IContext targetContext)
    {
        return Parse(commandLineArguments.ToList(), targetContext);
    }

    public IValidationContext Parse(List<string> commandLineArguments, IContext targetContext)
    {
        var validationContext = new ValidationContext();

        var quoteSplitCharacters = targetContext.QuoteSplitCharacters.ToArray();
        targetContext.OriginalCommandLine = string.Join(" ", commandLineArguments);

        var isHelp = commandLineArguments.Any(commandLineArgument => commandLineArgument.IsHelp(quoteSplitCharacters));
        if (isHelp)
        {
            targetContext.IsHelp = true;
            return validationContext;
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
                    var emptyOptionDefinition = optionDefinitions.FirstOrDefault(x => !x.HasSwitch());

                    if (emptyOptionDefinition is null)
                    {
                        var message = string.Format(_languageService.GetRequiredString(nameof(Resources.CommandLine_CannotParseNoEmptySwitch)), commandLineArgument);
                        Log.Debug(message);
                        validationContext.Add(BusinessRuleValidationResult.CreateError(message));
                        continue;
                    }

                    UpdateContext(targetContext, emptyOptionDefinition, commandLineArgument);
                    handledOptions.Add(emptyOptionDefinition.ShortName);
                    continue;
                }

                if (!commandLineArgument.IsSwitch(quoteSplitCharacters))
                {
                    var message = string.Format(_languageService.GetRequiredString(nameof(Resources.CommandLine_CannotParseNoSwitch)), commandLineArgument);
                    Log.Debug(message);
                    validationContext.Add(BusinessRuleValidationResult.CreateWarning(message));
                    continue;
                }

                var value = string.Empty;

                var optionDefinition = optionDefinitions.FirstOrDefault(x => x.IsSwitch(commandLineArgument, quoteSplitCharacters));
                if (optionDefinition is null)
                {
                    var message = string.Format(_languageService.GetRequiredString(nameof(Resources.CommandLine_CannotParseSwitchNotRecognized)), commandLineArgument);
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

                if (optionDefinition is null)
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
                        var message = string.Format(_languageService.GetRequiredString(nameof(Resources.CommandLine_CannotParseValueMissing)), commandLineArgument);
                        Log.Info(message);
                        validationContext.Add(BusinessRuleValidationResult.CreateWarning(message));
                        continue;
                    }

                    value = commandLineArguments[++i];
                }

                UpdateContext(targetContext, optionDefinition, value);
                handledOptions.Add(optionDefinition.ShortName);
            }
            catch (Exception ex)
            {
                validationContext.Add(BusinessRuleValidationResult.CreateError(_languageService.GetRequiredString(nameof(Resources.CommandLine_CannotParseExceptionOccurred)), commandLineArgument, ex.Message));
            }
        }

        ValidateMandatorySwitches(validationContext, optionDefinitions, handledOptions);

        Log.Debug("Finishing the context");

        targetContext.Finish();

        return validationContext;
    }

    protected virtual void ValidateMandatorySwitches(IValidationContext validationContext, IEnumerable<OptionDefinition> optionDefinitions, HashSet<string> handledOptions)
    {
        Log.Debug("Checking if all required options are specified");

        foreach (var optionDefinition in optionDefinitions)
        {
            if (optionDefinition.IsMandatory && !handledOptions.Contains(optionDefinition.ShortName))
            {
                var message = string.Format(_languageService.GetRequiredString(nameof(Resources.CommandLine_RequiredSwitchNotSpecified)), optionDefinition);
                Log.Error(message);
                validationContext.Add(FieldValidationResult.CreateError(optionDefinition.GetSwitchDisplay(), message));
            }
        }
    }

    protected virtual Regex CreateRegex(IContext targetContext)
    {
        // Working
        // "(?<match>[#\s\d\w\:/\\.\-\?]*)"|'(?<match>[#\s\d\w\:/\\.\-\?]*)'|(?<match>[\d\w\:/\\.\-\?]*)
        const string matchingCharactersRegexPart = @"[#\s\d\w\:/\\.\-\?\!\@\#\$\%\^\&\*\(\)]*";

        var blocks = targetContext.QuoteSplitCharacters
            .Select(quoteSplitCharacter => string.Format(@"{0}(?<match>{1}){0}", quoteSplitCharacter, matchingCharactersRegexPart))
            .ToList();

        // Add support for items without quotes, allow everything except whitespace (\s)
        blocks.Add($@"(?<match>{matchingCharactersRegexPart.Replace(@"\s", string.Empty)})");

        var regexString = string.Join("|", blocks);
        var regex = new Regex(regexString);
        return regex;
    }

    private static void UpdateContext(IContext targetContext, OptionDefinition optionDefinition, string value)
    {
        var propertyInfo = targetContext.GetType().GetPropertyEx(optionDefinition.PropertyNameOnContext);
        if (propertyInfo is null)
        {
            throw Log.ErrorAndCreateException<CommandLineException>($"Cannot find property name '{optionDefinition.PropertyNameOnContext}' on the context");
        }

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
