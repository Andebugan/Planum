using Planum.Logger;

namespace Planum.Config
{
    ///<summary>Repo config DTO</summary>
    public class RepoConfigJsonDTO
    {
        public string TaskFileFilterPattern { get; set; } = "*.md";

        public List<string> TaskLookupPaths { get; set; } = new List<string>();

        public string TaskDateTimeWriteFormat { get; set; } = "H:m d.M.y";
        public string TaskTimeSpanWriteFormat { get; set; } = @"d\.h\:m";

        public string TaskItemSymbol { get; set; } = "- ";
        public string TaskItemTabSymbol { get; set; } = "  ";
        public string TaskHeaderDelimeterSymbol { get; set; } = ": ";

        public string TaskDescriptionNewlineSymbol { get; set; } = "\\";

        public string TaskCheckboxStart { get; set; } = "[";
        public string TaskCheckboxEnd { get; set; } = "] ";
        public string TaskCompleteMarkerSymbol { get; set; } = "x";
        public string TaskNotCompleteMarkerSymbol { get; set; } = " ";
        public string TaskWarningMarkerSymbol { get; set; } = ".";
        public string TaskInProgressMarkerSymbol { get; set; } = "*";
        public string TaskOverdueMarkerSymbol { get; set; } = "#";

        public string TaskMarkerStartSymbol { get; set; } = "<planum:";
        public string TaskMarkerEndSymbol { get; set; } = ">";

        public string TaskNameSymbol { get; set; } = "n";
        public string TaskNameIdDelimiter { get; set; } = " | ";

        public string TaskTagSymbol { get; set; } = "t";
        public string TaskDescriptionSymbol { get; set; } = "d";

        public string TaskParentSymbol { get; set; } = "p";
        public string TaskChildSymbol { get; set; } = "c";

        public string TaskDeadlineHeaderSymbol { get; set; } = "D";
        public string TaskDeadlineSymbol { get; set; } = "d";

        public string TaskWarningTimeSymbol { get; set; } = "w";
        public string TaskDurationTimeSymbol { get; set; } = "du";
        public string TaskRepeatTimeSymbol { get; set; } = "r";

        public string TaskNextSymbol { get; set; } = "n";
    }

    ///<summary>Repository parsing settings config</summary>
    public class RepoConfig
    {
        public string RepoConfigPath { get; set; } = "./RepoConfig.json";

        public string TaskFileFilterPattern { get; set; } = "*.md";

        public HashSet<string> TaskLookupPaths = new HashSet<string>();

        public string TaskDateTimeWriteFormat { get; set; } = "H:m d.M.y";
        public string TaskTimeSpanWriteFormat { get; set; } = @"d\.h\:m";

        public string TaskItemSymbol { get; set; } = "- ";
        public string TaskItemTabSymbol { get; set; } = "  ";
        public string TaskHeaderDelimeterSymbol { get; set; } = ": ";

        public string TaskDescriptionNewlineSymbol { get; set; } = "\\";

        public string TaskCheckboxStart { get; set; } = "[";
        public string TaskCheckboxEnd { get; set; } = "] ";
        public string TaskCompleteMarkerSymbol { get; set; } = "x";
        public string TaskNotCompleteMarkerSymbol { get; set; } = " ";
        public string TaskWarningMarkerSymbol { get; set; } = ".";
        public string TaskInProgressMarkerSymbol { get; set; } = "*";
        public string TaskOverdueMarkerSymbol { get; set; } = "#";

        public string TaskMarkerStartSymbol { get; set; } = "<planum:";
        public string TaskMarkerEndSymbol { get; set; } = ">";

        public string TaskNameSymbol { get; set; } = "n";
        public string TaskNameIdDelimiter { get; set; } = " | ";

        public string TaskTagSymbol { get; set; } = "t";
        public string TaskDescriptionSymbol { get; set; } = "d";

        public string TaskParentSymbol { get; set; } = "p";
        public string TaskChildSymbol { get; set; } = "c";

        public string TaskDeadlineHeaderSymbol { get; set; } = "D";
        public string TaskDeadlineSymbol { get; set; } = "d";

        public string TaskWarningTimeSymbol { get; set; } = "w";
        public string TaskDurationTimeSymbol { get; set; } = "du";
        public string TaskRepeatTimeSymbol { get; set; } = "r";

