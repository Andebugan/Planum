using System;
using System.Collections.Generic;
using Planum.Model.Entities;
using Planum.Model.Managers;

namespace Planum.Console.Commands.Task
{
    public class TaskCommandSettings: ICommandSettings
    {
        TaskBufferManager TaskBufferManager { get; set; }

        public Deadline? CurrentDeadline { get; set; } = null;
        
        public Guid Id = Guid.Empty;
        public string Name = "";
        public string Description = "";
        public List<Deadline> Deadlines = new List<Deadline>();
        public List<Guid> Children = new List<Guid>();
        public List<Guid> Parents = new List<Guid>();
        public List<string> Tags = new List<string>();
        public string Filename = "";
        public bool Commit = false;

        public TaskCommandSettings(TaskBufferManager taskBufferManager) => TaskBufferManager = taskBufferManager;

        public PlanumTask GetPlanumTask()
        {
            CommitDeadline();
            return new PlanumTask(Id, Name, Description, Deadlines, Children, Parents, Tags);
        }

        public void CommitDeadline()
        {
            if (CurrentDeadline != null)
                Deadlines.Add(CurrentDeadline);
            CurrentDeadline = null;
        }
    }
}
