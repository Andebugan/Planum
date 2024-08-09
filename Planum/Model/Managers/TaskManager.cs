using System.Collections.Generic;
using System.Linq;
using Planum.Model.Entities;
using Planum.Model.Filters;
using Planum.Model.Repository;

namespace Planum.Model.Managers
{
    public class TaskManager
    {
        protected ITaskRepo TaskRepo { get; set; }

        public TaskManager(ITaskRepo planumTaskRepo)
        {
            TaskRepo = planumTaskRepo;
        }

        public IEnumerable<PlanumTask> Find(IPlanumTaskFilter? taskFilter = null)
        {
            var tasks = TaskRepo.Get();
            if (taskFilter is null)
                return tasks;
            else
                return taskFilter.Filter(tasks);
        }

        public void Add(PlanumTask task) => TaskRepo.Add(task);
        public void Add(IEnumerable<PlanumTask> tasks) => TaskRepo.Add(tasks);

        public void Update(PlanumTask task) => TaskRepo.Update(task);
        public void Update(IEnumerable<PlanumTask> tasks) => TaskRepo.Update(tasks);

        public void Delete(IPlanumTaskFilter? taskFilter = null)
        {
            var tasks = TaskRepo.Get();
            if (taskFilter is not null)
                tasks = taskFilter.Filter(tasks);
            TaskRepo.Delete(tasks.Select(x => x.Id));
        }

        public void Save() => TaskRepo.Save();
        public void Load() => TaskRepo.Load();
    }
}
