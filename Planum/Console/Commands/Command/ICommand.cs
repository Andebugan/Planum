using System.Collections.Generic;

namespace Planum.Commands
{
    public interface ICommand
    {
        public IEnumerable<IOption> CommandOptions { get; }
        public bool CheckMatch(string value);
        public abstract List<string> Execute(ref IEnumerator<string> args);
    }
}
