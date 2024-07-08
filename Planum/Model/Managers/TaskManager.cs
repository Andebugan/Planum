using System.Collections.Generic;
using Planum.Model.Entities;
using Planum.Model.Repository;

namespace Planum.Model.Managers
{
    public class TaskManager
    {
        protected IRepo PlanumTaskRepo { get; set; }

        public TaskManager(IRepo planumTaskRepo)
        {
            PlanumTaskRepo = planumTaskRepo;
        }

        public void Backup() => PlanumTaskRepo.Backup();
        public void Restore() => PlanumTaskRepo.Restore();

        public void Save() => PlanumTaskRepo.Save();
        public void Load() => PlanumTaskRepo.Load();
    }
}
