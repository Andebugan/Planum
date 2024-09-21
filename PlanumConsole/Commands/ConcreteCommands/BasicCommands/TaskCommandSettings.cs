using Planum.Logger;
using Planum.Model.Entities;
using Planum.Model.Filters;
using Planum.Model.Managers;

namespace Planum.Console.Commands.Task
{
    public class TaskCommandSettings: ICommandSettings
    {
        TaskBufferManager TaskBufferManager { get; set; }

        public List<PlanumTask> Tasks { get; set; } = new List<PlanumTask>();

        public bool Commit = false;

        public DeadlineFilter DeadlineFilter { get; set; }
 
        public TaskCommandSettings(TaskBufferManager taskBufferManager, ILoggerWrapper logger)
        {
            TaskBufferManager = taskBufferManager;
            DeadlineFilter = new DeadlineFilter(logger);
        }
    }
}
