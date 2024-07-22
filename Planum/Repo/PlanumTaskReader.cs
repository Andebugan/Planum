using System;
using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Entities;
using Planum.Model.Repository;
using Planum.Parser;

namespace Planum.Repository
{
    public class PlanumTaskReader : IPlanumTaskReader
    {
        AppConfig AppConfig { get; set; }
        RepoConfig RepoConfig { get; set; }

        public PlanumTaskReader(AppConfig appConfig, RepoConfig repoConfig)
        {
            AppConfig = appConfig;
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

        public bool CheckName(string line)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
            {
                if (line.StartsWith(RepoConfig.TaskItemSymbol +
                    statusMarker +
                    RepoConfig.TaskNameSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            }
            return false;
        }

        public string ParseTaskName(string line, ref bool complete)
        {
            if (line.StartsWith(RepoConfig.TaskItemSymbol +
                RepoConfig.TaskCompleteMarkerSymbol +
                RepoConfig.TaskNameSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol))
                complete = true;
            return line.Remove(0, (RepoConfig.TaskItemSymbol +
            RepoConfig.TaskDummyMarkerSymbol +
            RepoConfig.TaskNameSymbol +
            RepoConfig.TaskHeaderDelimeterSymbol).Length);
        }

        public bool CheckTag(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskTagSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        public string ParseTaskTag(string line, int level = 0) => line.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskTagSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol).Length);

        public bool CheckDescription(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDescriptionSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        public string ParseTaskDescription(string line, int level = 0) => line.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDescriptionSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol).Length);

        public bool CheckParent(string line)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(RepoConfig.TaskItemSymbol +
                                    statusMarker +
                                    RepoConfig.TaskParentSymbol +
                                    RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }
        public string ParseParentString(string line) => RepoConfig.ParseMarkdownLink(line.Remove(0, (RepoConfig.TaskItemSymbol +
                                                                        RepoConfig.TaskParentSymbol +
                                                                        RepoConfig.TaskHeaderDelimeterSymbol).Length));

        public bool CheckChild(string line)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(RepoConfig.TaskItemSymbol +
                                    statusMarker +
                                    RepoConfig.TaskChildSymbol +
                                    RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }
        public string ParseChildString(string line) => RepoConfig.ParseMarkdownLink(line.Remove(0, (RepoConfig.TaskItemSymbol +
                                                                        RepoConfig.TaskChildSymbol +
                                                                        RepoConfig.TaskHeaderDelimeterSymbol).Length));

        public string AddLineTabs(int level = 0)
        {
            var tabs = "";
            for (int i = 0; i < level; i++)
                tabs += RepoConfig.TaskItemTabSymbol;
            return tabs;
        }

        public bool CheckChecklist(string line, int level = 0)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(AddLineTabs(level) +
                            RepoConfig.TaskItemSymbol +
                            statusMarker +
                            RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }
        public void ParseChecklist(Guid taskId, IEnumerator<string> linesEnumerator, IList<PlanumTask> tasks, Dictionary<Guid, IList<string>> next, int level = 0)
        {
            var checklistTask = new PlanumTask(id: Guid.NewGuid());
            checklistTask.Parents.Add(taskId);
            checklistTask.Tags.Add(DefaultTags.Checklist);

            // parse checklist name
            if (linesEnumerator.Current.StartsWith(AddLineTabs(level) +
                                                    RepoConfig.TaskItemSymbol +
                                                    RepoConfig.TaskCompleteMarkerSymbol +
                                                    RepoConfig.TaskHeaderDelimeterSymbol))
            {
                checklistTask.Name = linesEnumerator.Current.Remove(0, (AddLineTabs(level) +
                                                                        RepoConfig.TaskItemSymbol +
                                                                        RepoConfig.TaskCompleteMarkerSymbol +
                                                                        RepoConfig.TaskHeaderDelimeterSymbol).Length);
                checklistTask.Tags.Add(DefaultTags.Complete);
            }

            // finish parsing checklist + recursive part
            while (linesEnumerator.MoveNext())
            {
                // parse description
                if (CheckDescription(linesEnumerator.Current, level + 1))
                    checklistTask.Description = ParseTaskDescription(linesEnumerator.Current, level + 1);
                // parse tag
                else if (CheckTag(linesEnumerator.Current))
                    checklistTask.Tags.Add(ParseTaskTag(linesEnumerator.Current, level + 1));
                // parse deadline
                else if (CheckDeadline(linesEnumerator.Current, level + 1))
                    checklistTask.Deadlines.Add(ParseDeadline(linesEnumerator, next, level + 1));
                // parse checklist
                else if (CheckChecklist(linesEnumerator.Current, level + 1))
                    ParseChecklist(checklistTask.Id, linesEnumerator, tasks, next, level + 1);
                else
                    return;
            }
        }

        public bool CheckDeadline(string line, int level = 0)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(RepoConfig.TaskItemSymbol +
                                    statusMarker +
                                    RepoConfig.TaskDeadlineSymbol +
                                    RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }

