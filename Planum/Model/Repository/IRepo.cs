using System.Collections.Generic;
using Planum.Model.Entities;

namespace Planum.Model.Repository {
  public interface IRepo {
    public int Add(Task task);
    public Task? Find(int id);
    public IEnumerable<Task> Find(IEnumerable<int>? ids = null);
    public void Update(Task task);
    public void Update(IEnumerable<Task> tasks);
    public void Delete(int id);
    public void Delete(IEnumerable<int> ids);
    public void Backup();
    public void Restore();
  }
}
