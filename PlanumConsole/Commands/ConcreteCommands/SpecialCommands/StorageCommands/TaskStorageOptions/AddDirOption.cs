using Planum.Config;

namespace Planum.Console.Commands.Special
{
    public class AddDirOption: BaseOption<TaskStorageCommandSettings>
    {
        public AddDirOption(OptionInfo optionInfo, ConsoleConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskStorageCommandSettings result)
        {
            var fullDirPath = Path.Combine(args.Current);
            var relativeDirPath = Path.Combine(Directory.GetCurrentDirectory(), args.Current);
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
