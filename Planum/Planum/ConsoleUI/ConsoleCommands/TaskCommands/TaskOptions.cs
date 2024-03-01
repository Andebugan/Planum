using Planum.ConsoleUI.CommandProcessor;
using Planum.ConsoleUI.UI;
using Planum.Model.Entities;
using Planum.Model.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands.TaskCommands
{
    public class IntValueOption : BaseOption<int>
    {
        bool onlyPositive = false;
        int? defaultVal = null;

        public IntValueOption(string name, string description, string usage, int Default, bool optional = true, bool used = false, bool onlyPositive = false, int? defaultVal = null) : base(name, description, usage, Default, optional, used)
        {
            this.onlyPositive = onlyPositive;
            this.defaultVal = defaultVal;
        }

        public override bool GetValue(ref List<string> args, ref string error)
        {
            args.RemoveAt(0);
            Used = true;
            int temp = 0;
            bool result = ArgumentParser.Parse(ref temp, ref args, defaultVal);
            if (!result)
                error = "failed to parse the argument:" + args.Last().ToString();
            
            value = temp;
            if (onlyPositive && value <= 0)
            {
                error = "id must be greater or equal than zero";
                return false;
            }
            return result;
        }

        public override string GetDefault()
        {
            if (defaultVal == null)
                return "doesn't have one";
            else
                return this.defaultVal.ToString();
        }
    }

    public class IntListValueOption : BaseOption<List<int>>
    {
        List<int>? defaultVal = null;

        public IntListValueOption(string name, string description, string usage, List<int> Default, bool optional = true, bool used = false, List<int>? defaultVal = null) : base(name, description, usage, Default, optional, used)
        {
            value = new List<int>(Default);
            this.defaultVal = defaultVal;
        }

        public override bool GetValue(ref List<string> args, ref string error)
        {
            args.RemoveAt(0);
            Used = true;
            List<int> temp = new List<int>();
            bool result = ArgumentParser.Parse(ref temp, ref args, defaultVal);
            if (!result)
                error = "failed to parse the argument:" + args.Last().ToString();
            value = temp;
            return result;
        }

        public override void Reset()
        {
            Used = false;
            value = new List<int>(Default);
        }
       
        public override string GetDefault()
        {
            if (defaultVal == null)
                return "doesn't have one";
            else
                return this.defaultVal.ToString();
        }
    }

    public class TaskNameOption : BaseOption<string>
    {
        TaskManager taskManager;
        string? defaultVal = null;

        public TaskNameOption(TaskManager taskManager, string name, string description, string usage, string Default, bool optional = true, bool used = false, string? defaultVal = null) : base(name, description, usage, Default, optional, used)
        {
            this.taskManager = taskManager;
            this.defaultVal = defaultVal;
        }

        public override bool GetValue(ref List<string> args, ref string error)
        {
            args.RemoveAt(0);
            Used = true;
            string temp = "";
            bool result = ArgumentParser.Parse(ref temp, ref args, defaultVal);
            if (!result)
            {
                error = "failed to parse the argument:" + args.Last().ToString();
                return false;
            }
            value = temp;
            Task? task = taskManager.FindTask(value);
            if (task != null)
            {
                error = "task with name: " + value + " already exists";
                return false;
            }
            return result;
        }
        public override string GetDefault()
        {
            if (defaultVal == null)
                return "doesn't have one";
            else
                return this.defaultVal.ToString();
        }
    }

    public class StringOption : BaseOption<string>
    {
        string? defaultVal = null;

        public StringOption(string name, string description, string usage, string Default, bool optional = true, bool used = false, string? defaultVal = null) : base(name, description, usage, Default, optional, used)
        {
            this.defaultVal = defaultVal;
        }

        public override bool GetValue(ref List<string> args, ref string error)
        {
            args.RemoveAt(0);
            Used = true;
            string temp = "";
            bool result = ArgumentParser.Parse(ref temp, ref args, defaultVal);
            if (!result)
                error = "failed to parse the argument:" + args.Last().ToString();
            value = temp;
            return result;
        }
        public override string GetDefault()
        {
            if (defaultVal == null)
                return "doesn't have one";
            else
                return this.defaultVal.ToString();
        }
    }

    public class StringListOption : BaseOption<List<string>>
    {
        List<string>? defaultVal = null;

        public StringListOption(string name, string description, string usage, List<string> Default, bool optional = true, bool used = false, List<string>? defaultVal = null) : base(name, description, usage, Default, optional, used)
        {
            value = new List<string>(Default);
            this.defaultVal = defaultVal;
        }

        public override bool GetValue(ref List<string> args, ref string error)
        {
            args.RemoveAt(0);
            Used = true;
            List<string> temp = new List<string>();
            bool result = ArgumentParser.Parse(ref temp, ref args, defaultVal);
            if (!result)
            {
                error = "failed to parse the argument:" + args.Last().ToString();
                return false;
            }
            foreach (string item in temp)
                value.Add(item);
            return result;
        }

        public override void Reset()
        {
            Used = false;
            value = new List<string>(Default);
        }
        public override string GetDefault()
        {
            if (defaultVal == null)
                return "doesn't have one";
            else
                return this.defaultVal.ToString();
        }
    }

    public class TaskIdOption : BaseOption<int>
    {
        TaskManager taskManager;
        int? defaultVal = null;

        public TaskIdOption(TaskManager taskManager, string name, string description, string usage, int Default, bool optional = true, bool used = false, int? defaultVal = null) : base(name, description, usage, Default, optional, used)
        {
            this.taskManager = taskManager;
            value = Default;
            this.defaultVal = defaultVal;
        }

        public override bool GetValue(ref List<string> args, ref string error)
        {
            args.RemoveAt(0);
            Used = true;
            int val = -1;
            bool result = ArgumentParser.Parse(ref val, ref args, defaultVal);
            if (!result)
                error = "failed to parse the argument:" + args.Last().ToString();
            value = val;
            Task? task = taskManager.FindTask(value);
            if (task == null)
            {
                error = "task with id " + val + " does not exist";
                return false;
            }
            return result;
        }

        public override void Reset()
        {
            Used = false;
            value = Default;
        }
        public override string GetDefault()
        {
            if (defaultVal == null)
                return "doesn't have one";
            else
                return this.defaultVal.ToString();
        }
    }

    public class TaskIdsOption : BaseOption<List<int>>
    {
        TaskManager taskManager;
        List<int>? defaultVal = null;

        public TaskIdsOption(TaskManager taskManager, string name, string description, string usage, List<int> Default, bool optional = true, bool used = false, List<int>? defaultVal = null) : base(name, description, usage, Default, optional, used)
        {
            this.taskManager = taskManager;
            value = new List<int>(Default);
            this.defaultVal = defaultVal;
        }

        public override bool GetValue(ref List<string> args, ref string error)
        {
            args.RemoveAt(0);
            Used = true;
            List<int> val = new List<int>();
            bool result = ArgumentParser.Parse(ref val, ref args, defaultVal);
            if (!result)
                error = "failed to parse the argument:" + args.Last().ToString();
            value = val.Distinct().ToList();
            foreach (var id in value)
            {
                Task? task = taskManager.FindTask(id);
                if (task == null)
                {
                    error = "task with id " + id.ToString() + " does not exist";
                    return false;
                }
            }
            value = value.Distinct().ToList();
            return result;
        }

        public override void Reset()
        {
            Used = false;
            value = new List<int>(Default);
        }
        public override string GetDefault()
        {
            if (defaultVal == null)
                return "doesn't have one";
            else
                return this.defaultVal.ToString();
        }
    }

    public class TaskNamesOption : BaseOption<List<int>>
    {
        TaskManager taskManager;
        List<string>? defaultVal = null;

        public TaskNamesOption(TaskManager taskManager, string name, string description, string usage, List<int> Default, bool optional = true, bool used = false, List<string>? defaultVal = null) : base(name, description, usage, Default, optional, used)
        {
            this.taskManager = taskManager;
            value = new List<int>(Default);
            this.defaultVal = defaultVal;
        }

        public override bool GetValue(ref List<string> args, ref string error)
        {
            args.RemoveAt(0);
            Used = true;
            List<string> temp = new List<string>();
            bool result = ArgumentParser.Parse(ref temp, ref args, defaultVal);
            if (!result)
            {
                error = "failed to parse the argument:" + args.Last().ToString();
                return false;
            }
            foreach (var name in temp)
            {
                Task? task = taskManager.FindTask(name);
                if (task == null)
                {
                    error = "task with name " + name + " does not exist";
                    return false;
                }
                value.Add(task.Id);
            }
            value = value.Distinct().ToList();
            return true;
        }

        public override void Reset()
        {
            Used = false;
            value = new List<int>(Default);
        }

        public override string GetDefault()
        {
            if (defaultVal == null)
                return "doesn't have one";
            else
                return this.defaultVal.ToString();
        }
    }

    public class BoolSettingOption : BaseOption<bool>
    {
        public BoolSettingOption(string name, string description, string usage, bool Default, bool optional = true, bool used = false) : base(name, description, usage, Default, optional, used) { }

        public override bool GetValue(ref List<string> args, ref string error)
        {
            args.RemoveAt(0);
            Used = true;
            value = true;
            return true;
        }
        public override string GetDefault()
        {
            return Default.ToString();
        }
    }

    public class BoolValueOption : BaseOption<bool>
    {
        bool? defaultVal = null;

        public BoolValueOption(string name, string description, string usage, bool Default, bool optional = true, bool used = false, bool? defaultVal = null) : base(name, description, usage, Default, optional, used) { }

        public override bool GetValue(ref List<string> args, ref string error)
        {
            args.RemoveAt(0);
            Used = true;
            bool temp = false;
            bool result = ArgumentParser.Parse(ref temp, ref args, defaultVal);
            if (!result)
                error = "failed to parse the argument:" + args.Last().ToString();
            value = temp;
            return result;
        }

        public override string GetDefault()
        {
            if (defaultVal == null)
                return "doesn't have one";
            else
                return this.defaultVal.ToString();
        }
    }

    public class DateTimeOption : BaseOption<DateTime>
    {
        DateTime? defaultVal = null;
        bool defaultEndOfDay = false;
        bool defaultValNow = false;

        public DateTimeOption(string name, string description, string usage, DateTime Default, bool defaultEndOfDay = false, bool optional = true, bool used = false, DateTime? defaultVal = null, bool defaultValNow = false) : base(name, description, usage, Default, optional, used)
        {
            this.defaultVal = defaultVal;
            this.defaultEndOfDay = defaultEndOfDay;
            this.defaultValNow = defaultValNow;
        }

        public override bool GetValue(ref List<string> args, ref string error)
        {
            args.RemoveAt(0);
            Used = true;
            DateTime temp = DateTime.Now;
            bool result = ArgumentParser.Parse(ref temp, ref args, defaultVal);
            if (!result)
                error = "failed to parse the argument:" + args.Last().ToString();
            if (defaultEndOfDay && temp.Hour == 0 && temp.Minute == 0)
                temp = temp.AddMinutes(59).AddHours(23);
            value = temp;
            return result;
        }

        public override string GetDefault()
        {
            if (defaultVal == null)
                return "doesn't have one";
            else
                return this.defaultVal.ToString();
        }

        public override void Reset()
        {
            base.Reset();
            if (defaultValNow)
            {
                defaultVal = DateTime.Now;
                value = DateTime.Now;
            }
        }
    }

    public class TimespanOption : BaseOption<TimeSpan>
    {
        TimeSpan? defaultVal = null;

        public TimespanOption(string name, string description, string usage, TimeSpan Default, bool optional = true, bool used = false, TimeSpan? defaultVal = null) : base(name, description, usage, Default, optional, used)
        {
            this.defaultVal = defaultVal;
        }

        public override bool GetValue(ref List<string> args, ref string error)
        {
            args.RemoveAt(0);
            Used = true;
            TimeSpan temp = TimeSpan.Zero;
            bool result = ArgumentParser.Parse(ref temp, ref args, defaultVal);
            if (!result)
            {
                error = "failed to parse the argument:" + args.Last().ToString();
                return false;
            }
            value = temp;
            return result;
        }

        public override string GetDefault()
        {
            if (defaultVal == null)
                return "doesn't have one";
            else
                return this.defaultVal.ToString();
        }
    }
}
