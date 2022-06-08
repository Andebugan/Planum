using Planum.Models.DTO;
using Planum.Models.BuisnessLogic.Entities;
using Task = Planum.Models.BuisnessLogic.Entities.Task;

namespace Planum.Models.BuisnessLogic.Managers
{
    public interface ITaskConverter
    {
        Task ConvertFromDTO(TaskDTO taskDTO);

        TaskDTO ConvertToDTO(Task task);
    }
}
