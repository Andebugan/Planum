using Planum.Models.BuisnessLogic.Managers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Diagnostics;
using Serilog;
using Planum.Models.BuisnessLogic.Entities;

namespace Planum.ViewModels
{
    public class TaskViewModel : ViewModelBase
    {
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;
        protected ITaskViewDTOConverter _taskViewDTOConverter;

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

        public TaskViewModel(TaskViewDTO taskViewDTO, ITaskManager taskManager, ITagManager tagManager,
            ITaskViewDTOConverter taskViewDTOConverter, ref TaskClickHandler taskClickHandler, bool showArchived)
        {
            TaskViewDTO = taskViewDTO;
            _taskManager = taskManager;
            _tagManager = tagManager;
            _taskClickHandler = taskClickHandler;
            IsArchived = !showArchived;
            IsUnarchived = showArchived;
            _taskViewDTOConverter = taskViewDTOConverter;
        }

        public void OnEditTaskBtnClick()
        {
            Log.Information("Edit task button clicked");

            bool ErrorPopupOpen = false;
            string ErrorText = "";
            Task? task = _taskViewDTOConverter.ConvertFromViewDTO(TaskViewDTO, _taskManager, _tagManager,
                ref ErrorPopupOpen, ref ErrorText);
            if (task != null)
                _taskManager.UpdateTask(task);
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

        public void OnNextStatusBtnClick()
        {
            Log.Information("Next status button clicked");
            _taskManager.NextStatus(TaskViewDTO.Id);
            _taskClickHandler.Invoke(false, "");
        }

        public void OnPreviousStatusBtnClick()
        {
            Log.Information("Previous status button clicked");
            _taskManager.PreviousStatus(TaskViewDTO.Id);
            _taskClickHandler.Invoke(false, "");
        }
    }
}
