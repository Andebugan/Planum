using System.Collections.Generic;
using Planum.Models.DTO;

namespace Planum.Models.BuisnessLogic.IRepo
{
    public interface ITaskRepo
    {
        public int AddTask(TaskDTO taskDTO); 
        public void UpdateTask(TaskDTO taskDTO);
        public void DeleteTask(int id);
        public void ArchiveTask(int id);
        public void UnarchiveTask(int id);
        public TaskDTO GetTask(int id);
        public TaskDTO GetTask(int id, int userId);
        public TaskDTO GetArchivedTask(int id);
        public TaskDTO GetArchivedTask(int id, int userId);
        public TaskDTO? FindTask(int id);
        public TaskDTO? FindTask(int id, int userId);
        public TaskDTO? FindArchivedTask(int id);
        public TaskDTO? FindArchivedTask(int id, int userId);
        public List<TaskDTO> GetAllTasks();
        public List<TaskDTO> GetAllArchivedTasks();
    }
}
