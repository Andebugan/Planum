using System;
using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Entities;

namespace Planum.Model.Repository
{
    public class PlanumTaskReader : IPlanumTaskReader
    {
        RepoConfig RepoConfig { get; set; }

        public PlanumTaskReader(RepoConfig repoConfig)
        {
            RepoConfig = repoConfig;
        }

        public bool CheckTaskMarker(string line) => line.StartsWith(RepoConfig.TaskMarkerStartSymbol) && line.EndsWith(RepoConfig.TaskMarkerEndSymbol);
        public Guid ParseTaskMarker(string line)
        {
            line = line.Trim(' ', '\n');
            line = line.Remove(0, RepoConfig.TaskMarkerStartSymbol.Length);
            line = line.Remove(line.Length - RepoConfig.TaskMarkerEndSymbol.Length, RepoConfig.TaskMarkerEndSymbol.Length);
            var id = Guid.Empty;
            if (line == string.Empty)
                return id;
            if (!Guid.TryParse(line, out id))
                throw new Exception("Unable to parse task ID from marker");
            return id;
        }

        public bool CheckName(string line) => line.StartsWith(RepoConfig.TaskItemSymbol + RepoConfig.TaskNameSymbol + RepoConfig.TaskHeaderDelimeterSymbol);
        public void ParseTaskName(string line)
        {
            line = line.Replace(RepoConfig.TaskItemSymbol, "")
                .Replace(RepoConfig.TaskNameSymbol, "")
                .Replace(RepoConfig.TaskHeaderDelimeterSymbol, "")
                .Replace("\n", "");

        }

        public void ReadTask(IEnumerator<string> linesEnumerator, IList<PlanumTask> tasks, Dictionary<Guid, string> children, Dictionary<Guid, string> parents)
        {
            if (!CheckTaskMarker(linesEnumerator.Current))
                return;
            // parse task ID
            PlanumTask task = new PlanumTask();
            task.Id = ParseTaskMarker(linesEnumerator.Current);

            while (linesEnumerator.MoveNext())
            {
                // parse name
                // parse description
                // parse parent
                // parse child
                // parse checklists
                // parse deadlines
            }
        }

        PlanumTask IPlanumTaskReader.ParseRelatives(IList<PlanumTask> tasks, Dictionary<Guid, string> children, Dictionary<Guid, string> parents)
        {
            throw new NotImplementedException();
        }
    }
}
