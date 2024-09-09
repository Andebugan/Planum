using System;
using System.Collections.Generic;
using Planum.Config;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class IdOption: BaseOption<TaskCommandSettings>
    {
        public IdOption(OptionInfo optionInfo, CommandConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            Guid id = Guid.Empty;
            if (!ValueParser.TryParse(ref id, args.Current))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse guid from: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
                result.Id = id;
            return true;
        }
    }
}
