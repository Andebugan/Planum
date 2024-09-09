using System;
using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Entities;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class WarningOption: BaseOption<TaskCommandSettings>
    {
        public WarningOption(OptionInfo optionInfo, CommandConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            TimeSpan warning = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref warning, args.Current))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse deadline duration from: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                if (result.CurrentDeadline == null)
                    result.CurrentDeadline = new Deadline();
                result.CurrentDeadline.warningTime = warning;
            }
            return true;
        }
    }
}
