using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Text;

// Calendar
// leveled display - for day/week/month/year
// range parameters, from to
// calculate for displayed tasks ranges of ranges from start time/today to deadline ---- and from deadline to today ===
// today mark as arrow

namespace Planum.ConsoleUI
{
    /*
     * day view
     * week view
     * month view - done
     * year view
     */
    public class TaskCalendarView
    {
        public void RenderTasks(List<Task> tasks,
            ITagManager tagManager, ITaskManager taskManager, Dictionary<string, bool> boolParams,
            Dictionary<string, string> stringParams)
        {
            /*
             *  { "showTodayTasks", false },
                { "showOverdueTasks", false },
                { "showNoParent", false },
                { "showNoChildren", false },
                { "showNoTags", false },
                { "showNoStatuses", false },
                { "showCurrentTasks", false },
                { "showNotCurrentTasks", false },
                filter those here
             */
            TaskMonthView taskMonthView = new TaskMonthView(tasks);
            taskMonthView.RenderTasks(new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day));
            taskMonthView.RenderTasks(DateTime.Now);
            taskMonthView.RenderTasks(new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, DateTime.Now.Day));
        }
    }
}
