using System.Collections.Generic;
using Planum.Models.DTO;

namespace Planum.Models.BuisnessLayer.RepoInterfaces
{
    public interface ITaskRepo
    {
        public int AddTask(TaskDTO taskDTO); 
        public void UpdateTask(TaskDTO taskDTO);
        public void DeleteTask(int id);
        public void ArchiveTask(int id);
        public void UnarchiveTask(int id);
        public TaskDTO GetTask(int id);
        public TaskDTO? FindTask(int id);
        public List<TaskDTO> GetAllTasks();
        public List<TaskDTO> GetAllArchivedTasks();
    }
}
