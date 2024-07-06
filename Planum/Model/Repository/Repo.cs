using Planum.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace Planum.Model.Repository
{

    public class Repo : IRepo
    {
        protected IEnumerable<PlanumTask> taskBuffer = new List<PlanumTask>();
        ITaskFileManager taskFileManager;

        public Repo()
        {
            taskFileManager = new TaskFileManager();
            Load();
        }

        public void Save() => taskFileManager.Write(taskBuffer);
        public void Load() => taskBuffer = taskFileManager.Read();

        public IEnumerable<PlanumTask> Get() => taskBuffer;

        public void Add(PlanumTask task) => taskBuffer.Append(task);
        public void Add(IEnumerable<PlanumTask> tasks) => taskBuffer = taskBuffer.Concat(tasks);

        public void Update(PlanumTask task) => Update(new PlanumTask[] { task });
        public void Update(IEnumerable<PlanumTask> tasks)
        {
            var ids = tasks.Select(x => x.Id);
            taskBuffer = taskBuffer.Where(x => !ids.Contains(x.Id)).Concat(tasks);
            taskFileManager.Write(taskBuffer);
        }

        public void Delete(Guid id) => Delete(new Guid[] { id });
        public void Delete(IEnumerable<Guid> ids)
        {
            taskBuffer = taskBuffer.Where(x => !ids.Contains(x.Id));
            taskFileManager.Write(taskBuffer);
        }

        public void Backup() => taskFileManager.Backup();
        public void Restore() => taskFileManager.Restore();
    }
}
