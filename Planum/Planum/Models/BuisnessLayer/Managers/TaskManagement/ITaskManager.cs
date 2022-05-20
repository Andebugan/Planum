using Planum.Models.BuisnessLayer.Entities;
using System;
using System.Collections.Generic;

namespace Planum.Models.BuisnessLayer.Managers
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
        Task? FindTask(int taskId);
        Task? FindArchivedTask(int taskId);
        List<Task> GetAllTasks();
        Task GetTask(int taskId);
        Task GetArhcivedTask(int taskId);
        void RemoveTagFromTask(int taskId, int tagId);
        void RemoveTagFromAll(int tagId);
        void UnarchiveTask(int taskId);
        void UpdateTask(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, bool timed = false, string description = "", bool isRepeated = false);
    }
}