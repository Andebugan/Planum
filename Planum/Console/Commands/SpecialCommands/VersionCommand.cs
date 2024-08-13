using System.Collections.Generic;

namespace Planum.Commands
{
    public class VersionCommand : ICommand
    {
        public bool CheckMatch(ref IEnumerator<string> args)
        {
            throw new System.NotImplementedException();
        }

        public List<string> Execute(ref IEnumerator<string> args)
        {
            throw new System.NotImplementedException();
        }

        public CommandInfo GetCommandInfo()
        {
            throw new System.NotImplementedException();
        }
    }
}
