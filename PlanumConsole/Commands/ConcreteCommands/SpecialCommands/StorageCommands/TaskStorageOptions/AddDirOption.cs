using Planum.Config;
using Planum.Logger;

namespace Planum.Console.Commands.Special
{
    public class AddDirOption: BaseOption<TaskStorageCommandSettings>
    {
        public AddDirOption(ILoggerWrapper logger, OptionInfo optionInfo, ConsoleConfig commandConfig) : base(logger, optionInfo, commandConfig) { }
        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskStorageCommandSettings result)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            var fullDirPath = Path.GetFullPath(Path.Combine(args.Current));
            var relativeDirPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), args.Current));
            if (!Directory.Exists(fullDirPath) && !Directory.Exists(relativeDirPath))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to find directory at path: \"{fullDirPath}\" or \"{relativeDirPath}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            if (Directory.Exists(fullDirPath))
                result.TaskLookupPaths.Add(fullDirPath);
            else if (Directory.Exists(relativeDirPath))
                result.TaskLookupPaths.Add(relativeDirPath);

            return true;
        }
    }
}
