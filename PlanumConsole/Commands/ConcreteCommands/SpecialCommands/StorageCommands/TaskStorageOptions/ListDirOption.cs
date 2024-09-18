using Planum.Config;

namespace Planum.Console.Commands.Special
{
    public class ListDirOption: BaseOption<TaskStorageCommandSettings>
    {
        public ListDirOption(OptionInfo optionInfo, ConsoleConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskStorageCommandSettings result)
        {
            result.ListResult = true;
            return true;
        }
    }
}
