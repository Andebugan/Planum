using Planum.Model.Entities;
using Planum.Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace Planum.Repository
{

    public class PlanumTaskRepo
    {
        protected IEnumerable<PlanumTask> taskBuffer = new List<PlanumTask>();
        IPlanumTaskFileManager PlanumTaskFileManager { get; set; }

        public PlanumTaskRepo(IPlanumTaskFileManager planumTaskFileManager) => PlanumTaskFileManager = planumTaskFileManager;

        public void Save(ref TaskFileManagerWriteStatus writeStatus, ref TaskFileManagerReadStatus readStatus) => PlanumTaskFileManager.Write(taskBuffer, ref writeStatus, ref readStatus);
        public void Load(ref TaskFileManagerReadStatus readStatus) => taskBuffer = PlanumTaskFileManager.Read(ref readStatus);

        public IEnumerable<PlanumTask> Get() => taskBuffer;

        public void Add(PlanumTask task) => taskBuffer.Append(task);
        public void Add(IEnumerable<PlanumTask> tasks) => taskBuffer = taskBuffer.Concat(tasks);

        public void Update(PlanumTask task, ref TaskFileManagerWriteStatus writeStatus, ref TaskFileManagerReadStatus readStatus) => Update(new PlanumTask[] { task }, ref writeStatus, ref readStatus);
        public void Update(IEnumerable<PlanumTask> tasks, ref TaskFileManagerWriteStatus writeStatus, ref TaskFileManagerReadStatus readStatus)
        {
            var ids = tasks.Select(x => x.Id);
            taskBuffer = taskBuffer.Where(x => !ids.Contains(x.Id)).Concat(tasks);
            PlanumTaskFileManager.Write(taskBuffer, ref writeStatus, ref readStatus);
        }

        public void Delete(Guid id, ref TaskFileManagerWriteStatus writeStatus, ref TaskFileManagerReadStatus readStatus) => Delete(new Guid[] { id }, ref writeStatus, ref readStatus);
        public void Delete(IEnumerable<Guid> ids, ref TaskFileManagerWriteStatus writeStatus, ref TaskFileManagerReadStatus readStatus)
        {
            taskBuffer = taskBuffer.Where(x => ids.Contains(x.Id));
            PlanumTaskFileManager.Write(taskBuffer, ref writeStatus, ref readStatus);
        }
    }
}
