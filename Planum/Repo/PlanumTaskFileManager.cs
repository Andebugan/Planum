﻿using Planum.Config;
using Planum.Model.Entities;
using Planum.Model.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#nullable enable

namespace Planum.Repository
{
    /*
     * <planum:guid>
     * - [ ] n(ame): {string}
     * - t: {string} (custom user tag)
     * - d(escription): {string}
     * - [ ] p(arent): [{string} | [guid]](path/to/task/file) (name or/and guid if name was not provided or name is not unique)
     * ...
     * - [ ] c(hildren): [{string} | [guid]](path/to/task/file) (name or/and guid if name was not provided or name is not unique)
     * ...
     * - [ ] D(eadline): ([x] - disabled/complete, otherwise enabled with status)
     *     - d(eadline): {hh:mm dd.mm.yyyy}
     *     - w(arning): {dd.hh.mm}
     *     - du(ration): {dd.hh.mm}
     *     - [ ] r(epeat duration): {y m d.hh:mm}
     *     - [ ] n(ext): [{string} | [guid]](path/to/task/file) (name or/and guid if name was not provided or name is not unique), counts as child
     * ...
     * - [ ] {string}
     *     - t(ag): {string} (custom user tag)
     *     - d(escription): ...
     *     - [ ] D(eadline)
     *        - ...
     *        - [ ] {string}
     *        - ...
     * ...
     *
    */
    public class PlanumTaskFileManager : IPlanumTaskFileManager
    {
        RepoConfig RepoConfig { get; set; }
        IPlanumTaskWriter PlanumTaskWriter { get; set; }
        IPlanumTaskReader PlanumTaskReader { get; set; }

        public PlanumTaskFileManager(RepoConfig repoConfig, IPlanumTaskWriter planumTaskWriter, IPlanumTaskReader planumTaskReader)
        {
            RepoConfig = repoConfig;
            PlanumTaskWriter = planumTaskWriter;
            PlanumTaskReader = planumTaskReader;
        }

        public void ReadFromFile(string path, IList<PlanumTask> tasks, Dictionary<Guid, IList<string>> children, Dictionary<Guid, IList<string>> parents, Dictionary<Guid, IList<string>> next)
        {
            if (!File.Exists(path))
                throw new Exception($"File doesn't exist at path: \"{path}\"");
            IEnumerator<string> linesEnumerator = (IEnumerator<string>)File.ReadAllLines(path).GetEnumerator();
            while (linesEnumerator.MoveNext());
                PlanumTaskReader.ReadTask(linesEnumerator, tasks, children, parents, next);
        }

        public IEnumerable<PlanumTask> Read()
        {
            IList<PlanumTask> tasks = new List<PlanumTask>();
            Dictionary<Guid, IList<string>> children = new Dictionary<Guid, IList<string>>();
            Dictionary<Guid, IList<string>> parents = new Dictionary<Guid, IList<string>>();
            Dictionary<Guid, IList<string>> next = new Dictionary<Guid, IList<string>>();

            foreach (var path in RepoConfig.TaskLookupPaths.Keys)
                ReadFromFile(path, tasks, children, parents, next);
            PlanumTaskReader.ParseIdentities(tasks, children, parents, next);

            return tasks;
        }

        public void WriteToFile(string path, IEnumerable<PlanumTask> tasks, bool create = false)
        {
            if (!File.Exists(path))
                if (create)
                    File.Create(path).Close();
                else
                    RepoConfig.TaskLookupPaths.Remove(path);

            IEnumerable<Guid> taskIds = tasks.Select(x => x.Id);
            List<Guid> writtenIds = new List<Guid>();

            IEnumerable<string> lines = File.ReadAllLines(path);
            List<string> newLines = new List<string>();
            IEnumerator<string> linesEnumerator = (IEnumerator<string>)lines.GetEnumerator();

            while (linesEnumerator.MoveNext())
            {
                // check if matches with planum header
                if (linesEnumerator.Current.StartsWith(RepoConfig.TaskMarkerStartSymbol) && linesEnumerator.Current.EndsWith(RepoConfig.TaskMarkerEndSymbol))
                {
                    var taskId = PlanumTaskReader.ReadSkipTask(linesEnumerator);
                    // if not in tasks - ignore
                    if (taskIds.Contains(taskId))
                    {
                        PlanumTask newTask = tasks.Where(x => x.Id == taskId).First();
                        writtenIds.Add(taskId);
                        PlanumTaskWriter.WriteTask(newLines, newTask, tasks);
                    }
                }
                else
                    newLines.Add(linesEnumerator.Current);
            }

            RepoConfig.TaskLookupPaths[path] = writtenIds;

            File.WriteAllLines(path, newLines);
        }

        public void Write(IEnumerable<PlanumTask> tasks, bool create = false)
        {
            foreach (var filepath in RepoConfig.TaskLookupPaths.Keys)
                WriteToFile(filepath, tasks.Where(x => RepoConfig.TaskLookupPaths[filepath].Contains(x.Id)), create);
            RepoConfig.Save();
        }
    }
}
