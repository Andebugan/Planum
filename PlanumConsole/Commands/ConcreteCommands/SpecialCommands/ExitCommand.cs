using Planum.Config;
using Planum.Logger;

namespace Planum.Console.Commands
{
    public class ExitCommandSettings
    {
    }

    public class ExitCommand: BaseCommand<ExitCommandSettings>
    {
        RepoConfig RepoConfig { get; set; }

        public ExitCommand(RepoConfig repoConfig, CommandInfo commandInfo, List<BaseOption<ExitCommandSettings>> commandOptions, ILoggerWrapper logger) : base(commandInfo, commandOptions, logger)
        {
            RepoConfig = repoConfig;
        }

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            Logger.Log("Executing exit command");
            Logger.Log("Successfully executed exit command");
            return new List<string>();
        }
    }
}
