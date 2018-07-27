namespace Orc.CommandLine
{
    public class CommandLineService : ICommandLineService
    {
        public virtual string GetCommandLine()
        {
            var commandLine = System.Environment.CommandLine;
            
            commandLine = commandLine.GetCommandLine(true);

            return commandLine;
        }
    }
}
