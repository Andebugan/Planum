using Planum.Models.DTO;

namespace Planum.DataModels
{
    public interface ITaskDTOComparator
    {
        public bool CompareDTOs(int firstId, TaskDTO firstDTO, int secondId, TaskDTO secondDTO);
        public bool CompareDTOs(TaskDTO firstDTO, TaskDTO secondDTO);
    }
}
