using Planum.Logger;
using Planum.Model.Entities;
using Planum.Model.Repository;
#nullable enable

namespace Planum.Repository
{
    /// <summary>
    /// Class for buffered loading, updating and commiting task data
    /// </summary>
    public class TaskRepo: ITaskRepo
    {
        protected ILoggerWrapper Logger;
        protected List<PlanumTask> taskBuffer = new List<PlanumTask>();
        protected List<PlanumTask> taskOldBuffer = new List<PlanumTask>();
        ITaskFileManager FileManager { get; set; }

        public TaskRepo(ILoggerWrapper logger, ITaskFileManager taskFileManager)
        {
            Logger = logger;
            FileManager = taskFileManager;
        }

        public void Save(ref WriteStatus writeStatus, ref ReadStatus readStatus)
        {
            FileManager.Write(taskBuffer, ref writeStatus, ref readStatus);
            taskOldBuffer = taskBuffer;
        }

        public void Load(ref ReadStatus readStatus)
        {
            taskBuffer = FileManager.Read(ref readStatus).ToList();
            taskOldBuffer = taskBuffer;
        }

        public IEnumerable<PlanumTask> Get() => taskBuffer;
        public IEnumerable<PlanumTask> GetDiff() => taskBuffer.Where(x => !taskOldBuffer.Contains(x));

        public void Add(PlanumTask task) => taskBuffer.Add(task);

        public void Add(IEnumerable<PlanumTask> tasks) => taskBuffer = taskBuffer.Concat(tasks).ToList();

        public void Update(PlanumTask task) => Update(new PlanumTask[] { task });
        public void Update(IEnumerable<PlanumTask> tasks)
        {
            var ids = tasks.Select(x => x.Id);
            taskBuffer = taskBuffer.Where(x => !ids.Contains(x.Id)).Concat(tasks).ToList();
        }

        public void Delete(Guid id) => Delete(new Guid[] { id });
        public void Delete(IEnumerable<Guid> ids)
        {
            taskBuffer = taskBuffer.Where(x => !ids.Contains(x.Id)).ToList();
        }
    }
}
