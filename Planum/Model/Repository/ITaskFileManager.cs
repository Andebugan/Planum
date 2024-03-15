using System.Collections.Generic;
using Planum.Model.Entities;

namespace Planum.Model.Repository {
  public interface ITaskFileManager {
    public IEnumerable<Task> Read();
    public void Write(IEnumerable<Task> tasks);
    public void Backup();
    public void Restore();
  }
}
