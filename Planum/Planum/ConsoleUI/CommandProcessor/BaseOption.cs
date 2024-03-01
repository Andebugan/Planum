using Planum.ConsoleUI.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planum.ConsoleUI.CommandProcessor
{
    /* template
    public class OptionName : BaseOption<T>
    {
        public OptionName(string name, string description, string usage, T Default, bool optional = true, bool used = false) : base(name, description, usage, Default, optional, used) { }

        public override bool GetValue(ref List<string> args, ref string error)
        {
            args.RemoveAt(0);
            Used = true;
        }
    }
    */

    public class BaseOption<T> : IOption
    {
        public virtual bool Optional { get; set; }

        public virtual bool Used { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string Usage { get; set; }

        public T Default;
        public T value;

        public BaseOption(string name, string description, string usage, T Default, bool optional = true, bool used = false)
        {
            if (name != "")
                Name = ArgumentParser.CommandDelimeter + name;
            else
                Name = name;
            Description = description;
            if (usage != "")
                Usage = Name + " " + usage;
            else
                Usage = Name;
            Used = used;
            Optional = optional;
            this.Default = Default;
            value = Default;
        }

        public virtual bool GetValue(ref List<string> args, ref string error)
        {
            return true;
        }

        public virtual void Reset()
        {
            Used = false;
            value = Default;
        }

        public virtual string GetDefault()
        {
            return "";
        }
    }
}
