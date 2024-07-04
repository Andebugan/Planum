using Planum.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace Planum.Model.Repository
{

    public class Repo : IRepo
    {
        private IEnumerable<PlanumTask> taskBuffer = new List<PlanumTask>();

        ITaskFileManager taskFileManager;

        public Repo()
        {
            taskFileManager = new TaskFileManager();
            Sync();
        }

        public PlanumTask? Find(Guid id)
        {
            var result = taskBuffer.Where(x => x.Id == id);
            if (result.Count() == 0)
                return null;
            return result.First();
        }

        public void Sync()
        {
            taskBuffer = taskFileManager.Read();
        }

        public IEnumerable<PlanumTask> Find(IEnumerable<Guid>? ids = null)
        {
            return ids == null ? taskBuffer.ToList() : taskBuffer.Where(x => ids.Contains(x.Id)).ToList();
        }

        public void Add(PlanumTask task)
        {
            taskBuffer.Append(task);
            taskFileManager.Write(taskBuffer);
        }

        public void Update(PlanumTask task)
        {
            Update(new PlanumTask[] { task });
        }

        public void Update(IEnumerable<PlanumTask> tasks)
        {
            var ids = tasks.Select(x => x.Id);
            taskBuffer = taskBuffer.Where(x => !ids.Contains(x.Id)).Concat(tasks);
            taskFileManager.Write(taskBuffer);
        }

        public void Delete(Guid id)
        {
            Delete(new Guid[] { id });
        }

        public void Delete(IEnumerable<Guid> ids)
        {
            taskBuffer = taskBuffer.Where(x => !ids.Contains(x.Id));
            taskFileManager.Write(taskBuffer);
        }

        public void Backup()
        {
            taskFileManager.Backup();
        }

        public void Restore()
        {
            taskFileManager.Restore();
        }
    }
}
