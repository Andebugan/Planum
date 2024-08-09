using System;
using System.Collections.Generic;
using Planum.Model.Entities;
using Planum.Repository;

namespace Planum.Model.Repository
{
    public interface ITaskRepo
    {
        public void Save(ref WriteStatus writeStatus, ref ReadStatus readStatus);
        public void Load(ref ReadStatus readStatus);

        public IEnumerable<PlanumTask> Get();

        public void Add(PlanumTask task);
        public void Add(IEnumerable<PlanumTask> tasks);

        public void Update(PlanumTask task, ref WriteStatus writeStatus, ref ReadStatus readStatus);
        public void Update(IEnumerable<PlanumTask> tasks, ref WriteStatus writeStatus, ref ReadStatus readStatus);
        
        public void Delete(Guid id, ref WriteStatus writeStatus, ref ReadStatus readStatus);
        public void Delete(IEnumerable<Guid> ids, ref WriteStatus writeStatus, ref ReadStatus readStatus);
    }
}