        public bool CheckDeadlineDate(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDeadlineSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        public DateTime ParseDeadlineDate(string line, int level = 0)
        {
            line = line.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDeadlineSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol).Length);
            DateTime deadline = DateTime.Today;
            if (!ValueParser.TryParse(ref deadline, line))
                throw new Exception($"Unable to parse deadline: {line}");
            return deadline;
        }

        public bool CheckWarningTime(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskWarningTimeSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        public TimeSpan ParseWarningTime(string line, int level = 0)
        {
            TimeSpan warningTime = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref warningTime, line))
                throw new Exception($"Unable to parse warning time");
            return warningTime;
        }

        public bool CheckDuration(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDurationTimeSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        public TimeSpan ParseDuration(string line, int level = 0)
        {
            TimeSpan duration = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref duration, line))
                throw new Exception($"Unable to parse duration");
            return duration;
        }

        public bool CheckRepeated(string line, int level = 0)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(AddLineTabs(level) +
                            RepoConfig.TaskItemSymbol +
                            statusMarker +
                            RepoConfig.TaskRepeatTimeSymbol +
                            RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }
        public void ParseRepeat(string line, ref Deadline deadline, int level = 0)
        {
            // parse header
            if (line.StartsWith(AddLineTabs(level) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskNotCompleteMarkerSymbol +
                        RepoConfig.TaskRepeatTimeSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol))
                deadline.repeated = false;
            else
                deadline.repeated = true;

            line = line.Remove(0, (AddLineTabs(level) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskDummyMarkerSymbol +
                        RepoConfig.TaskRepeatTimeSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol).Length);

            if (!ValueParser.TryParse(ref deadline.repeatSpan, ref deadline.repeatMonths, ref deadline.repeatYears, line))
                throw new Exception($"Unable to parse repeated");
        }

        public bool CheckNext(string line, int level = 0)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(AddLineTabs(level) +
                            RepoConfig.TaskItemSymbol +
                            statusMarker +
                            RepoConfig.TaskNextSymbol +
                            RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }
        public string ParseNextTaskString(string line, int level = 0) => RepoConfig.ParseMarkdownLink(line.Remove(0, (AddLineTabs(level) +
                    RepoConfig.TaskItemSymbol +
                    RepoConfig.TaskNextSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol).Length));

        public Deadline ParseDeadline(IEnumerator<string> linesEnumerator, Dictionary<Guid, IList<string>> next, int level = 0)
        {
            Deadline deadline = new Deadline();

            // parse header
            var line = linesEnumerator.Current.Remove(0, (AddLineTabs(level) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskNotCompleteMarkerSymbol +
                        RepoConfig.TaskDeadlineSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol).Length).Trim(' ', '\n');

            if (linesEnumerator.Current.StartsWith(AddLineTabs(level) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskNotCompleteMarkerSymbol +
                        RepoConfig.TaskDeadlineSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol))
                deadline.enabled = false;
            else
                deadline.enabled = true;
            var deadlineId = Guid.NewGuid();
            if (line != string.Empty && !ValueParser.TryParse(ref deadlineId, line))
                throw new Exception("Unable to parse deadline id");
            deadline.Id = deadlineId;

            while (linesEnumerator.MoveNext())
            {
                // parse deadline
                if (CheckDeadlineDate(linesEnumerator.Current, level + 1))
                    deadline.deadline = ParseDeadlineDate(linesEnumerator.Current, level + 1);
                // parse warning
                else if (CheckWarningTime(linesEnumerator.Current, level + 1))
                    deadline.warningTime = ParseWarningTime(linesEnumerator.Current, level + 1);
                // parse duration
                else if (CheckDuration(linesEnumerator.Current, level + 1))
                    deadline.duration = ParseDuration(linesEnumerator.Current, level + 1);
                // parse repeat duration
                else if (CheckRepeated(linesEnumerator.Current, level + 1))
                    ParseRepeat(linesEnumerator.Current, ref deadline, level + 1);
                // parse next tasks 
                else if (CheckNext(linesEnumerator.Current, level + 1))
                    if (next.ContainsKey(deadline.Id))
                        next[deadline.Id].Add(ParseNextTaskString(linesEnumerator.Current, level + 1));
                    else
                        next[deadline.Id] = new List<string>();
                else
                    break;
            }

            return deadline;
        }

        public Guid ReadTask(IEnumerator<string> linesEnumerator, IList<PlanumTask> tasks, Dictionary<Guid, IList<string>> children, Dictionary<Guid, IList<string>> parents, Dictionary<Guid, IList<string>> next)
        {
            if (!CheckTaskMarker(linesEnumerator.Current))
                return Guid.Empty;
            // parse task ID
            PlanumTask task = new PlanumTask();
            task.Id = ParseTaskMarker(linesEnumerator.Current);

            while (linesEnumerator.MoveNext())
            {
                // parse name
                if (CheckName(linesEnumerator.Current))
                {
                    var complete = false;
                    task.Name = ParseTaskName(linesEnumerator.Current, ref complete);
                    task.Tags.Add(DefaultTags.Complete);
                }
                // parse tag
                else if (CheckTag(linesEnumerator.Current))
                    task.Tags.Add(ParseTaskTag(linesEnumerator.Current));
                // parse description
                else if (CheckDescription(linesEnumerator.Current))
                    task.Description = ParseTaskDescription(linesEnumerator.Current);
                // parse child
                else if (CheckChild(linesEnumerator.Current))
                    if (children.ContainsKey(task.Id))
                        children[task.Id].Add(ParseChildString(linesEnumerator.Current));
                    else
                        children[task.Id] = new List<string>();
                // parse parent
                else if (CheckParent(linesEnumerator.Current))
                    if (parents.ContainsKey(task.Id))
                        parents[task.Id].Add(ParseParentString(linesEnumerator.Current));
                    else
                        parents[task.Id] = new List<string>();
                // parse deadlines
                else if (CheckDeadline(linesEnumerator.Current))
                    task.Deadlines.Add(ParseDeadline(linesEnumerator, next, 0));
                // parse checklists
                else if (CheckChecklist(linesEnumerator.Current))
                    ParseChecklist(task.Id, linesEnumerator, tasks, next, 0);
                else
                {
                    tasks.Add(task);
                    break;
                }
            }
            return task.Id;
        }

        public void ReadSkipDeadline(IEnumerator<string> linesEnumerator, int level = 0)
        {
            while (linesEnumerator.MoveNext())
            {
                if (CheckDeadlineDate(linesEnumerator.Current, level + 1))
                    continue;
                else if (CheckWarningTime(linesEnumerator.Current, level + 1))
                    continue;
                else if (CheckDuration(linesEnumerator.Current, level + 1))
                    continue;
                else if (CheckRepeated(linesEnumerator.Current, level + 1))
                    continue;
                else if (CheckNext(linesEnumerator.Current, level + 1))
                    continue;
                else
                    break;
            }
        }

        public void ReadSkipChecklists(IEnumerator<string> linesEnumerator, int level = 0)
        {
            while (linesEnumerator.MoveNext())
            {
                if (CheckDescription(linesEnumerator.Current, level + 1))
                    continue;
                else if (CheckTag(linesEnumerator.Current, level + 1))
                    continue;
                else if (CheckDeadline(linesEnumerator.Current, level + 1))
                    ReadSkipDeadline(linesEnumerator, level + 1);
                else if (CheckChecklist(linesEnumerator.Current, level + 1))
                    ReadSkipChecklists(linesEnumerator, level + 1);
                else
                    return;
            }
        }

        public Guid ReadSkipTask(IEnumerator<string> linesEnumerator)
        {
            if (!CheckTaskMarker(linesEnumerator.Current))
                return Guid.Empty;
            // parse task ID
            Guid Id = ParseTaskMarker(linesEnumerator.Current);

            while (linesEnumerator.MoveNext())
            {
                if (CheckName(linesEnumerator.Current))
                    continue;
                else if (CheckDescription(linesEnumerator.Current))
                    continue;
                else if (CheckTag(linesEnumerator.Current))
                    continue;
                else if (CheckChild(linesEnumerator.Current))
                    continue;
                else if (CheckParent(linesEnumerator.Current))
                    continue;
                else if (CheckDeadline(linesEnumerator.Current))
                    ReadSkipDeadline(linesEnumerator);
                else if (CheckChecklist(linesEnumerator.Current))
                    ReadSkipChecklists(linesEnumerator);
                else
                    break;
            }
            return Id;
        }

        public void ParseTaskIdentitesFromString(HashSet<Guid> identites, IEnumerable<string> stringValues, IEnumerable<PlanumTask> tasks) 
        {
            foreach (var stringValue in stringValues)
            {
                string id;
                string name;
                RepoConfig.ParseTaskName(stringValue, out id, out name);
                var identified = TaskValueParser.ParseIdentity(id, name, tasks);
                foreach (var task in identified)
                    identites.Add(task.Id);
            }
        }

        public void ParseIdentities(IList<PlanumTask> tasks, Dictionary<Guid, IList<string>> children, Dictionary<Guid, IList<string>> parents, Dictionary<Guid, IList<string>> next)
        {
            foreach (var task in tasks)
            {
                if (children.ContainsKey(task.Id))
                    ParseTaskIdentitesFromString(task.Children, children[task.Id], tasks);
                if (parents.ContainsKey(task.Id))
                    ParseTaskIdentitesFromString(task.Parents, parents[task.Id], tasks);
                foreach (var deadline in task.Deadlines)
                {
                    if (next.ContainsKey(deadline.Id))
                        ParseTaskIdentitesFromString(deadline.next, next[deadline.Id], tasks);
                }
            }
        }
    }
}
