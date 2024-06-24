using Planum.Model.Repository;

namespace Planum.Model.Managers
{
    public class TaskManager
    {
        protected Repo repo ;

        public TaskManager()
        {
            repo = new Repo();
        }
        public void Backup()
        {
            repo.Backup();
        }

        public void Restore()
        {
            repo.Restore();
        }

        // TODO
        public void Undo()
        {
            
        }
    }
}
