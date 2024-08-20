using System.Collections.Generic;

namespace Planum.Commands
{
    public interface IOption<T>
    {
        public OptionInfo OptionInfo { get; set; }
        
        public bool CheckMatch(string value);

        public bool TryParseValue(ref IEnumerator<string> args, ref T result);
    }
}
