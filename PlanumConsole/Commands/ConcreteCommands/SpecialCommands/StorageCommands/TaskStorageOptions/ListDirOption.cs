using Planum.Config;
using Planum.Logger;

namespace Planum.Console.Commands.Special
{
    public class ListDirOption: BaseOption<TaskStorageCommandSettings>
    {
        public ListDirOption(ILoggerWrapper logger, OptionInfo optionInfo, ConsoleConfig commandConfig) : base(logger, optionInfo, commandConfig) { }
        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskStorageCommandSettings result)
        {
            result.ListResult = true;
            return true;
        }
    }
}
