using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Model.Entities;
using Planum.Model.Repository;

namespace Planum.Model.Managers
{
    public class TaskManager
    {
        protected Repo repo;

        public TaskManager()
        {
            repo = new Repo();
        }

        public static IEnumerable<PlanumTask> ValidateRelatives(IEnumerable<PlanumTask> tasks)
        {
            Dictionary<Guid, IEnumerable<Guid>> parentToChildren = new Dictionary<Guid, IEnumerable<Guid>>();
            Dictionary<Guid, IEnumerable<Guid>> childToParents = new Dictionary<Guid, IEnumerable<Guid>>();

            foreach (var task in tasks)
            {
                if (!parentToChildren.ContainsKey(task.Id))
                    parentToChildren[task.Id] = new List<Guid>();
                if (!childToParents.ContainsKey(task.Id))
                    childToParents[task.Id] = new List<Guid>();
                parentToChildren[task.Id] = parentToChildren[task.Id].Concat(task.Children);
                foreach (var parentId in task.Parents)
                    parentToChildren[parentId] = parentToChildren[parentId].Append(task.Id);
                childToParents[task.Id] = childToParents[task.Id].Concat(task.Parents);
                foreach (var childId in task.Children)
                    childToParents[childId] = childToParents[childId].Append(task.Id);
            }

            foreach (var task in tasks)
            {
                task.Children = parentToChildren[task.Id].ToHashSet();
                task.Parents = childToParents[task.Id].ToHashSet();
            }

            return tasks;
        }


        public void Backup()
        {
            repo.Backup();
        }

        public void Restore()
        {
            repo.Restore();
        }

        public void Undo()
        {

        }
    }
}
