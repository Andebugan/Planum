using Planum.Models.BuisnessLayer.Entities;
using System;
using System.Collections.Generic;

namespace Planum.Models.BuisnessLayer.Managers
{
    public interface ITaskManager
    {
        void ArchiveTask(int taskId);
        int CreateTask(int user_id, DateTime startTime, DateTime deadline, TimeSpan repeatPeriod, List<int> tagIds,  bool timed = false, string name = "", string description = "", int parentId = -1, bool isRepeated = false);
        void DeleteConnectedToUser(int userId);
        void DeleteTask(int taskId);
        Task? FindTask(int taskId);
        List<Task> GetAllTasks();
        Task GetTask(int taskId);
        void RemoveTagFromAll(int tagId);
        void UnarchiveTask(int taskId);
        void UpdateTask(int id, DateTime startTime, DateTime deadline, TimeSpan repeatPeriod, IReadOnlyList<int> tagIds,  bool timed = false, string name = "", string description = "", int parentId = -1, bool isRepeated = false);
    }
}