        public string TaskNextSymbol { get; set; } = "n";

        ///<summary>Add checkbox to task marker</summary>
        public string AddCheckbox(string marker) => TaskCheckboxStart + marker + TaskCheckboxEnd;

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

        public string ParseMarkdownLink(string line)
        {
            var split = line.Trim('[', ')').Split("](");
            if (split.Length == 2)
            {
                return split[0];
            }
            else if (split.Length == 1)
                return split[0];
            return "";
        }

        public void ParseTaskName(string line, out string id, out string name)
        {
            id = "";
            name = "";
            var split = line.Trim(' ', '\n').Split(TaskNameIdDelimiter);
            if (split.Length == 1)
                name = split[0];
            else if (split.Length == 2)
            {
                name = split[0];
                id = split[1];
            }
        }

        public RepoConfig(string repoConfigPath = "")
        {
            RepoConfigPath = repoConfigPath;
        }

        public string[] GetTaskStatusMarkerSymbols()
        {
            return new string[] {
                TaskNotCompleteMarkerSymbol,
                TaskWarningMarkerSymbol,
                TaskCompleteMarkerSymbol,
                TaskInProgressMarkerSymbol,
                TaskOverdueMarkerSymbol,
            };
        }

        static RepoConfigJsonDTO ToJsonDTO(RepoConfig config)
        {
            RepoConfigJsonDTO configJsonDTO = new RepoConfigJsonDTO();

            configJsonDTO.TaskFileFilterPattern = config.TaskFileFilterPattern;

            configJsonDTO.TaskLookupPaths = config.TaskLookupPaths.ToList();

            configJsonDTO.TaskDateTimeWriteFormat = config.TaskDateTimeWriteFormat;
            configJsonDTO.TaskTimeSpanWriteFormat = config.TaskTimeSpanWriteFormat;
            configJsonDTO.TaskItemSymbol = config.TaskItemSymbol;
            configJsonDTO.TaskItemTabSymbol = config.TaskItemTabSymbol;
            configJsonDTO.TaskHeaderDelimeterSymbol = config.TaskHeaderDelimeterSymbol;
            configJsonDTO.TaskCheckboxStart = config.TaskCheckboxStart;
            configJsonDTO.TaskCheckboxEnd = config.TaskCheckboxEnd;
            configJsonDTO.TaskDescriptionNewlineSymbol = config.TaskDescriptionNewlineSymbol;
            configJsonDTO.TaskCompleteMarkerSymbol = config.TaskCompleteMarkerSymbol;
            configJsonDTO.TaskNotCompleteMarkerSymbol = config.TaskNotCompleteMarkerSymbol;
            configJsonDTO.TaskWarningMarkerSymbol = config.TaskWarningMarkerSymbol;
            configJsonDTO.TaskInProgressMarkerSymbol = config.TaskInProgressMarkerSymbol;
            configJsonDTO.TaskOverdueMarkerSymbol = config.TaskOverdueMarkerSymbol;
            configJsonDTO.TaskMarkerStartSymbol = config.TaskMarkerStartSymbol;
            configJsonDTO.TaskMarkerEndSymbol = config.TaskMarkerEndSymbol;
            configJsonDTO.TaskNameSymbol = config.TaskNameSymbol;
            configJsonDTO.TaskNameIdDelimiter = config.TaskNameIdDelimiter;
            configJsonDTO.TaskTagSymbol = config.TaskTagSymbol;
            configJsonDTO.TaskDescriptionSymbol = config.TaskDescriptionSymbol;
            configJsonDTO.TaskParentSymbol = config.TaskParentSymbol;
            configJsonDTO.TaskChildSymbol = config.TaskChildSymbol;
            configJsonDTO.TaskDeadlineHeaderSymbol = config.TaskDeadlineHeaderSymbol;
            configJsonDTO.TaskDeadlineSymbol = config.TaskDeadlineSymbol;
            configJsonDTO.TaskWarningTimeSymbol = config.TaskWarningTimeSymbol;
            configJsonDTO.TaskDurationTimeSymbol = config.TaskDurationTimeSymbol;
            configJsonDTO.TaskRepeatTimeSymbol = config.TaskRepeatTimeSymbol;
            configJsonDTO.TaskNextSymbol = config.TaskNextSymbol;

            return configJsonDTO;
        }

