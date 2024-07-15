using System.Collections.Generic;
using System.Linq;
using Planum.Model.Entities;
using Planum.Model.Filters;
using Planum.Model.Repository;

namespace Planum.Model.Managers
{
    public class PlanumTaskManager
    {
        protected IRepo PlanumTaskRepo { get; set; }

        public PlanumTaskManager(IRepo planumTaskRepo)
        {
            PlanumTaskRepo = planumTaskRepo;
        }

        public IEnumerable<PlanumTask> Find(IPlanumTaskFilter? taskFilter = null)
        {
            var tasks = PlanumTaskRepo.Get();
            if (taskFilter is null)
                return tasks;
            else
                return taskFilter.Filter(tasks);
        }

        public void Add(PlanumTask task) => PlanumTaskRepo.Add(task);
        public void Add(IEnumerable<PlanumTask> tasks) => PlanumTaskRepo.Add(tasks);

        public void Update(PlanumTask task) => PlanumTaskRepo.Update(task);
        public void Update(IEnumerable<PlanumTask> tasks) => PlanumTaskRepo.Update(tasks);

        public void Delete(IPlanumTaskFilter? taskFilter = null)
        {
            var tasks = PlanumTaskRepo.Get();
            if (taskFilter is not null)
                tasks = taskFilter.Filter(tasks);
            PlanumTaskRepo.Delete(tasks.Select(x => x.Id));
        }

        public void Backup() => PlanumTaskRepo.Backup();
        public void Restore() => PlanumTaskRepo.Restore();

        public void Save() => PlanumTaskRepo.Save();
        public void Load() => PlanumTaskRepo.Load();
    }
}
