using System;
using System.Collections.Generic;
using System.Linq;

namespace Planum.Config
{
    public class RepoConfig
    {
        public string TaskFilename { get; set; }
        public string TaskBackupFilename { get; set; }
        public string TaskFileSearchPattern { get; set; }

        Dictionary<string, IEnumerable<Guid>> taskLookupPaths;
        public Dictionary<string, IEnumerable<Guid>> TaskLookupPaths
        {
            get
            {
                return taskLookupPaths;
            }

            set
            {
                foreach (var key in value.Keys)
                    value[key] = value[key].Distinct();
                taskLookupPaths = value;
            }
        }

        public string TaskItemSymbol { get; set; } = "- ";
        public string TaskItemTabSymbol { get; set; } = "  ";
        public string TaskHeaderDelimeterSymbol { get; set; } = ": ";

        public string TaskCompleteMarkerSymbol { get; set; } = "[x] ";
        public string TaskNotCompleteMarkerSymbol { get; set; } = "[ ] ";
        public string TaskWarningMarkerSymbol { get; set; } = "[w]" ;
        public string TaskInProgressMarkerSymbol { get; set; } = "[p] ";
        public string TaskOverdueMarkerSymbol { get; set; } = "[!] ";

        public string TaskMarkerStartSymbol { get; set; } = "<planum:";
        public string TaskMarkerEndSymbol { get; set; } = ">";

        public string TaskIdSymbol { get; set; } = "i";
        public string TaskNameSymbol { get; set; } = "n";
        public string TaskNameIdDelimiter { get; set; } = "|";
        public string TaskDescriptionSymbol { get; set; } = "d";
        public string TaskParentSymbol { get; set; } = "p";
        public string TaskChildSymbol { get; set; } = "c";
        public string TaskDeadlineHeaderSymbol { get; set; } = "D";
        public string TaskDeadlineEnabledSymbol { get; set; } = "e";
        public string TaskDeadlineRepeatedSymbol { get; set; } = "r";
        public string TaskDeadlineSymbol { get; set; } = "d";
        public string TaskWarningTimeSymbol { get; set; } = "w";
        public string TaskDurationTimeSymbol { get; set; } = "du";
        public string TaskRepeatTimeSymbol { get; set; } = "r";

        public string ChecklistTaskName { get; set; } = ".checklist";
        public string CompleteTaskName { get; set; } = ".complete";
        public string OverdueTaskName { get; set; } = ".overdue";
        public string InProgressTaskName { get; set; } = ".progress";
        public string WarningTaskName { get; set; } = ".warning";

        public RepoConfig()
        {
            TaskFilename = "tasks.md";
            TaskBackupFilename = "tasks_backup.md";
            TaskFileSearchPattern = "*.md";
            taskLookupPaths = new Dictionary<string, IEnumerable<Guid>>();
        }
    }
}
