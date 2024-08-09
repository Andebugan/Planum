using System.Collections.Generic;
using System.Linq;

namespace Planum.Repository
{
    public class ReadStatus
    {
        public IList<TaskReadStatus> ReadStatuses { get; set; } = new List<TaskReadStatus>();
        public bool CheckOkStatus() => !ReadStatuses.Where(x => x.Status == TaskReadStatusType.OK).Any();
    }
}
