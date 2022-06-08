using Planum.Models.BuisnessLogic.Entities;
using Task = Planum.Models.BuisnessLogic.Entities.Task;

namespace Planum.Models.BuisnessLogic.Managers
{
    public interface ITaskManager
    {
        void ArchiveTask(int taskId);
        int CreateTask(DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, bool timed = false,
            string description = "", bool isRepeated = false);
        void DeleteConnectedToUser(int userId);
        void DeleteTask(int taskId);
        Task GetTask(int taskId, bool? archived = false);
        Task? FindTask(int taskId, bool? archived = false);
        List<Task> GetAllTasks(bool? archived = false);
        void UnarchiveTask(int taskId);
        void UpdateTask(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, bool timed = false, string description = "", bool isRepeated = false);
        void AddTagToTask(int taskId, int tagId);
        void RemoveTagFromTask(int taskId, int tagId);
        void RemoveTagFromAll(int tagId);
        void ClearTags(int taskId);
        void AddChildToTask(int taskId, int childId);
        void RemoveChildFromTask(int taskId, int childId);
        void ClearChildren(int taskId);
        void AddParentToTask(int taskId, int parentId);
        void RemoveParentFromTask(int taskId, int parentId);
        void ClearParents(int taskId);
    }
}