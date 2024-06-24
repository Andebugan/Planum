using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Model.Entities;

namespace Planum.Parser {
    public static class TaskValueParser {
        public static bool ParseIdentity(ref IEnumerable<Task> value, string data, IEnumerable<Task> taskBuffer) {
            Guid guid = new Guid();
            // try parse guid
            if (ValueParser.Parse(ref guid, data)) {
                value = taskBuffer.Where(x => x.Id == guid);
                if (value.Any())
                    return true;
                return false;
            }

            value = taskBuffer.Where(x => x.Id.ToString().StartsWith(data));
            if (value.Any())
                return true;

            value = taskBuffer.Where(x => x.Name == data);
            if (value.Any())
                return true;

            value = taskBuffer.Where(x => x.Name.StartsWith(data));
            if (value.Any())
                return true;

            return false;
        }
    }
}
