namespace Planum.Console.Commands.Special
{
    public class TaskStorageCommandSettings: ICommandSettings
    {
        public HashSet<string> TaskLookupPaths { get; set; } = new HashSet<string>();
        public bool ListResult { get; set; } = false;
    }
}
