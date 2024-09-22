namespace Planum.Model.Exporters
{
    public static class TaskMarkdownExporter
    {
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
    }
}
