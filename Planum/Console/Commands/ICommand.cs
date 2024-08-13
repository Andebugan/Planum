using System.Collections.Generic;

namespace Planum.Commands
{
    public interface ICommand
    {
        public bool CheckMatch(ref IEnumerator<string> args);
        public List<string> Execute(ref IEnumerator<string> args);

        public CommandInfo GetCommandInfo();
    }
}
