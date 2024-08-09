using Planum.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace Planum.Repository
{

    public class TaskRepo
    {
        protected IEnumerable<PlanumTask> taskBuffer = new List<PlanumTask>();
        ITaskFileManager FileManager { get; set; }

        public TaskRepo(ITaskFileManager planumTaskFileManager) => FileManager = planumTaskFileManager;

        public void Save(ref WriteStatus writeStatus, ref ReadStatus readStatus) => FileManager.Write(taskBuffer, ref writeStatus, ref readStatus);
        public void Load(ref ReadStatus readStatus) => taskBuffer = FileManager.Read(ref readStatus);

        public IEnumerable<PlanumTask> Get() => taskBuffer;

        public void Add(PlanumTask task) => taskBuffer.Append(task);
        public void Add(IEnumerable<PlanumTask> tasks) => taskBuffer = taskBuffer.Concat(tasks);

        public void Update(PlanumTask task, ref WriteStatus writeStatus, ref ReadStatus readStatus) => Update(new PlanumTask[] { task }, ref writeStatus, ref readStatus);
        public void Update(IEnumerable<PlanumTask> tasks, ref WriteStatus writeStatus, ref ReadStatus readStatus)
        {
            var ids = tasks.Select(x => x.Id);
            taskBuffer = taskBuffer.Where(x => !ids.Contains(x.Id)).Concat(tasks);
            FileManager.Write(taskBuffer, ref writeStatus, ref readStatus);
        }

        public void Delete(Guid id, ref WriteStatus writeStatus, ref ReadStatus readStatus) => Delete(new Guid[] { id }, ref writeStatus, ref readStatus);
        public void Delete(IEnumerable<Guid> ids, ref WriteStatus writeStatus, ref ReadStatus readStatus)
        {
            taskBuffer = taskBuffer.Where(x => ids.Contains(x.Id));
            FileManager.Write(taskBuffer, ref writeStatus, ref readStatus);
        }
    }
}
