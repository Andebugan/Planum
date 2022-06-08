using Planum.Models.DTO;
using System.Linq;
using Planum.Models.BuisnessLogic.Entities;
using Task = Planum.Models.BuisnessLogic.Entities.Task;

namespace Planum.Models.BuisnessLogic.Managers
{
    public class TaskConverter : ITaskConverter
    {
        public Task ConvertFromDTO(TaskDTO taskDTO)
        {
            Task temp = new Task(taskDTO.Id, taskDTO.StartTime, taskDTO.Deadline, taskDTO.RepeatPeriod,
                taskDTO.TagIds.ToList<int>(), taskDTO.ParentIds.ToList<int>(), taskDTO.ChildIds.ToList<int>(),
                taskDTO.Name, taskDTO.Timed, taskDTO.UserId, taskDTO.Description, taskDTO.IsRepeated);
            temp.Archived = taskDTO.Archived;
            return temp;
        }

        public TaskDTO ConvertToDTO(Task task)
        {
            TaskDTO temp = new TaskDTO(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod,
                task.TagIds.ToList<int>(), task.ParentIds.ToList<int>(),
                task.ChildIds.ToList<int>(), task.Name, task.Timed, task.UserId, task.Description, task.IsRepeated);
            temp.Archived = task.Archived;
            return temp;
        }
    }
}
