using Planum.Models.DTO.ModelData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Planum.Models.BuisnessLayer.Entities;

namespace Planum.Models.BuisnessLayer.Managers.TaskManagement
{
    public class TaskConverter
    {
        protected Task ConvertFromDTO(TaskDTO taskDTO)
        {
            return new Task(taskDTO.Id, taskDTO.StartTime, taskDTO.Deadline, taskDTO.RepeatPeriod,
                taskDTO.TagIds.ToList<int>(), taskDTO.UserId, taskDTO.Name, taskDTO.Description, taskDTO.ParentId, taskDTO.IsRepeated);
        }

        protected TaskDTO ConvertToDTO(Task task)
        {
            return new TaskDTO(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod,
                task.TagIds.ToList<int>(), task.UserId, task.Name, task.Description, task.ParentId, task.IsRepeated);
        }
    }
}
