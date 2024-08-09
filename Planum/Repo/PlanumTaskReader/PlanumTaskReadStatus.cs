using Planum.Model.Entities;

namespace Planum.Repository
{
    public enum TaskReadStatusType
    {
        OK,
        UNABLE_TO_PARSE_TASK_GUID,
        UNABLE_TO_PARSE_DEADLINE_GUID,
        UNABLE_TO_PARSE_DEADLINE,
        UNABLE_TO_PARSE_WARNING_TIME,
        UNABLE_TO_PARSE_DURATION,
        UNABLE_TO_PARSE_REPEAT_PERIOD,
        UNABLE_TO_FIND_TASK_FILE
    }

    public class TaskReadStatus
    {
        public TaskReadStatusType Status { get; set; }

        public string FilePath { get; set; }
        public int LineNumber { get; set; }

        public PlanumTask? Victim { get; set; }

        public string Line { get; set; }
        public string Message { get; set; }

        public TaskReadStatus(PlanumTask? victim = null, TaskReadStatusType status = TaskReadStatusType.OK, string filePath = "", int lineNumber = 0, string line = "", string message = "")
        {
            Victim = victim;
            Status = status;
            FilePath = filePath;
            LineNumber = lineNumber;
            Line = line;
            Message = message;
        }
    }
}
