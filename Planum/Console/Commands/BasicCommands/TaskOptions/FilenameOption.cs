using System.Collections.Generic;
using System.IO;
using Planum.Config;

namespace Planum.Commands
{
    public class FilenameOption: BaseOption<TaskCommandSettings>
    {
        public FilenameOption(OptionInfo optionInfo, CommandConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            var fullfilepath = Path.Combine(args.Current);
            var relativeFilepath = Path.Combine(Directory.GetCurrentDirectory(), args.Current);
            if (!File.Exists(fullfilepath) && !File.Exists(relativeFilepath))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to find file at path: \"{fullfilepath}\" or \"{relativeFilepath}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            result.Filename = args.Current;
            return true;
        }
    }
}
