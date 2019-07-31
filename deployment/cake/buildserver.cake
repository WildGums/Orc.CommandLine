// Customize this file when using a different build server
#l "buildserver-continuaci.cake"

#addin "nuget:?package=MagicChunks&version=2.0.0.119"

using System.Runtime.InteropServices;

public interface IBuildServer
{
    void PinBuild(string comment);
    void SetVersion(string version);
    void SetVariable(string name, string value);
    Tuple<bool, string> GetVariable(string variableName, string defaultValue);
}

//-------------------------------------------------------------

public class BuildServerIntegration : IntegrationBase
{
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode)]
    static extern uint GetPrivateProfileString(
        string lpAppName, 
        string lpKeyName,
        string lpDefault, 
        StringBuilder lpReturnedString, 
        uint nSize,
        string lpFileName);

    private Dictionary<string, string> _buildServerVariableCache = null;
    private readonly List<IBuildServer> _buildServers = new List<IBuildServer>();

    public BuildServerIntegration(BuildContext buildContext)
        : base(buildContext)
    {
        _buildServers.Add(new ContinuaCIBuildServer(buildContext));
    }

    //-------------------------------------------------------------

    public void SetVersion(string version)
    {
        foreach (var buildServer in _buildServers)
        {
            buildServer.SetVersion(version);
        }
    }

    //-------------------------------------------------------------

    public void SetVariable(string variableName, string value)
    {
        foreach (var buildServer in _buildServers)
        {
            buildServer.SetVariable(variableName, value);
        }
    }

    //-------------------------------------------------------------

    public bool GetVariableAsBool(string variableName, bool defaultValue, bool showValue = false)
    {
        var value = defaultValue;

        if (bool.TryParse(GetVariable(variableName, "unknown", showValue: showValue), out var retrievedValue))
        {
            value = retrievedValue;
        }

        return value;
    }

    //-------------------------------------------------------------

    public string GetVariable(string variableName, string defaultValue = null, bool showValue = false)
    {
        if (_buildServerVariableCache is null)
        {
            _buildServerVariableCache = new Dictionary<string, string>();
        }

        var cacheKey = string.Format("{0}__{1}", variableName ?? string.Empty, defaultValue ?? string.Empty);

        if (!_buildServerVariableCache.TryGetValue(cacheKey, out string value))
        {
            value = GetVariableForCache(buildContext, variableName, defaultValue, showValue);
            //if (value != defaultValue &&
            //    !string.IsNullOrEmpty(value) && 
            //    !string.IsNullOrEmpty(defaultValue))
            //{
                var valueForLog = showValue ? value : "********";
                buildContext.CakeContext.Information("{0}: '{1}'", variableName, valueForLog);
            //}
            
            _buildServerVariableCache[cacheKey] = value;
        }
        //else
        //{
        //    Information("Retrieved value for '{0}' from cache", variableName);
        //}
        
        return value;
    }

    //-------------------------------------------------------------

    private string GetVariableForCache(string variableName, string defaultValue = null, bool showValue = false)
    {
        var argumentValue = BuildContext.CakeContext.Argument(variableName, "non-existing");
        if (argumentValue != "non-existing")
        {
            BuildContext.CakeContext.Information("Variable '{0}' is specified via an argument", variableName);

            return argumentValue;
        }

        // Check each build server
        foreach (var buildServer in _buildServers)
        {
            var buildServerVariable = buildServer.GetVariable(variableName, defaultValue);
            if (buildServerVariable.Item1)
            {
                return buildServerVariable.Item2;
            }
        }

        var overrideFile = "./build.cakeoverrides";
        if (System.IO.File.Exists(overrideFile))
        {
            var sb = new StringBuilder(string.Empty, 256);
            var lengthRead = GetPrivateProfileString("General", variableName, null, sb, (uint)sb.Capacity, overrideFile);
            if (lengthRead > 0)
            {
                BuildContext.CakeContext.Information("Variable '{0}' is specified via build.cakeoverrides", variableName);
            
                return sb.ToString();
            }
        }
        
        if (BuildContext.CakeContext.HasEnvironmentVariable(variableName))
        {
            BuildContext.CakeContext.Information("Variable '{0}' is specified via an environment variable", variableName);
        
            return BuildContext.CakeContext.EnvironmentVariable(variableName);
        }
        
        var parameters = BuildContext.Parameters;
        if (parameters.TryGetValue(variableName, out var parameter))
        {
            BuildContext.CakeContext.Information("Variable '{0}' is specified via the Parameters dictionary", variableName);
        
            if (parameter is null)
            {
                return null;
            }
        
            if (parameter is string)
            {
                return (string)parameter;
            }
            
            if (parameter is Func<string>)
            {
                return ((Func<string>)parameter).Invoke();
            }
            
            throw new Exception(string.Format("Parameter is defined as '{0}', but that type is not supported yet...", parameter.GetType().Name));
        }
        
        BuildContext.CakeContext.Information("Variable '{0}' is not specified, returning default value", variableName);
        
        return defaultValue ?? string.Empty;
    }
}