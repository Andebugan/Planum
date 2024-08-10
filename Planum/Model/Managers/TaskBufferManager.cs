﻿using System.Collections.Generic;
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

        public TaskBufferManager(ITaskRepo planumTaskRepo)
        {
            TaskRepo = planumTaskRepo;
        }

        public IEnumerable<PlanumTask> Find(ITaskFilter? taskFilter = null)
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
