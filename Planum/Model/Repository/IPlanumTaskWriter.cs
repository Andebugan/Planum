using System.Collections.Generic;
using Planum.Model.Entities;

namespace Planum.Model.Repository
{
    public interface IPlanumTaskWriter
    {
        public void WriteTask(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks);
    }
}
