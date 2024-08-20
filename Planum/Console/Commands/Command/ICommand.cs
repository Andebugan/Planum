using System.Collections.Generic;

namespace Planum.Commands
{
    public interface ICommand<T>
    {
        public List<IOption<T>> CommandOptions { get; set; }
        public bool CheckMatch(ref IEnumerator<string> args);
        public abstract List<string> Execute(ref IEnumerator<string> args);
    }
}
