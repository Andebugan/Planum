using Planum.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Planum.DataModels
{
    public class TaskDTOComparator: ITaskDTOComparator
    {
        public bool CompareDTOs(int firstId, TaskDTO firstDTO, int secondId, TaskDTO secondDTO)
        {
            if (firstId != secondId)
                return false;
            if (firstDTO.UserId != secondDTO.UserId)
                return false;
            if (firstDTO.Name != secondDTO.Name)
                return false;
            if (firstDTO.Description != secondDTO.Description)
                return false;
            if (firstDTO.Timed != secondDTO.Timed)
                return false;
            if (Math.Abs((firstDTO.StartTime - secondDTO.StartTime).TotalSeconds) > 1)
                return false;
            if (Math.Abs((firstDTO.Deadline - secondDTO.Deadline).TotalSeconds) > 1)
                return false;
            List<int> temp_1 = (List<int>)firstDTO.TagIds;
            List<int> temp_2 = (List<int>)secondDTO.TagIds;
            if (!temp_1.SequenceEqual(temp_2))
                return false;
            temp_1 = (List<int>)firstDTO.ChildIds;
            temp_2 = (List<int>)secondDTO.ChildIds;
            if (!temp_1.SequenceEqual(temp_2))
                return false;
            temp_1 = (List<int>)firstDTO.ParentIds;
            temp_2 = (List<int>)secondDTO.ParentIds;
            if (!temp_1.SequenceEqual(temp_2))
                return false;
            if (firstDTO.IsRepeated != secondDTO.IsRepeated)
                return false;
            if (Math.Abs((firstDTO.RepeatPeriod - secondDTO.RepeatPeriod).TotalSeconds) > 1)
                return false;
            return true;
        }

        public bool CompareDTOs(TaskDTO firstDTO, TaskDTO secondDTO)
        {
            if (firstDTO.Id != secondDTO.Id)
                return false;
            if (firstDTO.UserId != secondDTO.UserId)
                return false;
            if (firstDTO.Name != secondDTO.Name)
                return false;
            if (firstDTO.Description != secondDTO.Description)
                return false;
            if (firstDTO.Timed != secondDTO.Timed)
                return false;
            if (Math.Abs((firstDTO.StartTime - secondDTO.StartTime).TotalSeconds) > 1)
                return false;
            if (Math.Abs((firstDTO.Deadline - secondDTO.Deadline).TotalSeconds) > 1)
                return false;
            List<int> temp_1 = (List<int>)firstDTO.TagIds;
            List<int> temp_2 = (List<int>)secondDTO.TagIds;
            if (!temp_1.SequenceEqual(temp_2))
                return false;
            temp_1 = (List<int>)firstDTO.ChildIds;
            temp_2 = (List<int>)secondDTO.ChildIds;
            if (!temp_1.SequenceEqual(temp_2))
                return false;
            temp_1 = (List<int>)firstDTO.ParentIds;
            temp_2 = (List<int>)secondDTO.ParentIds;
            if (!temp_1.SequenceEqual(temp_2))
                return false;
            if (firstDTO.IsRepeated != secondDTO.IsRepeated)
                return false;
            if (Math.Abs((firstDTO.RepeatPeriod - secondDTO.RepeatPeriod).TotalSeconds) > 1)
                return false;
            return true;
        }
    }
}
