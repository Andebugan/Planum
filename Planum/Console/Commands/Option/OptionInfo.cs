namespace Planum.Commands
{
    public class OptionInfo
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Usage { get; set; } = "";

        public OptionInfo(string name, string description, string usage)
        {
            Name = name;
            Description = description;
            Usage = usage;
        }
    }
}
