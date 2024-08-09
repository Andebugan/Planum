using Planum.Model.Entities;

namespace Planum.Repository
{
    public enum TaskWriteStatusType
    {
        OK,
        TASK_HEADER_NOT_FOUND,
        UNABLE_TO_FIND_TASK_FILE,
        UNABLE_TO_SKIP_TASK
    }

    public class TaskWriteStatus
    {
        public TaskWriteStatusType Status { get; set; }

        public string FilePath { get; set; }
        public int LineNumber { get; set; }

        public PlanumTask Victim { get; set; }

        public string Message { get; set; }

        public TaskWriteStatus(PlanumTask victim, TaskWriteStatusType status = TaskWriteStatusType.OK, string filePath = "", int lineNumber = 0, string message = "")
        {
            Victim = victim;
            Status = status;
            FilePath = filePath;
            LineNumber = lineNumber;
            Message = message;
        }
    }
}
