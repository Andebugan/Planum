using System.Collections.Generic;
using Planum.Model.Entities;
using Planum.Model.Filters;
using Planum.Model.Managers;

namespace Planum.Commands.Selector
{
    public class SelectorParser
    {
        TaskBufferManager BufferManager { get; set; }
        public IEnumerable<IOption<TaskFilter>> SelectorOptions { get; set; }

        public SelectorParser(TaskBufferManager bufferManager, IEnumerable<IOption<TaskFilter>> selectorOptions)
        {
            BufferManager = bufferManager;
            SelectorOptions = selectorOptions;
        }

        public bool TryParseSelector(ref IEnumerator<string> argsEnumerator, out IEnumerable<PlanumTask> tasks)
        {
            TaskFilter taskFilter = new TaskFilter();

            bool optionParsed = false;
            while (optionParsed)
            {
                foreach (var option in SelectorOptions)
                {
                    optionParsed = option.TryParseValue(ref argsEnumerator, ref taskFilter);
                    if (optionParsed)
                        break;
                }
            }

            tasks = BufferManager.Find(taskFilter);
            return false;
        }
    }
}
