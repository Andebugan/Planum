using Planum.Config;

namespace Planum.Console.Commands.Special
{
    public class RemoveDirOption: BaseOption<TaskStorageCommandSettings>
    {
        public RemoveDirOption(OptionInfo optionInfo, ConsoleConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskStorageCommandSettings result)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            var fullDirPath = Path.Combine(args.Current);
            var relativeDirPath = Path.Combine(Directory.GetCurrentDirectory(), args.Current);

            result.TaskLookupPaths.Remove(fullDirPath);
            result.TaskLookupPaths.Remove(relativeDirPath);

            return true;
        }
    }
}
