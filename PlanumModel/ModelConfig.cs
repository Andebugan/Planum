using Planum.Config;
using Planum.Logger;

namespace Planum.Model
{
    public class ModelConfig
    {
        public string TaskDateTimeWriteFormat { get; set; } = "H:m d.M.y";
        public string TaskTimeSpanWriteFormat { get; set; } = @"d\.h\:m";

        public string TaskItemSymbol { get; set; } = "- ";
        public string TaskItemTabSymbol { get; set; } = "  ";
        public string TaskHeaderDelimeterSymbol { get; set; } = ": ";

        public string TaskDescriptionNewlineSymbol { get; set; } = "\\";

        public string TaskCheckboxStart { get; set; } = "[";
        public string TaskCheckboxEnd { get; set; } = "] ";

        public string TaskWarningMarkerSymbol { get; set; } = ".";
        public string TaskInProgressMarkerSymbol { get; set; } = "*";
        public string TaskOverdueMarkerSymbol { get; set; } = "#";
        public string TaskCompleteMarkerSymbol { get; set; } = "x";
        public string TaskNotCompleteMarkerSymbol { get; set; } = " ";

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
        protected string ModelConfigPath { get; set; } = "./ModelConfig.json";

        public ModelConfig(string configPath) => ModelConfigPath = configPath;

        public static ModelConfig Load(string configPath, ILoggerWrapper logger)
        {
            logger.Log("Loading model config", LogLevel.INFO);
            var config = ConfigLoader.LoadConfig<ModelConfig>(configPath, new ModelConfig(configPath), logger); 
            logger.Log("Model config loaded", LogLevel.INFO);
            return config;
        }

        public void Save(ILoggerWrapper logger)
        {
            logger.Log("Saving model config", LogLevel.INFO);
            ConfigLoader.SaveConfig<ModelConfig>(ModelConfigPath, this, logger);
            logger.Log("Model config saved", LogLevel.INFO);
        }
    }
}
