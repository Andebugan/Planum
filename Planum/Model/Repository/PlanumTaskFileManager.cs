using Planum.Config;
using Planum.Model.Entities;
using Planum.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#nullable enable

namespace Planum.Model.Repository
{
    /*
     * <planum:guid>
     * - [ ] n(ame): {string}
     * - d(escription): {string}
     * - [ ] p(arent): [{string} | [guid]](path/to/task/file) (name or/and guid if name was not provided or name is not unique)
     * ...
     * - [ ] c(hildren): [{string} | [guid]](path/to/task/file) (name or/and guid if name was not provided or name is not unique)
     * ...
     * - [ ] D(eadline): ([x] - disabled/complete, [ ] - enabled)
     *     - d(adline]: {hh:mm dd.mm.yyyy}
     *     - w(arning]: {dd.hh.mm}
     *     - du(ration]: {dd.hh.mm}
     *     - [ ] r(epeat duration): {y m d.hh:mm}
     *     - [ ] p(ipeline node): [{string} | [guid]](path/to/task/file) => [{string} | [guid]](path/to/task/file) (name or/and guid if name was not provided or name is not unique)
     * ...
     * - [ ] {string} [| {guid}] (name or/and guid if name was not provided or name is not unique) (level 1 checklist)
     *     - d(escription)
     *     - [ ] D(eadline)
     *        - ...
     *        - [ ] {string} [| {guid}] (name or/and guid if name was not provided or name is not unique) (level 1 checklist)
     *        - ...
     * ...
     * <- ends with empty line/<planum> for next task after it
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

        public IEnumerable<PlanumTask> ReadFromFile(string path, IEnumerable<PlanumTask> tasks)
        {
            return new PlanumTask[] { };
        }

        public IEnumerable<PlanumTask> Read()
        {
            IList<PlanumTask> tasks = new List<PlanumTask>();
            foreach (var path in RepoConfig.TaskLookupPaths.Keys)
            {
                
            }

            return tasks;
        }

        protected void WriteToFile(string path, IEnumerable<PlanumTask> tasks, bool create = false)
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
                    var task = PlanumTaskReader.ReadTask(linesEnumerator, tasks);
                    // if not in tasks - ignore
                    if (taskIds.Contains(task.Id))
                    {
                        PlanumTask newTask = tasks.Where(x => x.Id == task.Id).First();
                        writtenIds.Add(task.Id);
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
