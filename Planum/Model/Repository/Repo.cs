using Planum.Model.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Planum.Model.Repository
{

  public class Repo : IRepo
  {
    IEnumerable<Task> taskBuffer = new List<Task>();
    IEnumerable<Task> TaskBuffer
    {
      get
      {
        return taskBuffer;
      }
      set
      {
        taskBuffer = value.OrderBy(x => x.Id).ToList();
      }
    }

    ITaskFileManager taskFileManager;

    public Repo() { }

    public Task? Find(int id)
    {
      var result = TaskBuffer.Where(x => x.Id == id);
      if (result.Count() == 0)
        return null;
      return result.First();
    }

    public IEnumerable<Task> Find(List<int>? ids = null)
    {
      return ids == null ? TaskBuffer : TaskBuffer.Where(x => ids.Contains(x.Id));
    }

    public int Add(Task task)
    {
      var ids = TaskBuffer.Select(x => x.Id).ToList();
      ids.Sort();
      int id = 1;
      while (ids.Contains(id)) { id++; }

      task = new Task(id, task);
      TaskBuffer.Append(task);
      return id;
    }

    public IEnumerable<Task> Find(IEnumerable<int>? ids = null)
    {
      if (ids is null)
        return TaskBuffer;
      return TaskBuffer.Where(x => ids.Contains(x.Id));
    }

    public void Update(Task task)
    {
      Update(new Task[] { task });
    }

    public void Update(IEnumerable<Task> tasks)
    {
      var ids = tasks.Select(x => x.Id);
      TaskBuffer = taskBuffer.Where(x => !ids.Contains(x.Id)).Concat(tasks);
      taskFileManager.Write(TaskBuffer);
    }

    public void Delete(int id)
    {
      Delete(new int[] { id });
    }

    public void Delete(IEnumerable<int> ids)
    {
      TaskBuffer = taskBuffer.Where(x => !ids.Contains(x.Id));
      taskFileManager.Write(TaskBuffer);
    }

    public void Backup()
    {
      taskFileManager.Backup();
    }

    public void Restore()
    {
      taskFileManager.Restore();
    }
  }
}
