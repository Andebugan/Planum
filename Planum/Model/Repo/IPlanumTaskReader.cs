using System;
using System.Collections.Generic;
using Planum.Model.Entities;

namespace Planum.Model.Repository
{
    public interface IPlanumTaskReader
    {  
        public Guid ReadTask(IEnumerator<string> linesEnumerator, IList<PlanumTask> tasks, Dictionary<Guid, IList<string>> children, Dictionary<Guid, IList<string>> parents, Dictionary<Guid, IList<string>> next);
        public Guid ReadSkipTask(IEnumerator<string> linesEnumerator);
        public void ParseIdentities(IList<PlanumTask> tasks, Dictionary<Guid, IList<string>> children, Dictionary<Guid, IList<string>> parents, Dictionary<Guid, IList<string>> next);
    }
}
