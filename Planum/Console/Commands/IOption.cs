using System.Collections.Generic;

namespace Planum.Commands
{
    public interface IOption
    {
        public bool TryParseValue<T>(ref IEnumerator<string> args, ref T result);

        public OptionInfo GetOptionInfo();
    }
}
