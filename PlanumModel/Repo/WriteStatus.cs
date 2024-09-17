using System.Collections.Generic;
using System.Linq;

namespace Planum.Repository
{
    public class WriteStatus
    {
        public IList<TaskWriteStatus> WriteStatuses { get; set; } = new List<TaskWriteStatus>();
        public bool CheckOkStatus() => !WriteStatuses.Where(x => x.Status != TaskWriteStatusType.OK).Any();
    }
}
