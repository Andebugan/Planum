namespace Planum.Commands
{
    public class OptionInfo
    {
        string Name { get; set; } = "";
        string Description { get; set; } = "";
        string Usage { get; set; } = "";

        public OptionInfo(string name, string description, string usage)
        {
            Name = name;
            Description = description;
            Usage = usage;
        }
    }
}
