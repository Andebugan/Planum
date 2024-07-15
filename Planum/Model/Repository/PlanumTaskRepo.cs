using Planum.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace Planum.Model.Repository
{

    public class PlanumTaskRepo : IPlanumTaskRepo
    {
        protected IEnumerable<PlanumTask> taskBuffer = new List<PlanumTask>();
        IPlanumTaskFileManager PlanumTaskFileManager { get; set; }

        public PlanumTaskRepo(IPlanumTaskFileManager planumTaskFileManager)
        {
            PlanumTaskFileManager = planumTaskFileManager;
            Load();
        }

        public void Save() => PlanumTaskFileManager.Write(taskBuffer);
        public void Load() => taskBuffer = PlanumTaskFileManager.Read();

        public IEnumerable<PlanumTask> Get() => taskBuffer;

        public void Add(PlanumTask task) => taskBuffer.Append(task);
        public void Add(IEnumerable<PlanumTask> tasks) => taskBuffer = taskBuffer.Concat(tasks);

        public void Update(PlanumTask task) => Update(new PlanumTask[] { task });
        public void Update(IEnumerable<PlanumTask> tasks)
        {
            var ids = tasks.Select(x => x.Id);
            taskBuffer = taskBuffer.Where(x => !ids.Contains(x.Id)).Concat(tasks);
            PlanumTaskFileManager.Write(taskBuffer);
        }

        public void Delete(Guid id) => Delete(new Guid[] { id });
        public void Delete(IEnumerable<Guid> ids)
        {
            taskBuffer = taskBuffer.Where(x => ids.Contains(x.Id));
            PlanumTaskFileManager.Write(taskBuffer, true);
        }
    }
}
