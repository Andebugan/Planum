using Planum.Models.BuisnessLogic.Entities;
using System;
using System.Collections.Generic;

namespace Planum.Models.BuisnessLogic.Managers
{
    public interface ITaskManager
    {
        void ArchiveTask(int taskId);
        int CreateTask(DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, int userId, bool timed = false,
            string description = "", bool isRepeated = false);
        void DeleteConnectedToUser(int userId);
        void DeleteTask(int taskId);
        void DeleteTask(int taskId, int userId);
        Task? FindTask(int taskId);
        Task? FindTask(int taskId, int userId);
        Task? FindArchivedTask(int taskId);
        Task? FindArchivedTask(int taskId, int userId);
        List<Task> GetAllTasks();
        List<Task> GetAllTasks(int userId);
        Task GetTask(int taskId);
        Task GetTask(int taskId, int userId);
        Task GetArhcivedTask(int taskId);
        Task GetArhcivedTask(int taskId, int userId);
        void AddTagToTask(int taskId, int tagId);
        void RemoveTagFromTask(int taskId, int tagId);
        void RemoveTagFromTask(int taskId, int tagId, int userId);
        void RemoveTagFromAll(int tagId);
        void RemoveTagFromAll(int tagId, int userId);
        void UnarchiveTask(int taskId);
        void UnarchiveTask(int taskId, int userId);
        void UpdateTask(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, bool timed = false, string description = "", bool isRepeated = false);
        void UpdateTask(int id, int userId, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, bool timed = false, string description = "", bool isRepeated = false);
    }
}