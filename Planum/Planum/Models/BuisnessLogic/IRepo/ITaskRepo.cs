using System.Collections.Generic;
using Planum.Models.DTO;

namespace Planum.Models.BuisnessLogic.IRepo
{
    public interface ITaskRepo
    {
        public int AddTask(TaskDTO taskDTO); 
        public void UpdateTask(TaskDTO taskDTO);
        public void DeleteTask(int id);
        public TaskDTO GetTask(int id);
        public TaskDTO? FindTask(int id);
        public List<TaskDTO> GetAllTasks();
    }
}
