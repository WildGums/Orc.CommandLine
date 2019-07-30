// Customize this file when using a different build server
#l "buildserver-continuaci.cake"

#addin "nuget:?package=MagicChunks&version=2.0.0.119"

using System.Runtime.InteropServices;

[DllImport("kernel32.dll", CharSet=CharSet.Unicode)]
static extern uint GetPrivateProfileString(
   string lpAppName, 
   string lpKeyName,
   string lpDefault, 
   StringBuilder lpReturnedString, 
   uint nSize,
   string lpFileName);

private static Dictionary<string, string> _buildServerVariableCache = null;

//-------------------------------------------------------------

public static void SetBuildServerVersion(IBuildContext buildContext, string version)
{
    SetContinuaCIVersion(buildContext, version);
}

//-------------------------------------------------------------

public static void SetBuildServerVariable(IBuildContext buildContext, string variableName, string value)
{
    SetContinuaCIVariable(buildContext, variableName, value);
}

//-------------------------------------------------------------

public static bool GetBuildServerVariableAsBool(IBuildContext buildContext, string variableName, bool defaultValue, bool showValue = false)
{
    var value = defaultValue;

    if (bool.TryParse(GetBuildServerVariable(buildContext, variableName, "unknown", showValue: showValue), out var retrievedValue))
    {
        value = retrievedValue;
    }

    return value;
}

//-------------------------------------------------------------

public static string GetBuildServerVariable(IBuildContext buildContext, string variableName, string defaultValue = null, bool showValue = false)
{
    if (_buildServerVariableCache == null)
    {
        _buildServerVariableCache = new Dictionary<string, string>();
    }

    var cacheKey = string.Format("{0}__{1}", variableName ?? string.Empty, defaultValue ?? string.Empty);

    if (!_buildServerVariableCache.TryGetValue(cacheKey, out string value))
    {
        value = GetBuildServerVariableForCache(buildContext, variableName, defaultValue, showValue);
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

private static string GetBuildServerVariableForCache(IBuildContext buildContext, string variableName, string defaultValue = null, bool showValue = false)
{
    var argumentValue = buildContext.CakeContext.Argument(variableName, "non-existing");
    if (argumentValue != "non-existing")
    {
        buildContext.CakeContext.Information("Variable '{0}' is specified via an argument", variableName);

        return argumentValue;
    }

    // Just a forwarder, change this line to use a different build server
    var buildServerVariable = GetContinuaCIVariable(buildContext, variableName, defaultValue);
    if (buildServerVariable.Item1)
    {
        return buildServerVariable.Item2;
    }

    var overrideFile = "./build.cakeoverrides";
    if (System.IO.File.Exists(overrideFile))
    {
        var sb = new StringBuilder(string.Empty, 256);
        var lengthRead = GetPrivateProfileString("General", variableName, null, sb, (uint)sb.Capacity, overrideFile);
        if (lengthRead > 0)
        {
            buildContext.CakeContext.Information("Variable '{0}' is specified via build.cakeoverrides", variableName);
        
            return sb.ToString();
        }
    }
    
    if (buildContext.CakeContext.HasEnvironmentVariable(variableName))
    {
        buildContext.CakeContext.Information("Variable '{0}' is specified via an environment variable", variableName);
    
        return buildContext.CakeContext.EnvironmentVariable(variableName);
    }
    
    var parameters = Parameters;
    if (parameters.TryGetValue(variableName, out var parameter))
    {
        buildContext.CakeContext.Information("Variable '{0}' is specified via the Parameters dictionary", variableName);
    
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
    
    buildContext.CakeContext.Information("Variable '{0}' is not specified, returning default value", variableName);
    
    return defaultValue ?? string.Empty;
}