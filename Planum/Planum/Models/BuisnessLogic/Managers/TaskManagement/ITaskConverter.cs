using Planum.Models.DTO;
using Planum.Models.BuisnessLogic.Entities;

namespace Planum.Models.BuisnessLogic.Managers
{
    public interface ITaskConverter
    {
        Task ConvertFromDTO(TaskDTO taskDTO);

        TaskDTO ConvertToDTO(Task task);
    }
}
