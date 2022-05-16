using Planum.Models.DTO.ModelData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.BuisnessLayer.Managers
{
    public interface ITaskManager
    {
        Task ConvertFromDTO(TaskDTO taskDTO);

        TaskDTO ConvertToDTO(Task task);

        void CreateTask(int user_id, string? name, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, List<int> tagIds, string? description = null, int parentId = -1, bool isRepeated = false);

        void UpdateTask(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, List<int> tagIds, string? name, string? description = null,
            int parentId = -1, bool isRepeated = false);

        void DeleteTask(int taskId);

        void ArchiveTask(int taskId);

        void UnarchiveTask(int taskId);

        Task GetTask(int taskId);

        void RemoveTagFromAll(int tagId);

        void DeleteConnectedToUser(int userId);

        List<Task> GetAll();
    }
}
