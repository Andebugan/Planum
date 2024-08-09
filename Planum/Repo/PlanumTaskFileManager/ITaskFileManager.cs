using System.Collections.Generic;
using Planum.Model.Entities;

namespace Planum.Repository
{
    public interface ITaskFileManager
    {
        public IEnumerable<PlanumTask> Read(ref ReadStatus readStatus);
        public void Write(IEnumerable<PlanumTask> tasks, ref WriteStatus writeStatus, ref ReadStatus readStatus);
    }
}
