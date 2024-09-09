using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Entities;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class RepeatedOption: BaseOption<TaskCommandSettings>
    {
        public RepeatedOption(OptionInfo optionInfo, CommandConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            bool repeated = false;
            if (!ValueParser.TryParse(ref repeated, args.Current))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse deadline repeated from: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                if (result.CurrentDeadline == null)
                    result.CurrentDeadline = new Deadline();
                result.CurrentDeadline.repeated = repeated;
            }
            return true;
        }
    }
}
