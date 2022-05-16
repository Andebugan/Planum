using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Planum.Models.DTO.ModelData;
using Planum.Models.BuisnessLayer.Entities;

namespace Planum.Models.BuisnessLayer.Managers.TaskManagement
{
    public interface ITaskConverter
    {
        Task ConvertFromDTO(TaskDTO taskDTO);

        TaskDTO ConvertToDTO(Task task);
    }
}
