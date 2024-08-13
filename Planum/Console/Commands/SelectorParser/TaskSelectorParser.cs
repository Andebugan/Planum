using System.Collections.Generic;
using Planum.Model.Entities;
using Planum.Model.Filters;
using Planum.Model.Managers;

namespace Planum.Commands
{
    public class TaskSelectorParser
    {
        TaskBufferManager BufferManager { get; set; }
        public IEnumerable<IOption> SelectorOptions { get; set; }

        public TaskSelectorParser(TaskBufferManager bufferManager, IEnumerable<IOption> selectorOptions)
        {
            BufferManager = bufferManager;
            SelectorOptions = selectorOptions;
        }

        public bool TryParseSelector(ref IEnumerator<string> args, out IEnumerable<PlanumTask> tasks)
        {
            TaskFilter taskFilter = new TaskFilter();

            tasks = BufferManager.Find(taskFilter);
            return false;
        }
    }
}
