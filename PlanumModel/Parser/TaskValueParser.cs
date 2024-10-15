using Planum.Model.Entities;

namespace Planum.Parser
{
    public static class TaskValueParser
    {
        public static IEnumerable<Guid> ParseIdentity(string id, string name, Dictionary<Guid, string> tasks)
        {
            Guid guid = new Guid();
            // try parse guid
            if (ValueParser.TryParse(ref guid, id))
                return tasks.Keys.Where(x => x == guid);

            if (id != string.Empty)
            {
                var idMatches = tasks.Keys.Where(x => x.ToString().StartsWith(id));
                if (idMatches.Any())
                    return idMatches;
            }

            var nameMatches = tasks.Where(x => x.Value == name).ToDictionary();
            if (nameMatches.Any())
                return nameMatches.Keys;

            nameMatches = tasks.Where(x => x.Value.StartsWith(name)).ToDictionary();
            if (nameMatches.Any())
                return nameMatches.Keys;

            return new List<Guid>();
        }

        public static IEnumerable<Guid> ParseIdentity(string id, string name, IEnumerable<PlanumTask> tasks)
        {
            var taskDict = new Dictionary<Guid, string>();
            foreach (var task in tasks)
                taskDict[task.Id] = task.Name;
            return ParseIdentity(id, name, taskDict);
        }

        public static bool TryParseRepeat(ref RepeatSpan repeatSpan, string data)
        {
            data = data.Trim(' ', '\n');
            var split = data.Split(' ').AsEnumerable();
            IEnumerator<string> dataEnumerator = (IEnumerator<string>)split.GetEnumerator();
            dataEnumerator.MoveNext();

            var tmp_months = 0;
            var tmp_years = 0;

            // timespan
            TimeSpan span = TimeSpan.Zero;
            if (ValueParser.TryParse(ref span, dataEnumerator.Current))
            {
                repeatSpan.Span = span;
                if (!dataEnumerator.MoveNext())
                    return true;
            }                        

            // months
            if (int.TryParse(dataEnumerator.Current, out tmp_months))
            {
                repeatSpan.Months = tmp_months;
                if (!dataEnumerator.MoveNext())
                    return true;
            }

            // years
            if (int.TryParse(dataEnumerator.Current, out tmp_years))
            {
                repeatSpan.Years = tmp_years;
                if (!dataEnumerator.MoveNext())
                    return true;
            }
            return true;
        }
    }
}
