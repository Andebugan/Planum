using System;
using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Entities;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class DeadlineIdOption: BaseOption<TaskCommandSettings>
    {
        public DeadlineIdOption(OptionInfo optionInfo, CommandConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            Guid id = Guid.Empty;
            if (!ValueParser.TryParse(ref id, args.Current))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse deadline guid id from: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                if (result.CurrentDeadline == null)
                    result.CurrentDeadline = new Deadline();
                result.CurrentDeadline.Id = id;
            }
            return true;
        }
    }
}
