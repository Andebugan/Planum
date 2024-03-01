using Planum.ConsoleUI.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Planum.Model.Entities
{
    public class Checklist
    {
        public class Item
        {
            public string description = "";
            public bool done = false;

            public Item(string description, bool done = false)
            {
                this.description = description;
                this.done = done;
            }

            public override string ToString()
            {
                string result = "";
                if (done)
                    result += "v ";
                else
                    result += "x ";
                result += description;
                return result;
            }
        }

        public List<Item> items = new List<Item>();

        public bool ResetOnComplete { get; set; } = false;

        public Checklist()
        {
        }

        public Checklist(Checklist checklist)
        {
            foreach (var item in checklist.items)
                AddItem(item.description, item.done);
        }

        public void AddItem(string description, bool done = false)
        {
            if (!items.Exists(x => x.description == description && x.done == done))
                items.Add(new Item(description, done));
        }

        public void RemoveItem(int id)
        {
            items.RemoveAt(id);
        }

        public void CompleteItem(int id)
        {
            if (id >= 0 && id < items.Count)
                items[id].done = true;
        }

        public void CompleteItem(List<int> ids)
        {
            foreach (int id in ids)
                CompleteItem(id);
        }

        public void Reset()
        {
            foreach (var item in items)
            {
                item.done = false;
            }
        }

        public string ToString(bool sortByDone = false)
        {
            string result = "";
            if (sortByDone)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (!items[i].done)
                    {
                        if (items[i].done)
                            result += "v ";
                        else
                            result += "x ";
                        result += "[" + i + "] ";
                        result += items[i].description;
                        if (i != items.Count - 1)
                            result += "\n";
                    }
                }

                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].done)
                    {
                        if (items[i].done)
                            result += "v ";
                        else
                            result += "x ";
                        result += "[" + i + "] ";
                        result += items[i].description;
                        if (i != items.Count - 1)
                            result += "\n";
                    }
                }
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].done)
                        result += "v ";
                    else
                        result += "x ";
                    result += "[" + i + "] ";
                    result += items[i].description;
                    if (i != items.Count - 1)
                        result += "\n";
                }
            }
            return result;
        }
    }

    public class RepeatParams
    {
        public bool enabled = false;
        public bool autorepeat = false;

        public int years = 0;
        public int months = 0;
        public TimeSpan custom = TimeSpan.Zero;

        public static bool operator ==(RepeatParams? paramsOld, RepeatParams? paramsNew)
        {
            if (paramsOld is null && paramsNew is null)
                return true;
            if (paramsOld is null)
                return false;
            if (paramsNew is null)
                return false;
            if (paramsOld.enabled == paramsNew.enabled &&
                paramsOld.autorepeat == paramsNew.autorepeat &&
                paramsOld.years == paramsNew.years && 
                paramsOld.months == paramsNew.months &&
                paramsOld.custom == paramsNew.custom)
                return true;
            return false;
        }

        public static bool operator !=(RepeatParams? paramsOld, RepeatParams? paramsNew)
        {
            if (!(paramsOld == paramsNew))
                return true;
            return false;
        }

        public RepeatParams() { }

        public RepeatParams(RepeatParams repeatParams)
        {
            enabled = repeatParams.enabled;
            autorepeat = repeatParams.autorepeat;
            years = repeatParams.years;
            months = repeatParams.months;
            custom = repeatParams.custom;
        }

        public RepeatParams(bool enabled = false, bool autorepeat = false, int years = 0, int months = 0, TimeSpan? custom = null)
        {
            this.enabled = enabled;
            this.autorepeat = autorepeat;
            this.years = years;
            this.months = months;
            if (custom is null)
                this.custom = TimeSpan.Zero;
            else
                this.custom = (TimeSpan)custom;
        }

        public override bool Equals(object? obj)
        {
            return obj is RepeatParams @params &&
                   enabled == @params.enabled &&
                   autorepeat == @params.autorepeat &&
                   years == @params.years &&
                   months == @params.months &&
                   custom == @params.custom;
        }

        public string GetRepeatPeriod()
        {
            string repeatPeriod = "";
            repeatPeriod += years.ToString() + " ";
            repeatPeriod += months.ToString() + " ";
            repeatPeriod += custom.ToString(ArgumentParser.TimeSpanFormat);
            return repeatPeriod;
        }
    }

    public class TimeParams
    {
        public bool enabled = false;
        public DateTime Start { get; set; }
        public DateTime Deadline { get; set; }
        public RepeatParams repeat { get; set; }

        public static bool operator ==(TimeParams? paramsOld, TimeParams? paramsNew)
        {
            if (paramsOld is null && paramsNew is null)
                return true;
            if (paramsOld is null)
                return false;
            if (paramsNew is null)
                return false;
            if (paramsOld.enabled == paramsNew.enabled &&
                paramsOld.Start == paramsNew.Start && 
                paramsOld.Deadline == paramsNew.Deadline &&
                paramsOld.repeat == paramsNew.repeat)
                return true;
            return false;
        }
        public static bool operator !=(TimeParams? paramsOld, TimeParams? paramsNew)
        {
            if (!(paramsOld == paramsNew))
                return true;
            return false;
        }

        public TimeParams(TimeParams timeParams)
        {
            enabled = timeParams.enabled;
            Start = timeParams.Start;
            Deadline = timeParams.Deadline;
            repeat = new RepeatParams(timeParams.repeat);
        }

        public TimeParams() { repeat = new RepeatParams(); }

        public TimeParams(bool enabled = false, DateTime? start = null, DateTime? deadline = null, RepeatParams? repeat = null)
        {
            this.enabled = enabled;
            if (start != null)
                Start = (DateTime)start;
            else
                Start = DateTime.MinValue;
            if (deadline != null)
                Deadline = (DateTime)deadline;
            else
                Deadline = DateTime.MaxValue;
            if (repeat != null)
                this.repeat = repeat;
            else
                this.repeat = new RepeatParams(enabled);
        }

        public override bool Equals(object? obj)
        {
            return obj is TimeParams @params &&
                   enabled == @params.enabled &&
                   Start == @params.Start &&
                   Deadline == @params.Deadline &&
                   repeat == @params.repeat;
        }
    }

    public class TaskTimeComparer : IComparer<Task>
    {
        public int Compare([AllowNull] Task x, [AllowNull] Task y)
        {
            if (x == null || y == null)
                throw new Exception("compared task is null");
            if (y.TimeParams.enabled == false && x.TimeParams.enabled)
                return 1;
            if (x.TimeParams.enabled == false && y.TimeParams.enabled)
                return -1;
            if (!x.TimeParams.enabled && !y.TimeParams.enabled)
                return 0;

            if (x.IsOverdue())
            {
                if (!y.HasStarted())
                    return 1;
                if (y.InProgress())
                    return 1;
                if (y.IsOverdue())
                {
                    if (x.TimeParams.Deadline < y.TimeParams.Deadline)
                        return 1;
                    if (x.TimeParams.Deadline == y.TimeParams.Deadline)
                        return 0;
                    if (x.TimeParams.Deadline > y.TimeParams.Deadline)
                        return -1;
                }
            }

            if (y.IsOverdue())
            {
                if (!x.HasStarted())
                    return -1;
                if (x.InProgress())
                    return -1;
                if (x.IsOverdue())
                {
                    if (y.TimeParams.Deadline < x.TimeParams.Deadline)
                        return -1;
                    if (y.TimeParams.Deadline == x.TimeParams.Deadline)
                        return 0;
                    if (y.TimeParams.Deadline > x.TimeParams.Deadline)
                        return 1;
                }
            }

            if (x.InProgress())
            {
                if (!y.HasStarted())
                    return 1;
                if (y.InProgress())
                {
                    if (x.TimeParams.Deadline > y.TimeParams.Deadline)
                        return -1;
                    if (x.TimeParams.Deadline == y.TimeParams.Deadline)
                        return 0;
                    if (x.TimeParams.Deadline < y.TimeParams.Deadline)
                        return 1;
                }
                if (y.IsOverdue())
                    return -1;
            }

            if (y.InProgress())
            {
                if (!x.HasStarted())
                    return -1;
                if (x.InProgress())
                {
                    if (y.TimeParams.Deadline > x.TimeParams.Deadline)
                        return 1;
                    if (y.TimeParams.Deadline == x.TimeParams.Deadline)
                        return 0;
                    if (y.TimeParams.Deadline < x.TimeParams.Deadline)
                        return -1;
                }
                if (x.IsOverdue())
                    return 1;
            }

            if (!x.HasStarted())
            {
                if (!y.HasStarted())
                {
                    if (x.TimeParams.Start < y.TimeParams.Start)
                        return 1;
                    if (x.TimeParams.Start == y.TimeParams.Start)
                        return 0;
                    if (x.TimeParams.Start > y.TimeParams.Start)
                        return -1;
                }
                if (y.InProgress())
                    return -1;
                if (y.IsOverdue())
                    return -1;
            }

            if (!y.HasStarted())
            {
                if (!x.HasStarted())
                {
                    if (y.TimeParams.Start < x.TimeParams.Start)
                        return -1;
                    if (y.TimeParams.Start == x.TimeParams.Start)
                        return 0;
                    if (y.TimeParams.Start > x.TimeParams.Start)
                        return 1;
                }
                if (x.InProgress())
                    return 1;
                if (x.IsOverdue())
                    return 1;
            }

            return 0;
        }
    }


    public class Task
    {
        public int Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }

        private List<int> parentIds = new List<int>();
        public List<int> ParentIds
        {
            get
            {
                return parentIds;
            }

            set
            {
                if (value.Contains(Id))
                    value.Remove(Id);
                parentIds = value;
            }
        }

        private List<int> childIds = new List<int>();
        public List<int> ChildIds
        {
            get
            {
                return childIds;
            }

            set
            {
                if (value.Contains(Id))
                    value.Remove(Id);
                childIds = value;
            }
        }

        public TimeParams TimeParams { get; set; }

        public bool Archived { get; set; } = false;

        public Checklist checklist = new Checklist();

        public DateTime Start()
        {
            return TimeParams.Start;
        }

        public DateTime Deadline()
        {
            return TimeParams.Deadline;
        }

        public bool HasChildren()
        {
            return ChildIds.Count > 0;
        }

        public bool HasParents()
        {
            return ParentIds.Count > 0;
        }

        public bool Timed()
        {
            return TimeParams.enabled;
        }

        public bool HasStartTime()
        {
            return TimeParams.Start != DateTime.MinValue;
        }

        public bool HasDeadline()
        {
            return TimeParams.Deadline != DateTime.MaxValue;
        }

        public bool Repeated()
        {
            return TimeParams.repeat.enabled;
        }

        public bool Autorepeated()
        {
            return TimeParams.repeat.autorepeat;
        }

        public bool HasStarted()
        {
            return DateTime.Now > TimeParams.Start;
        }

        public bool InProgress()
        {
            return HasStarted() && !IsOverdue();
        }

        public bool IsOverdue()
        {
            return DateTime.Now > TimeParams.Deadline;
        }

        public override bool Equals(object? obj)
        {
            return obj is Task task &&
                   Name == task.Name &&
                   Description == task.Description &&
                   ParentIds.SequenceEqual(task.ParentIds) &&
                   ChildIds.SequenceEqual(task.ChildIds) &&
                   TimeParams == task.TimeParams &&
                   Archived == task.Archived &&
                   checklist.items.SequenceEqual(task.checklist.items);
        }

        public Task(int id, string name, string description = "", List<int>? ParentIds = null, List<int>? ChildIds = null, TimeParams? timeParams = null, bool archived = false, Checklist? checklist = null)
        {
            Id = id;
            Name = name;
            Description = description;

            if (ParentIds != null)
                this.ParentIds = new List<int>(ParentIds);
            else
                this.ParentIds = new List<int>();

            if (ChildIds != null)
                this.ChildIds = new List<int>(ChildIds);
            else
                this.ChildIds = new List<int>();

            if (timeParams != null)
                TimeParams = timeParams;
            else
                TimeParams = new TimeParams();

            Archived = archived;

            if (checklist == null)
                this.checklist = new Checklist();
            else
                this.checklist = checklist;
        }

        public Task(Task task)
        {
            this.Id = task.Id;
            this.Name = task.Name;
            this.Description = task.Description;
            this.ParentIds = new List<int>(task.ParentIds);
            this.ChildIds = new List<int>(task.ChildIds);
            this.TimeParams = new TimeParams(task.TimeParams);
            this.checklist = new Checklist(task.checklist);
            Archived = task.Archived;
        }

        public Task(int id, Task task)
        {
            this.Id = id;
            this.Name = task.Name;
            this.Description = task.Description;
            this.ParentIds = new List<int>(task.ParentIds);
            this.ChildIds = new List<int>(task.ChildIds);
            this.TimeParams = new TimeParams(task.TimeParams);
            this.checklist = new Checklist(task.checklist);
            Archived = task.Archived;
        }

        public static bool operator ==(Task? taskOld, Task? taskNew)
        {
            if (taskOld is null && taskNew is null)
                return true;
            if (taskOld is null)
                return false;
            if (taskNew is null)
                return false;
            if (taskOld.Name == taskNew.Name &&
                taskOld.Description == taskNew.Description &&
                taskOld.ParentIds.SequenceEqual(taskNew.ParentIds) &&
                taskOld.ChildIds.SequenceEqual(taskNew.ChildIds) &&
                taskOld.Archived == taskNew.Archived &&
                taskOld.TimeParams == taskNew.TimeParams &&
                taskOld.checklist.items.SequenceEqual(taskNew.checklist.items))
                return true;
            return false;
        }

        public static bool operator !=(Task? taskOld, Task? taskNew)
        {
            if (!(taskOld == taskNew))
                return true;
            return false;
        }

        public void ApplyRepeat(bool reverse = false)
        {
            DateTime startTime = TimeParams.Start;
            DateTime deadline = TimeParams.Deadline;
            if (startTime != DateTime.MinValue)
            {
                if (reverse)
                {
                    startTime = startTime.AddYears(-TimeParams.repeat.years);
                    startTime = startTime.AddMonths(-TimeParams.repeat.months);
                    startTime = startTime.Add(-TimeParams.repeat.custom);
                }
                else
                {
                    startTime = startTime.AddYears(TimeParams.repeat.years);
                    startTime = startTime.AddMonths(TimeParams.repeat.months);
                    startTime = startTime.Add(TimeParams.repeat.custom);
                }
            }
            if (deadline != DateTime.MaxValue)
            {
                if (reverse)
                {
                    deadline = deadline.AddYears(-TimeParams.repeat.years);
                    deadline = deadline.AddMonths(-TimeParams.repeat.months);
                    deadline = deadline.Add(-TimeParams.repeat.custom);
                }
                else
                {
                    deadline = deadline.AddYears(TimeParams.repeat.years);
                    deadline = deadline.AddMonths(TimeParams.repeat.months);
                    deadline = deadline.Add(TimeParams.repeat.custom);
                }
            }

            TimeParams.Start = startTime;
            TimeParams.Deadline = deadline;
        }
    }
}
