public static void SetContinuaCIVersion(IBuildContext buildContext, string version)
{
    if (ContinuaCI.IsRunningOnContinuaCI)
    {
        buildContext.CakeContext.Information("Setting version '{0}' in Continua CI", version);

        var message = string.Format("@@continua[setBuildVersion value='{0}']", version);
        WriteContinuaCiIntegration(buildContext, message);
    }
}

//-------------------------------------------------------------

public static void SetContinuaCIVariable(IBuildContext buildContext, string variableName, string value)
{
    if (ContinuaCI.IsRunningOnContinuaCI)
    {
        buildContext.CakeContext.Information("Setting variable '{0}' to '{1}' in Continua CI", variableName, value);
    
        var message = string.Format("@@continua[setVariable name='{0}' value='{1}' skipIfNotDefined='true']", variableName, value);
        WriteContinuaCiIntegration(buildContext, message);
    }
}

//-------------------------------------------------------------

public static Tuple<bool, string> GetContinuaCIVariable(IBuildContext buildContext, string variableName, string defaultValue)
{
    var exists = false;
    var value = string.Empty;

    if (ContinuaCI.IsRunningOnContinuaCI)
    {
        var buildServerVariables = ContinuaCI.Environment.Variable;
        if (buildServerVariables.ContainsKey(variableName))
        {
            buildContext.CakeContext.Information("Variable '{0}' is specified via Continua CI", variableName);
        
            exists = true;
            value = buildServerVariables[variableName];
        }
    }

    return new Tuple<bool, string>(exists, value);
}

//-------------------------------------------------------------

private static void WriteContinuaCiIntegration(IBuildContext buildContext, string message)
{
    // Must be Console.WriteLine
    buildContext.CakeContext.Information(message);
}