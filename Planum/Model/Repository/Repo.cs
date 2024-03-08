using Planum.Model.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Planum.Model.Repository
{

    public class Repo
    {
        List<Task> taskBuffer = new List<Task>();

        TaskFileManager taskFileManager = new TaskFileManager();

        public Repo()
        {
            taskBuffer = taskFileManager.ReadAll();
            taskBuffer = taskBuffer.OrderBy(x => x.Id).ToList();
        }

        public int Add(Task obj)
        {
            Task objReady = new Task(taskFileManager.GetFreeId(), obj);

            taskFileManager.Write(new List<Task> { objReady }, true);

            taskBuffer.Add(objReady);
            taskBuffer = taskBuffer.OrderBy(x => x.Id).ToList();

            return objReady.Id;
        }

        public Task? Find(int id)
        {
            var result = taskBuffer.Where(x => x.Id == id);
            if (result.Count() == 0)
                return null;
            return result.First();
        }

        public List<Task> Find(List<int>? ids = null)
        {
            if (ids == null)
                return taskBuffer;
            else
                return taskBuffer.Where(x => ids.Contains(x.Id)).OrderBy(x => x.Id).ToList();
        }

        public void Update(Task obj)
        {
            Update(new List<Task>() { obj });
        }

        public void Update(List<Task> objs)
        {
            taskBuffer = taskBuffer.Where(x => !objs.Exists(y => y.Id == x.Id)).ToList();
            taskBuffer = taskBuffer.Concat(objs).OrderBy(x => x.Id).ToList();
            taskFileManager.Write(objs, true);
        }

        public void Delete(List<int> ids)
        {
            taskBuffer = taskBuffer.Where(x => !ids.Contains(x.Id)).OrderBy(x => x.Id).ToList();
            taskFileManager.Delete(ids);
        }

        public void Backup(bool restore = false)
        {
            taskFileManager.Backup(restore);
        }
    
        public void Undo()
        {
            Backup(true);
        }
    }
}