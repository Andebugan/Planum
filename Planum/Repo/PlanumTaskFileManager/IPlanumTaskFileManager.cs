using System.Collections.Generic;
using Planum.Model.Entities;

namespace Planum.Repository
{
    public interface IPlanumTaskFileManager
    {
        public IEnumerable<PlanumTask> Read(ref TaskFileManagerReadStatus readStatus);
        public void Write(IEnumerable<PlanumTask> tasks, ref TaskFileManagerWriteStatus writeStatus, ref TaskFileManagerReadStatus readStatus);
    }
}
