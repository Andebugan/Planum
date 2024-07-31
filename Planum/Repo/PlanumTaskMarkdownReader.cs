using System;
using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Entities;
using Planum.Model.Repository;
using Planum.Parser;

namespace Planum.Repository
{
    public class PlanumTaskMarkdownReader : IPlanumTaskReader
    {
        AppConfig AppConfig { get; set; }
        RepoConfig RepoConfig { get; set; }

        public PlanumTaskMarkdownReader(AppConfig appConfig, RepoConfig repoConfig)
        {
            AppConfig = appConfig;
            RepoConfig = repoConfig;
        }

        protected bool CheckTaskMarker(string line) => line.StartsWith(RepoConfig.TaskMarkerStartSymbol) && line.EndsWith(RepoConfig.TaskMarkerEndSymbol);
        protected Guid ParseTaskMarker(string line)
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

        protected bool CheckName(string line)
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

        protected string ParseTaskName(string line, ref bool complete)
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

        protected bool CheckTag(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskTagSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        protected string ParseTaskTag(string line, int level = 0) => line.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskTagSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol).Length);

        protected bool CheckDescription(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDescriptionSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        protected string ParseTaskDescription(string line, int level = 0) => line.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDescriptionSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol).Length);

        protected bool CheckParent(string line)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(RepoConfig.TaskItemSymbol +
                                    statusMarker +
                                    RepoConfig.TaskParentSymbol +
                                    RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }
        protected string ParseParentString(string line) => RepoConfig.ParseMarkdownLink(line.Remove(0, (RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskDummyMarkerSymbol + 
                        RepoConfig.TaskParentSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol).Length));

        protected bool CheckChild(string line)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(RepoConfig.TaskItemSymbol +
                                    statusMarker +
                                    RepoConfig.TaskChildSymbol +
                                    RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }
        protected string ParseChildString(string line) => RepoConfig.ParseMarkdownLink(line.Remove(0, (RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskDummyMarkerSymbol + 
                        RepoConfig.TaskChildSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol).Length));

        protected string AddLineTabs(int level = 0)
        {
            var tabs = "";
            for (int i = 0; i < level; i++)
                tabs += RepoConfig.TaskItemTabSymbol;
            return tabs;
        }

        protected bool CheckChecklist(string line, int level = 0)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(AddLineTabs(level) +
                            RepoConfig.TaskItemSymbol +
                            statusMarker))
                    return true;
            return false;
        }
        protected void ParseChecklist(ref bool moveNext, Guid taskId, IEnumerator<string> linesEnumerator, IList<PlanumTask> tasks, Dictionary<Guid, IList<string>> next, int level = 0)
        {
            var checklistTask = new PlanumTask(id: Guid.NewGuid());
            checklistTask.Parents.Add(taskId);
            checklistTask.Tags.Add(DefaultTags.Checklist);

            // parse checklist name
            if (linesEnumerator.Current.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskCompleteMarkerSymbol))
                                checklistTask.Tags.Add(DefaultTags.Complete);

            checklistTask.Name = linesEnumerator.Current.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskCompleteMarkerSymbol).Length);

            moveNext = linesEnumerator.MoveNext();

            // finish parsing checklist + recursive part
            while (moveNext)
            {
                // parse description
                if (CheckDescription(linesEnumerator.Current, level + 1))
                {
                    checklistTask.Description = ParseTaskDescription(linesEnumerator.Current, level + 1);
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse tag
                else if (CheckTag(linesEnumerator.Current))
                {
                    checklistTask.Tags.Add(ParseTaskTag(linesEnumerator.Current, level + 1));
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse deadline
                else if (CheckDeadline(linesEnumerator.Current, level + 1))
                {
                    checklistTask.Deadlines.Add(ParseDeadline(ref moveNext, linesEnumerator, next, level + 1));
                }
                // parse checklist
                else if (CheckChecklist(linesEnumerator.Current, level + 1))
                {
                    ParseChecklist(ref moveNext, checklistTask.Id, linesEnumerator, tasks, next, level + 1);
                }
                else
                {
                    moveNext = true;
                    return;
                }
            }
        }

        protected bool CheckDeadline(string line, int level = 0)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(RepoConfig.TaskItemSymbol +
                                    statusMarker +
                                    RepoConfig.TaskDeadlineHeaderSymbol +
                                    RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }

        protected bool CheckDeadlineDate(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDeadlineSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        protected DateTime ParseDeadlineDate(string line, int level = 0)
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

        protected bool CheckWarningTime(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskWarningTimeSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        protected TimeSpan ParseWarningTime(string line, int level = 0)
        {
            line = line.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskWarningTimeSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol).Length);
            TimeSpan warningTime = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref warningTime, line))
                throw new Exception($"Unable to parse warning time");
            return warningTime;
        }

        protected bool CheckDuration(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDurationTimeSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        protected TimeSpan ParseDuration(string line, int level = 0)
        {
            line = line.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDurationTimeSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol).Length);
            TimeSpan duration = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref duration, line))
                throw new Exception($"Unable to parse duration");
            return duration;
        }

        protected bool CheckRepeated(string line, int level = 0)
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
        protected void ParseRepeat(string line, ref Deadline deadline, int level = 0)
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

        protected bool CheckNext(string line, int level = 0)
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
        protected string ParseNextTaskString(string line, int level = 0) => RepoConfig.ParseMarkdownLink(line.Remove(0, (AddLineTabs(level) +
                    RepoConfig.TaskItemSymbol +
                    RepoConfig.TaskDummyMarkerSymbol + 
                    RepoConfig.TaskNextSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol).Length));

        protected Deadline ParseDeadline(ref bool moveNext, IEnumerator<string> linesEnumerator, Dictionary<Guid, IList<string>> next, int level = 0)
        {
            Deadline deadline = new Deadline();

            // parse header
            var line = linesEnumerator.Current.Remove(0, (AddLineTabs(level) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskDummyMarkerSymbol +
                        RepoConfig.TaskDeadlineHeaderSymbol+
                        RepoConfig.TaskHeaderDelimeterSymbol).Length).Trim(' ', '\n');

            if (linesEnumerator.Current.StartsWith(AddLineTabs(level) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskCompleteMarkerSymbol +
                        RepoConfig.TaskDeadlineHeaderSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol))
                deadline.enabled = false;
            else
                deadline.enabled = true;

            var deadlineId = Guid.NewGuid();

            if (line != string.Empty && !ValueParser.TryParse(ref deadlineId, line))
                throw new Exception("Unable to parse deadline id");
            deadline.Id = deadlineId;

            moveNext = linesEnumerator.MoveNext();

            while (moveNext)
            {
                // parse deadline
                if (CheckDeadlineDate(linesEnumerator.Current, level + 1))
                {
                    deadline.deadline = ParseDeadlineDate(linesEnumerator.Current, level + 1);
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse warning
                else if (CheckWarningTime(linesEnumerator.Current, level + 1))
                {
                    deadline.warningTime = ParseWarningTime(linesEnumerator.Current, level + 1);
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse duration
                else if (CheckDuration(linesEnumerator.Current, level + 1))
                {
                    deadline.duration = ParseDuration(linesEnumerator.Current, level + 1);
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse repeat duration
                else if (CheckRepeated(linesEnumerator.Current, level + 1))
                {
                    ParseRepeat(linesEnumerator.Current, ref deadline, level + 1);
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse next tasks 
                else if (CheckNext(linesEnumerator.Current, level + 1))
                {
                    if (!next.ContainsKey(deadline.Id))
                        next[deadline.Id] = new List<string>();
                    next[deadline.Id].Add(ParseNextTaskString(linesEnumerator.Current, level + 1));
                    moveNext = linesEnumerator.MoveNext();
                }
                else
                {
                    moveNext = true;
                    break;
                }
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
            bool moveNext = linesEnumerator.MoveNext();

            while (moveNext)
            {
                // parse name
                if (CheckName(linesEnumerator.Current))
                {
                    var complete = false;
                    task.Name = ParseTaskName(linesEnumerator.Current, ref complete);
                    if (complete)
                        task.Tags.Add(DefaultTags.Complete);
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse tag
                else if (CheckTag(linesEnumerator.Current))
                {
                    task.Tags.Add(ParseTaskTag(linesEnumerator.Current));
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse description
                else if (CheckDescription(linesEnumerator.Current))
                {
                    task.Description = ParseTaskDescription(linesEnumerator.Current);
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse child
                else if (CheckChild(linesEnumerator.Current))
                {
                    if (!children.ContainsKey(task.Id))
                        children[task.Id] = new List<string>();
                    children[task.Id].Add(ParseChildString(linesEnumerator.Current));
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse parent
                else if (CheckParent(linesEnumerator.Current))
                {
                    if (!parents.ContainsKey(task.Id))
                        parents[task.Id] = new List<string>();
                    parents[task.Id].Add(ParseParentString(linesEnumerator.Current));
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse deadlines
                else if (CheckDeadline(linesEnumerator.Current))
                {
                    task.Deadlines.Add(ParseDeadline(ref moveNext, linesEnumerator, next, 0));
                }
                // parse checklists
                else if (CheckChecklist(linesEnumerator.Current))
                {
                    ParseChecklist(ref moveNext, task.Id, linesEnumerator, tasks, next, 0);
                }
                else
                    break;
            }
            tasks.Add(task);
            return task.Id;
        }

        protected void ReadSkipDeadline(IEnumerator<string> linesEnumerator, int level = 0)
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

        protected void ReadSkipChecklists(IEnumerator<string> linesEnumerator, int level = 0)
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

        protected void ParseTaskIdentitesFromString(HashSet<Guid> identites, IEnumerable<string> stringValues, IEnumerable<PlanumTask> tasks) 
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
