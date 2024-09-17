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
        public IEnumerable<PlanumTask> GetDiff();

        public void Add(PlanumTask task);
        public void Add(IEnumerable<PlanumTask> tasks);

        public void Update(PlanumTask task);
        public void Update(IEnumerable<PlanumTask> tasks);
        
        public void Delete(Guid id);
        public void Delete(IEnumerable<Guid> ids);
    }
}
