public class ContinuaCIBuildServer : IBuildServer
{
    public ContinuaCIBuildServer(BuildContext buildContext)
    {
        BuildContext = buildContext;
    }

    public BuildContext BuildContext { get; private set; }

    public void PinBuild(string comment)
    {
        if (!ContinuaCI.IsRunningOnContinuaCI)
        {
            return;
        }

        BuildContext.CakeContext.Information("Pinning build in Continua CI");

        var message = string.Format("@@continua[pinBuild comment='{0}' appendComment='{1}']", 
            comment, !string.IsNullOrWhiteSpace(comment));
        WriteIntegration(message);
    }

    public void SetVersion(string version)
    {
        if (!ContinuaCI.IsRunningOnContinuaCI)
        {
            return;
        }

        BuildContext.CakeContext.Information("Setting version '{0}' in Continua CI", version);

        var message = string.Format("@@continua[setBuildVersion value='{0}']", version);
        WriteIntegration(message);
    }

    public void SetVariable(string variableName, string value)
    {
        if (!ContinuaCI.IsRunningOnContinuaCI)
        {
            return;
        }

        BuildContext.CakeContext.Information("Setting variable '{0}' to '{1}' in Continua CI", variableName, value);
    
        var message = string.Format("@@continua[setVariable name='{0}' value='{1}' skipIfNotDefined='true']", variableName, value);
        WriteContinuaCiIntegration(message);
    }

    public Tuple<bool, string> GetVariable(string variableName, string defaultValue)
    {
        if (!ContinuaCI.IsRunningOnContinuaCI)
        {
            return new Tuple<bool, string>(false, string.Empty);
        }

        var exists = false;
        var value = string.Empty;

        var buildServerVariables = ContinuaCI.Environment.Variable;
        if (buildServerVariables.ContainsKey(variableName))
        {
            BuildContext.CakeContext.Information("Variable '{0}' is specified via Continua CI", variableName);
        
            exists = true;
            value = buildServerVariables[variableName];
        }
        
        return new Tuple<bool, string>(exists, value);
    }

    private void WriteIntegration(string message)
    {
        // Must be Console.WriteLine
        BuildContext.CakeContext.Information(message);
    }
}