using System.Collections.Generic;
using Planum.Model.Entities;

namespace Planum.Model.Repository
{
    public interface IPlanumTaskWriter
    {
        public IEnumerable<string> WriteID(IEnumerable<string> lines, PlanumTask task);
        public IEnumerable<string> WriteName(IEnumerable<string> lines, PlanumTask task);
        public IEnumerable<string> WriteDescription(IEnumerable<string> lines, PlanumTask task);
        public IEnumerable<string> WriteParents(IEnumerable<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks);

        public IEnumerable<string> WriteChildren(IEnumerable<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks);
        public IEnumerable<string> WriteChecklistItem(IEnumerable<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, int level = 0);
        public IEnumerable<string> WriteDeadline(IEnumerable<string> lines, PlanumTask task);

        public IEnumerable<string> WriteTask(IEnumerable<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks);
    }
}
