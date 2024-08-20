using System;
using System.Collections.Generic;
using Planum.Model.Entities;
using Planum.Model.Managers;

namespace Planum.Commands
{
    public class CreateCommandSettings
    {
        TaskBufferManager TaskBufferManager { get; set; }

        Guid Id = Guid.NewGuid();
        public string Name = "";
        public string Description = "";
        public List<Deadline> Deadlines = new List<Deadline>();
        public List<Guid> Children = new List<Guid>();
        public List<Guid> Parents = new List<Guid>();
        public List<string> Tags = new List<string>();
        public string Filename = "";

        public CreateCommandSettings(TaskBufferManager taskBufferManager) => TaskBufferManager = taskBufferManager;
        public PlanumTask GetPlanumTask() => new PlanumTask(Id, Name, Description, Deadlines, Children, Parents, Tags);
    }
}
