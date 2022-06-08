using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;

namespace Planum.ViewModels
{
    public class TaskViewDTOConverter : ITaskViewDTOConverter
    {
        public Task? ConvertFromViewDTO(TaskViewDTO taskViewDTO, ITaskManager _taskManager,
             ITagManager _tagManager, ref bool ErrorPopupOpen, ref string ErrorText)
        {
            string name = taskViewDTO.Name;
            if (string.IsNullOrEmpty(name))
            {
                ErrorPopupOpen = true;
                ErrorText = "Task name can't be null or empty";
                return null;
            }

            string? description = taskViewDTO.Description;

            string input = taskViewDTO.TagIds;
            List<int> tagIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    tagIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();
                }
                catch (Exception ex)
                {
                    ErrorPopupOpen = true;
                    ErrorText = $"Incorrect tag Ids format";
                    return null;
                }
            }
            foreach (int tagId in tagIds)
            {
                if (_tagManager.FindTag(tagId) == null)
                {
                    ErrorPopupOpen = true;
                    ErrorText = $"Tag with id {tagId} does not exist";
                    return null;
                }
            }

            input = taskViewDTO.ParentIds;
            List<int> parentIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    parentIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();
                }
                catch (Exception ex)
                {
                    ErrorPopupOpen = true;
                    ErrorText = $"Incorrect parent Ids format";
                    return null;
                }
            }

            foreach (int parentId in parentIds)
            {
                if (_taskManager.FindTask(parentId) == null)
                {
                    ErrorPopupOpen = true;
                    ErrorText = $"Task with id {parentId} does not exist";
                    return null;
                }
            }

            input = taskViewDTO.ChildIds;
            List<int> childIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    childIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();
                }
                catch (Exception ex)
                {
                    ErrorPopupOpen = true;
                    ErrorText = $"Incorrect child Ids format";
                    return null;
                }
            }

            foreach (int childId in childIds)
            {
                if (_taskManager.FindTask(childId) == null)
                {
                    ErrorPopupOpen = true;
                    ErrorText = $"Task with id {childId} does not exist";
                    return null;
                }
            }

            input = taskViewDTO.StatusQueueIds;
            List<int> statusQueueIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    statusQueueIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();
                }
                catch (Exception ex)
                {
                    ErrorPopupOpen = true;
                    ErrorText = $"Incorrect status queue format";
                    return null;
                }
            }

            foreach (int tagId in statusQueueIds)
            {
                if (_tagManager.FindTag(tagId) == null)
                {
                    ErrorPopupOpen = true;
                    ErrorText = $"Tag with id {tagId} does not exist";
                    return null;
                }
            }

            int currentStatusIndex = 0;
            if (statusQueueIds.Count > 0)
            {
                currentStatusIndex = taskViewDTO.CurrentStatusIndex;
                if (currentStatusIndex < 0 || currentStatusIndex >= statusQueueIds.Count)
                {
                    ErrorPopupOpen = true;
                    ErrorText = $"Current status index incorrect";
                    return null;
                }
            }

            bool archived = taskViewDTO.Archived;

            if (!taskViewDTO.Timed)
            {
                Task task = new Task(taskViewDTO.Id, DateTime.Now, DateTime.Now, TimeSpan.Zero, tagIds, parentIds, childIds, name, taskViewDTO.Timed,
                    taskViewDTO.UserId, description, StatusQueueIds: statusQueueIds);
                task.SetStatusIndex(currentStatusIndex);
                return task;
            }

            input = taskViewDTO.StartTime;
            DateTime startTime;
            if (string.IsNullOrEmpty(input))
                startTime = DateTime.MinValue;
            else if (!DateTime.TryParseExact(input.Replace(".", "/"), "dd/MM/yyyy HH:mm:ss", null, DateTimeStyles.None, out startTime))
            {
                ErrorPopupOpen = true;
                ErrorText = $"Incorrect start time format";
                return null;
            }

            input = taskViewDTO.Deadline;
            DateTime deadline;
            if (string.IsNullOrEmpty(input))
                deadline = DateTime.MinValue;
            else if (!DateTime.TryParseExact(input.Replace(".", "/"), "dd/MM/yyyy HH:mm:ss", null, DateTimeStyles.None, out deadline))
            {
                ErrorPopupOpen = true;
                ErrorText = $"Incorrect deadline format";
                return null;
            }

            if (!taskViewDTO.IsRepeated)
            {
                Task task = new Task(taskViewDTO.Id, startTime, deadline, TimeSpan.Zero, tagIds, parentIds, childIds, name, taskViewDTO.Timed,
                    taskViewDTO.UserId, description, taskViewDTO.IsRepeated, archived, statusQueueIds);
                task.SetStatusIndex(currentStatusIndex);
                return task;
            }

            input = taskViewDTO.RepeatPeriod;
            TimeSpan repeatPeriod;
            if (string.IsNullOrEmpty(input))
                repeatPeriod = TimeSpan.Zero;
            else if (!TimeSpan.TryParseExact(input, @"d\:hh\:mm", CultureInfo.InvariantCulture, TimeSpanStyles.None, out repeatPeriod))
            {
                ErrorPopupOpen = true;
                ErrorText = $"Incorrect repeat period format";
                return null;
            }

            Task newTask = new Task(taskViewDTO.Id, startTime, deadline, repeatPeriod, tagIds, parentIds, childIds, name, taskViewDTO.Timed,
                    taskViewDTO.UserId, description, taskViewDTO.IsRepeated, archived, statusQueueIds);
            newTask.SetStatusIndex(currentStatusIndex);
            return newTask;
        }

        public TaskViewDTO ConvertToViewDTO(Task task)
        {
            TaskViewDTO temp = new TaskViewDTO(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod,
                task.TagIds.ToList<int>(), task.ParentIds.ToList<int>(),
                task.ChildIds.ToList<int>(), task.Name, task.Timed, task.UserId, task.Description, task.IsRepeated,
                task.Archived, task.StatusQueueIds, task.CurrentStatusIndex);
            return temp;
        }
    }
}
