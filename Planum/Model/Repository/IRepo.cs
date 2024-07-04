using System;
using System.Collections.Generic;
using Planum.Model.Entities;

namespace Planum.Model.Repository {
  public interface IRepo {
    public void Sync();
    public void Add(PlanumTask task);
    public PlanumTask? Find(Guid id);
    public IEnumerable<PlanumTask> Find(IEnumerable<Guid>? ids = null);
    public void Update(PlanumTask task);
    public void Update(IEnumerable<PlanumTask> tasks);
    public void Delete(Guid id);
    public void Delete(IEnumerable<Guid> ids);
    public void Backup();
    public void Restore();
  }
}
