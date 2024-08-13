namespace Planum.Commands
{
    public class CommandInfo
    {
        string Name { get; set; } = "";
        string Description { get; set; } = "";
        string Usage { get; set; } = "";

        public CommandInfo(string name, string description, string usage)
        {
            Name = name;
            Description = description;
            Usage = usage;
        }
    }
}
