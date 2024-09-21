using Planum.Config;
using Planum.Logger;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class RemoveDeadlineOption: BaseOption<TaskCommandSettings>
    {
        public RemoveDeadlineOption(ILoggerWrapper logger, OptionInfo optionInfo, ConsoleConfig commandConfig) : base(logger, optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            Guid id = Guid.Empty;

            ValueMatchType equal = ValueMatchType.IGNORE;
            ValueMatchType inCompared = ValueMatchType.IGNORE;
            if (ValueParser.TryParse(ref id, args.Current))
               equal = ValueMatchType.NOT;
            else
                inCompared = ValueMatchType.NOT;

            ValueMatch<Guid> valueMatch = new ValueMatch<Guid>(id, args.Current, equal: equal, valueStrInCompared: inCompared);
            ValueFilter<Guid> idFilter = new ValueFilter<Guid>(Logger);
            idFilter.AddMatch(valueMatch);
            DeadlineFilter deadlineFilter = new DeadlineFilter(Logger, idFilter);

            foreach (var task in result.Tasks)
                task.Deadlines = deadlineFilter.Filter(task.Deadlines).ToHashSet();

            return true;
        }
    }
}
