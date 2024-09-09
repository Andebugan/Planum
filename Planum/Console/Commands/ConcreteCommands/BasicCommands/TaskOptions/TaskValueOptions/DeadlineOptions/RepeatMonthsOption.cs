using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Entities;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class RepeatMonthsOption: BaseOption<TaskCommandSettings>
    {
        public RepeatMonthsOption(OptionInfo optionInfo, CommandConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            int months = 0;
            if (!ValueParser.TryParse(ref months, args.Current))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse deadline repeat months from: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                if (result.CurrentDeadline == null)
                    result.CurrentDeadline = new Deadline();
                result.CurrentDeadline.repeatMonths = months;
            }
            return true;
        }
    }
}
