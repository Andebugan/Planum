using Planum.Models.DTO;
using System.Linq;
using Planum.Models.BuisnessLayer.Entities;

namespace Planum.Models.BuisnessLayer.Managers.TaskManagement
{
    public class TaskConverter
    {
        protected Task ConvertFromDTO(TaskDTO taskDTO)
        {
            return new Task(taskDTO.Id, taskDTO.StartTime, taskDTO.Deadline, taskDTO.RepeatPeriod,
                taskDTO.TagIds.ToList<int>(), taskDTO.ParentIds.ToList<int>(), taskDTO.ChildIds.ToList<int>(),
                taskDTO.Name, taskDTO.Timed, taskDTO.UserId, taskDTO.Description, taskDTO.IsRepeated);
        }

        protected TaskDTO ConvertToDTO(Task task)
        {
            return new TaskDTO(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod,
                task.TagIds.ToList<int>(), task.ParentIds.ToList<int>(),
                task.ChildIds.ToList<int>(), task.Name, task.Timed, task.UserId, task.Description, task.IsRepeated);
        }
    }
}
