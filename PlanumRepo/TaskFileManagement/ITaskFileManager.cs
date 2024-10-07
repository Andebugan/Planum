using Planum.Model.Entities;

namespace Planum.Repository
{
    public interface ITaskFileManager
    {
        public IEnumerable<PlanumTask> Read();
        public void Write(IEnumerable<PlanumTask> tasks);
    }
}
