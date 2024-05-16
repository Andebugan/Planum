using System;
using System.Collections.Generic;
using Planum.Model.Entities;

namespace Planum.Model.Repository {
  public interface IRepo {
    public void Sync();
    public void Add(Task task);
    public Task? Find(Guid id);
    public IEnumerable<Task> Find(IEnumerable<Guid>? ids = null);
    public void Update(Task task);
    public void Update(IEnumerable<Task> tasks);
    public void Delete(Guid id);
    public void Delete(IEnumerable<Guid> ids);
    public void Backup();
    public void Restore();
  }
}
