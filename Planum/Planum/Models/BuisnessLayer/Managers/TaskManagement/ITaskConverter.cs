using Planum.Models.DTO;
using Planum.Models.BuisnessLayer.Entities;

namespace Planum.Models.BuisnessLayer.Managers.TaskManagement
{
    public interface ITaskConverter
    {
        Task ConvertFromDTO(TaskDTO taskDTO);

        TaskDTO ConvertToDTO(Task task);
    }
}
