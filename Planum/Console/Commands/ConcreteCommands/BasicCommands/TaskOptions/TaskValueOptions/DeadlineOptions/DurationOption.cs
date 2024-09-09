using System;
using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Entities;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class DurationOption: BaseOption<TaskCommandSettings>
    {
        public DurationOption(OptionInfo optionInfo, CommandConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            TimeSpan duration = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref duration, args.Current))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse deadline duration from: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                if (result.CurrentDeadline == null)
                    result.CurrentDeadline = new Deadline();
                result.CurrentDeadline.duration = duration;
            }
            return true;
        }
    }
}
