using System;
using System.Collections.Generic;
using Planum.Model.Entities;

namespace Planum.Model.Repository
{
    public interface IPlanumTaskReader
    {  
        public void ReadTask(IEnumerator<string> linesEnumeraor, IList<PlanumTask> tasks, Dictionary<Guid, string> children, Dictionary<Guid, string> parents);
        public PlanumTask ParseRelatives(IList<PlanumTask> tasks, Dictionary<Guid, string> children, Dictionary<Guid, string> parents);
    }
}