        static RepoConfig FromJsonDTO(RepoConfigJsonDTO configJsonDTO)
        {
            RepoConfig config = new RepoConfig();
            
            config.TaskFileFilterPattern = configJsonDTO.TaskFileFilterPattern;

            config.TaskLookupPaths = configJsonDTO.TaskLookupPaths.ToHashSet();

            config.TaskDateTimeWriteFormat = configJsonDTO.TaskDateTimeWriteFormat;
            config.TaskTimeSpanWriteFormat = configJsonDTO.TaskTimeSpanWriteFormat;
            config.TaskItemSymbol = configJsonDTO.TaskItemSymbol;
            config.TaskItemTabSymbol = configJsonDTO.TaskItemTabSymbol;
            config.TaskHeaderDelimeterSymbol = configJsonDTO.TaskHeaderDelimeterSymbol;
            config.TaskDescriptionNewlineSymbol = configJsonDTO.TaskDescriptionNewlineSymbol;
            config.TaskCheckboxStart = configJsonDTO.TaskCheckboxStart;
            config.TaskCheckboxEnd = configJsonDTO.TaskCheckboxEnd;
            config.TaskDescriptionNewlineSymbol = configJsonDTO.TaskDescriptionNewlineSymbol;
            config.TaskCompleteMarkerSymbol = configJsonDTO.TaskCompleteMarkerSymbol;
            config.TaskNotCompleteMarkerSymbol = configJsonDTO.TaskNotCompleteMarkerSymbol;
            config.TaskWarningMarkerSymbol = configJsonDTO.TaskWarningMarkerSymbol;
            config.TaskInProgressMarkerSymbol = configJsonDTO.TaskInProgressMarkerSymbol;
            config.TaskOverdueMarkerSymbol = configJsonDTO.TaskOverdueMarkerSymbol;
            config.TaskMarkerStartSymbol = configJsonDTO.TaskMarkerStartSymbol;
            config.TaskMarkerEndSymbol = configJsonDTO.TaskMarkerEndSymbol;
            config.TaskNameSymbol = configJsonDTO.TaskNameSymbol;
            config.TaskNameIdDelimiter = configJsonDTO.TaskNameIdDelimiter;
            config.TaskTagSymbol = configJsonDTO.TaskTagSymbol;
            config.TaskDescriptionSymbol = configJsonDTO.TaskDescriptionSymbol;
            config.TaskParentSymbol = configJsonDTO.TaskParentSymbol;
            config.TaskChildSymbol = configJsonDTO.TaskChildSymbol;
            config.TaskDeadlineHeaderSymbol = configJsonDTO.TaskDeadlineHeaderSymbol;
            config.TaskDeadlineSymbol = configJsonDTO.TaskDeadlineSymbol;
            config.TaskWarningTimeSymbol = configJsonDTO.TaskWarningTimeSymbol;
            config.TaskDurationTimeSymbol = configJsonDTO.TaskDurationTimeSymbol;
            config.TaskRepeatTimeSymbol = configJsonDTO.TaskRepeatTimeSymbol;
            config.TaskNextSymbol = configJsonDTO.TaskNextSymbol;

            return config;
        }

        /// <summary>Load repo config to path defined in app config</summary>
        public static RepoConfig Load(string repoConfigPath, ILoggerWrapper logger)
        {
            logger.Log("Loading repo config", LogLevel.INFO);
            RepoConfigJsonDTO configDTO = ConfigLoader.LoadConfig<RepoConfigJsonDTO>(repoConfigPath, new RepoConfigJsonDTO(), logger);
            var repoConfig = FromJsonDTO(configDTO);
            repoConfig.RepoConfigPath = repoConfigPath;
            logger.Log("Repo config loaded", LogLevel.INFO);
            return repoConfig;
        }

        /// <summary>Save repo config to path defined in app config</summary>
        public void Save(ILoggerWrapper logger)
        {
            logger.Log("Saving repo config", LogLevel.INFO);
            ConfigLoader.SaveConfig<RepoConfigJsonDTO>(RepoConfigPath, ToJsonDTO(this), logger);
            logger.Log("Repo config saved", LogLevel.INFO);
        }
    }
}
