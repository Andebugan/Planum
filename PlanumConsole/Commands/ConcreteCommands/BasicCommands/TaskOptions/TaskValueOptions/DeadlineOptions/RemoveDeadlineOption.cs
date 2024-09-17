using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class RemoveDeadlineOption: BaseOption<TaskCommandSettings>
    {
        public RemoveDeadlineOption(OptionInfo optionInfo, CommandConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            Guid id = Guid.Empty;

            MatchType equal = MatchType.IGNORE;
            MatchType inCompared = MatchType.IGNORE;
            if (ValueParser.TryParse(ref id, args.Current))
               equal = MatchType.NOT;
            else
                inCompared = MatchType.NOT;

            ValueMatch<Guid> valueMatch = new ValueMatch<Guid>(id, args.Current, equal: equal, valueStrInCompared: inCompared);
            ValueFilter<Guid> idFilter = new ValueFilter<Guid>();
            idFilter.AddMatch(valueMatch);
            DeadlineFilter deadlineFilter = new DeadlineFilter(idFilter);

            foreach (var task in result.Tasks)
                task.Deadlines = deadlineFilter.Filter(task.Deadlines).ToHashSet();

            return true;
        }
    }
}
