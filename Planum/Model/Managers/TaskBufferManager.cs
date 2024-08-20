using System.Collections.Generic;
using System.Linq;
using Planum.Model.Entities;
using Planum.Model.Filters;
using Planum.Model.Repository;
using Planum.Repository;

namespace Planum.Model.Managers
{
    public class TaskBufferManager
    {
        protected ITaskRepo TaskRepo { get; set; }
        protected TaskValidationManager TaskValidationManager { get; set; }

        public TaskBufferManager(ITaskRepo planumTaskRepo, TaskValidationManager taskValidationManager)
        {
            TaskRepo = planumTaskRepo;
            TaskValidationManager = taskValidationManager;
        }

        public IEnumerable<PlanumTask> Find(ITaskFilter? taskFilter = null)
        {
            var tasks = TaskRepo.Get();
            if (taskFilter is null)
                return tasks;
            else
                return taskFilter.Filter(tasks);
        }

        public IEnumerable<TaskValidationResult> Add(PlanumTask task)
        {
            var validationResults = new List<TaskValidationResult>();
            TaskValidationManager.ValidateTask(task, ref validationResults);
            if (validationResults.Any())
                TaskRepo.Add(task);
            return validationResults;
        }

        public IEnumerable<TaskValidationResult> Add(IEnumerable<PlanumTask> tasks)
        {
            var validationResults = new List<TaskValidationResult>();
            TaskValidationManager.ValidateTask(tasks, ref validationResults);
            if (validationResults.Any())
                TaskRepo.Add(tasks);
            return validationResults;
        }

        public IEnumerable<TaskValidationResult> Update(PlanumTask task)
        {
            var validationResults = new List<TaskValidationResult>();
            TaskValidationManager.ValidateTask(task, ref validationResults);
            if (validationResults.Any())
                TaskRepo.Update(task);
            return validationResults;
        }

        public IEnumerable<TaskValidationResult> Update(IEnumerable<PlanumTask> tasks)
        {
            var validationResults = new List<TaskValidationResult>();
            TaskValidationManager.ValidateTask(tasks, ref validationResults);
            if (validationResults.Any())
                TaskRepo.Update(tasks);
            return validationResults;
        }

        public void Delete(ITaskFilter? taskFilter = null)
        {
            var tasks = TaskRepo.Get();
            if (taskFilter is not null)
                tasks = taskFilter.Filter(tasks);
            TaskRepo.Delete(tasks.Select(x => x.Id));
        }

        public void Save(ref WriteStatus writeStatus, ref ReadStatus readStatus) => TaskRepo.Save(ref writeStatus, ref readStatus);
        public void Load(ref ReadStatus readStatus) => TaskRepo.Load(ref readStatus);
    }
}
