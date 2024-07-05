using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Model.Entities;

namespace Planum.Parser {
    public static class TaskValueParser {
        public static IEnumerable<PlanumTask> ParseIdentity(string id, string name, IEnumerable<PlanumTask> taskBuffer) {
            Guid guid = new Guid();
            // try parse guid
            if (ValueParser.Parse(ref guid, id)) {
                return taskBuffer.Where(x => x.Id == guid);
            }

            var tasks = taskBuffer.Where(x => x.Id.ToString().StartsWith(id));
            if (tasks.Any())
                return tasks;

            tasks = taskBuffer.Where(x => x.Name == name);
            if (tasks.Any())
                return tasks;

            tasks = taskBuffer.Where(x => x.Name.StartsWith(name));
            if (tasks.Any())
                return tasks;

            return tasks;
        }
    }
}
