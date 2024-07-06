using System;
using System.Collections.Generic;
using Planum.Model.Entities;

namespace Planum.Model.Repository {
  public interface IRepo {

    public void Save();
    public void Load();

    public IEnumerable<PlanumTask> Get();

    public void Add(PlanumTask task);
    public void Add(IEnumerable<PlanumTask> tasks);

    public void Update(PlanumTask task);
    public void Update(IEnumerable<PlanumTask> tasks);
    
    public void Delete(Guid id);
    public void Delete(IEnumerable<Guid> ids);

    public void Backup();
    public void Restore();
  }
}
