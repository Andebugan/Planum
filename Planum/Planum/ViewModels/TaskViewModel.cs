using Planum.Models.BuisnessLogic.Managers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Diagnostics;
using Serilog;

namespace Planum.ViewModels
{
    public class TaskViewModel : ViewModelBase
    {
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        protected bool _isArchived;
        public bool IsArchived
        {
            get => _isArchived;
            set => this.RaiseAndSetIfChanged(ref _isArchived, value);
        }

        protected bool _isUnarchived;
        public bool IsUnarchived
        {
            get => _isUnarchived;
            set => this.RaiseAndSetIfChanged(ref _isUnarchived, value);
        }

        TaskClickHandler _taskClickHandler;

        private TaskViewDTO _taskViewDTO;
        public TaskViewDTO TaskViewDTO
        {
            get => _taskViewDTO;
            set => this.RaiseAndSetIfChanged(ref _taskViewDTO, value);
        }

        public TaskViewModel(TaskViewDTO taskViewDTO, ITaskManager taskManager, ITagManager tagManager, ref TaskClickHandler taskClickHandler, bool showArchived)
        {
            TaskViewDTO = taskViewDTO;
            _taskManager = taskManager;
            _tagManager = tagManager;
            _taskClickHandler = taskClickHandler;
            IsArchived = !showArchived;
            IsUnarchived = showArchived;
        }

        public void OnEditTaskBtnClick()
        {
            Log.Information("Edit task button clicked");
            bool ErrorPopupOpen = false;
            string ErrorText = "";
            
            string name = TaskViewDTO.Name;
            if (string.IsNullOrEmpty(name))
            {
                ErrorPopupOpen = true;  
                ErrorText = "Task name can't be null or empty";
                _taskClickHandler.Invoke(ErrorPopupOpen, ErrorText);
                return;
            }

            string? description = TaskViewDTO.Description;

            string input = TaskViewDTO.TagIds;
            List<int> tagIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
                tagIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();
            foreach (int tagId in tagIds)
            {
                if (_tagManager.FindTag(tagId) == null)
                {
                    ErrorPopupOpen = true;
                    ErrorText = $"Tag with id {tagId} does not exist";
                    _taskClickHandler.Invoke(ErrorPopupOpen, ErrorText);
                    return;
                }
            }

            input = TaskViewDTO.ParentIds;
            List<int> parentIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
                parentIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();
            foreach (int parentId in parentIds)
            {
                if (_taskManager.FindTask(parentId) == null)
                {
                    ErrorPopupOpen = true;
                    ErrorText = $"Task with id {parentId} does not exist";
                    _taskClickHandler.Invoke(ErrorPopupOpen, ErrorText);
                    return;
                }
            }

            input = TaskViewDTO.ChildIds;
            List<int> childIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
                childIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();

            foreach (int childId in childIds)
            {
                if (_taskManager.FindTask(childId) == null)
                {
                    ErrorPopupOpen = true;
                    ErrorText = $"Task with id {childId} does not exist";
                    _taskClickHandler.Invoke(ErrorPopupOpen, ErrorText);
                    return;
                }
            }

            if (!TaskViewDTO.Timed)
            {
                _taskManager.UpdateTask(TaskViewDTO.Id, DateTime.MinValue, DateTime.MinValue, TimeSpan.Zero, tagIds, parentIds, childIds, name,
                    description: description);
                _taskClickHandler.Invoke(ErrorPopupOpen, ErrorText);
                return;
            }

            input = TaskViewDTO.StartTime;
            DateTime startTime;
            if (string.IsNullOrEmpty(input))
                startTime = DateTime.MinValue;
            else if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
            {
                ErrorPopupOpen = true;
                ErrorText = $"Incorrect start time format";
                _taskClickHandler.Invoke(ErrorPopupOpen, ErrorText);
                return;
            }

            input = TaskViewDTO.Deadline;
            DateTime deadline;
            if (string.IsNullOrEmpty(input))
                deadline = DateTime.MinValue;
            else if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out deadline))
            {
                ErrorPopupOpen = true;
                ErrorText = $"Incorrect deadline format";
                _taskClickHandler.Invoke(ErrorPopupOpen, ErrorText);
                return;
            }

            if (!TaskViewDTO.IsRepeated)
            {
                _taskManager.UpdateTask(TaskViewDTO.Id, startTime, deadline, TimeSpan.Zero, tagIds, parentIds, childIds, name,
                    description: description, timed: true);
                _taskClickHandler.Invoke(ErrorPopupOpen, ErrorText);
                return;
            }

            input = TaskViewDTO.RepeatPeriod;
            TimeSpan repeatPeriod;
            if (string.IsNullOrEmpty(input))
                repeatPeriod = TimeSpan.Zero;
            else if (!TimeSpan.TryParseExact(input, @"d\:hh\:mm", CultureInfo.InvariantCulture, TimeSpanStyles.None, out repeatPeriod))
            {
                ErrorPopupOpen = true;
                ErrorText = $"Incorrect repeat period format";
                _taskClickHandler.Invoke(ErrorPopupOpen, ErrorText);
                return;
            }

            _taskManager.UpdateTask(TaskViewDTO.Id, startTime, deadline, repeatPeriod, tagIds, parentIds, childIds, name,
                description: description, timed: true, isRepeated: true);
            _taskClickHandler.Invoke(ErrorPopupOpen, ErrorText);
        }

        public void OnDeleteTaskBtnClick()
        {
            Log.Information("Delete task button clicked");
            _taskManager.DeleteTask(TaskViewDTO.Id);
            _taskClickHandler.Invoke(false, "");
        }

        public void OnArchiveTaskBtnClick()
        {
            Log.Information("Archive task button clicked");
            _taskManager.ArchiveTask(TaskViewDTO.Id);
            _taskClickHandler.Invoke(false, "");
        }

        public void OnUnarchiveTaskBtnClick()
        {
            Log.Information("Unarchive task button clicked");
            _taskManager.UnarchiveTask(TaskViewDTO.Id);
            _taskClickHandler.Invoke(false, "");
        }
    }
}
