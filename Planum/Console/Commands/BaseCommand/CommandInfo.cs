namespace Planum.Console.Commands
{
    public class CommandInfo
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Usage { get; set; } = "";

        public CommandInfo(string name, string description, string usage)
        {
            Name = name;
            Description = description;
            Usage = usage;
        }
    }
}
