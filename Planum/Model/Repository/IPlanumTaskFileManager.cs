using System.Collections.Generic;
using Planum.Model.Entities;

namespace Planum.Model.Repository {
  public interface IPlanumTaskFileManager {
    public IEnumerable<PlanumTask> Read();
    public void Write(IEnumerable<PlanumTask> tasks);
    public void Backup();
    public void Restore();
  }
}
