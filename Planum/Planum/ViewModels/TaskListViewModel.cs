using Planum.Models.BuisnessLogic.Managers;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Planum.Models.BuisnessLogic.Entities;
using System.Linq;
using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia.Interactivity;
using Serilog;

namespace Planum.ViewModels
{
    public delegate void TaskClickHandler(bool isError, string errorText);
    public delegate void TagClickHandler(bool isError, string errorText);

    public class TaskListViewModel : ViewModelBase
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public event TaskClickHandler OnBtnClick;
        public event TagClickHandler OnTagClick;

        public TaskListViewModel(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
            OnBtnClick += TaskClick;
            OnTagClick += TagClick;
        }
        // List view

        private ObservableCollection<TaskViewModel> _taskList;
        public ObservableCollection<TaskViewModel> TaskList
        {
            get => _taskList;
            set => this.RaiseAndSetIfChanged(ref _taskList, value);
        }

        private ObservableCollection<TagViewModel> _tagList;
        public ObservableCollection<TagViewModel> TagList
        {
            get => _tagList;
            set => this.RaiseAndSetIfChanged(ref _tagList, value);
        }

        public void LoadTasks()
        {
            List<Task> tasks = _taskManager.GetAllTasks(ShowArchived);
            TaskList = new ObservableCollection<TaskViewModel>();
            foreach (Task task in tasks)
            {
                TaskViewDTO temp = new TaskViewDTO(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.UserId, task.Description, task.IsRepeated);
                TaskList.Add(new TaskViewModel(temp, _taskManager, _tagManager, ref OnBtnClick, ShowArchived));
            }
        }

        private string _errorText;
        public string ErrorText
        {
            get => _errorText;
            set => this.RaiseAndSetIfChanged(ref _errorText, value);
        }

        private bool _errorPopupOpen;
        public bool ErrorPopupOpen
        {
            get => _errorPopupOpen;
            set => this.RaiseAndSetIfChanged(ref _errorPopupOpen, value);
        }

        public void OnCloseErrorPopupBtnClick()
        {
            Log.Information("Close error popup button clicked");
            ErrorPopupOpen = false;
        }

        public void OnAddTaskBtnClick()
        {
            Log.Information("Add task button clicked");
            string name = "new task";
            List<int> tagIds = new List<int>();
            List<int> parentIds = new List<int>();
            List<int> childIds = new List<int>();

            _taskManager.CreateTask(DateTime.MinValue, DateTime.MinValue, TimeSpan.Zero, tagIds, parentIds, childIds, name);
            LoadTasks();
        }

        public void OnUnarchiveTaskBtnClick(TaskViewDTO taskViewDTO)
        {
            Log.Information("Unarchive task button clicked");
            _taskManager.UnarchiveTask(taskViewDTO.Id);
            LoadTasks();
        }

        private bool _showArchived = false;
        public bool ShowArchived
        {
            get => _showArchived;
            set
            {
                this.RaiseAndSetIfChanged(ref _showArchived, value);
            }
        }

        public void OnShowArchivedBtnClick()
        {
            Log.Information("Show archived tasks button clicked");
            LoadTasks();
        }

        public void OnAddTagBtnClick()
        {
            Log.Information("Add tag button clicked");
            string name = "tag name";
            _tagManager.CreateTag(-1, name, "");
            LoadTags();
        }

        public void LoadTags()
        {
            List<Tag> tags = _tagManager.GetAllTags();
            TagList = new ObservableCollection<TagViewModel>();
            foreach (Tag tag in tags)
            {
                TagList.Add(new TagViewModel(_tagManager, new TagViewDTO(tag.Id, tag.UserId, tag.Category, tag.Name, tag.Description),
                    OnTagClick));
            }
        }

        public void TaskClick(bool isError, string errorText)
        {
            ErrorPopupOpen = isError;
            ErrorText = errorText;
            LoadTasks();
        }

        public void TagClick(bool isError, string errorText)
        {
            ErrorPopupOpen = isError;
            ErrorText = errorText;
            LoadTags();
            LoadTasks();
        }
    }
}
