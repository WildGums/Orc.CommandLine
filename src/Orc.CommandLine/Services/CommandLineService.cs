namespace Orc.CommandLine
{
    using System.Linq;

    public class CommandLineService : ICommandLineService
    {
        public virtual string GetCommandLine()
        {
            var commandArguments = System.Environment.GetCommandLineArgs().Skip(1).ToArray();

            var commandLine = string.Join(" ", commandArguments.Select(arg => arg));

            return commandLine;
        }
    }
}
