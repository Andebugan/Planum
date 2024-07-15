using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Planum.Config
{
    public class RepoConfig
    {
        public string TaskFilename { get; set; }
        public string TaskFileSearchPattern { get; set; }

        Dictionary<string, IEnumerable<Guid>> taskLookupPaths;
        public Dictionary<string, IEnumerable<Guid>> TaskLookupPaths
        {
            get => taskLookupPaths;

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

        public string TaskNameSymbol { get; set; } = "n";
        public string TaskNameIdDelimiter { get; set; } = "| ";

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

        public string TaskPipelineDelimeterSymbol { get; set; } = " => ";
        public string AddMarkdownLink(string line, string path) => "[" + line + "](" + path + ")";
        public string ParseMarkdownLink(string line, out string path)
        {
            path = ""; 
            var split = line.Trim('[', ')').Split("](");
            if (split.Length == 2)
            {
                path = split[1];
                return split[0];
            }
            else if (split.Length == 1)
                return split[0];
            return "";
        }

        public RepoConfig()
        {
            TaskFilename = "tasks.md";
            TaskFileSearchPattern = "*.md";
            taskLookupPaths = new Dictionary<string, IEnumerable<Guid>>();
        }

        public string GetTaskPath(Guid id)
        {
            var paths = TaskLookupPaths.Keys.Where(x => TaskLookupPaths[x].Contains(id));
            if (paths.Any())
                return paths.First();
            return "";
        }

        public string GetDefaultFilePath()
        {
            string savePath = AppContext.BaseDirectory;
            return Path.Combine(savePath, TaskFilename);
        }

        public static RepoConfig Load()
        {
            var appConfig = AppConfig.Load();
            return ConfigLoader.LoadConfig<RepoConfig>(appConfig.RepoConfigPath, new RepoConfig());
        }

        public void Save()
        {
            var appConfig = AppConfig.Load();
            ConfigLoader.SaveConfig<RepoConfig>(appConfig.RepoConfigPath, this);
        }
    }
}
