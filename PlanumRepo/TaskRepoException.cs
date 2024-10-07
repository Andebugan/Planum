namespace Planum.Repository
{
    public class TaskRepoException : Exception
    {
        public TaskRepoException() { }
        public TaskRepoException(string? message) : base(message) { }
        public TaskRepoException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
