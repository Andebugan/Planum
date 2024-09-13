using System.Collections.Generic;
using Planum.Model.Entities;
using Planum.Model.Filters;
using Planum.Model.Managers;

namespace Planum.Console.Commands.Task
{
    public class TaskCommandSettings: ICommandSettings
    {
        TaskBufferManager TaskBufferManager { get; set; }

        public List<PlanumTask> Tasks { get; set; } = new List<PlanumTask>();

        public string Filename = "";
        public bool Commit = false;

        public DeadlineFilter DeadlineFilter { get; set; } = new DeadlineFilter(); // for targeted deadline changes 
 
        public TaskCommandSettings(TaskBufferManager taskBufferManager) => TaskBufferManager = taskBufferManager;
    }
}
