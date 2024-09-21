using Planum.Config;

namespace Planum.Console.Commands.Task
{
    public class FilenameOption: BaseOption<TaskCommandSettings>
    {
        public FilenameOption(OptionInfo optionInfo, ConsoleConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            var fullfilepath = Path.Combine(args.Current);
            var relativeFilepath = Path.Combine(Directory.GetCurrentDirectory(), args.Current);
            if (!File.Exists(fullfilepath) && !File.Exists(relativeFilepath))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to find file at path: \"{fullfilepath}\" or \"{relativeFilepath}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            foreach (var task in result.Tasks)
                task.SaveFile = args.Current;

            return true;
        }
    }
}
