using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;

namespace Planum.ViewModels
{
    public interface ITaskViewDTOConverter
    {
        Task? ConvertFromViewDTO(TaskViewDTO taskViewDTO, ITaskManager _taskManager,
             ITagManager _tagManager, ref bool ErrorPopupOpen, ref string ErrorText);

        TaskViewDTO ConvertToViewDTO(Task task);
    }
}
