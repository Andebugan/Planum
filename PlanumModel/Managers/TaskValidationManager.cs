using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Model.Entities;

namespace Planum.Model.Managers
{
    public enum TaskValidationErrorType
    {
        OK,
        CHILD_DOES_NOT_EXIST,
        TASK_CANT_BE_CHILD_OF_ITSELF,
        TASK_CANT_BE_PARENT_OF_ITSELF,
        PARENT_DOES_NOT_EXIST,
        NEXT_DOES_NOT_EXIST
    }

    public struct TaskValidationResult
    {
        public TaskValidationErrorType ErrorType { get; set; } = TaskValidationErrorType.OK;
        public string Message = "";

        public TaskValidationResult(TaskValidationErrorType errorType, string message)
        {
            ErrorType = errorType;
            Message = message;
        }
    }
    
    public class TaskValidationManager
    {
        public TaskValidationManager() { }

        void ValidateChildren(PlanumTask task, IEnumerable<Guid> taskIds, ref List<TaskValidationResult> validationResults)
        {
            if (task.Children.Contains(task.Id))
                validationResults.Add(new TaskValidationResult(TaskValidationErrorType.TASK_CANT_BE_CHILD_OF_ITSELF, $"Task {task.Name} ({task.Id.ToString()}) can't be child of itself"));

            foreach (var child in task.Children)
                if (!taskIds.Contains(child))
                    validationResults.Add(new TaskValidationResult(TaskValidationErrorType.CHILD_DOES_NOT_EXIST, $"Task {task.Name} ({task.Id.ToString()}): child with id: \"{child.ToString()}\" does not exist"));
        }

        void ValidateParents(PlanumTask task, IEnumerable<Guid> taskIds, ref List<TaskValidationResult> validationResults)
        {
            if (task.Parents.Contains(task.Id))
                validationResults.Add(new TaskValidationResult(TaskValidationErrorType.TASK_CANT_BE_PARENT_OF_ITSELF, $"Task {task.Name} ({task.Id.ToString()}) can't be parent of itself"));

            foreach (var parent in task.Parents)
                if (!taskIds.Contains(parent))
                    validationResults.Add(new TaskValidationResult(TaskValidationErrorType.PARENT_DOES_NOT_EXIST, ($"Task {task.Name} ({task.Id.ToString()}): parent with id: \"{parent.ToString()}\" does not exist")));
        }

        void ValidateDeadlines(PlanumTask task, IEnumerable<Guid> taskIds, ref List<TaskValidationResult> validationResults)
        {
            foreach (var deadline in task.Deadlines)
            {
                foreach (var next in deadline.next)
                    if (!taskIds.Contains(next))
                        validationResults.Add(new TaskValidationResult(TaskValidationErrorType.NEXT_DOES_NOT_EXIST, ($"Task {task.Name} ({task.Id.ToString()}), deadline ({deadline.Id}): next with id: \"{next.ToString()}\" does not exist")));
            }
        }
        
        public void ValidateTask(PlanumTask task, IEnumerable<PlanumTask> tasks, ref List<TaskValidationResult> validationResults)
        {
            var taskIds = tasks.Select(x => x.Id);
            ValidateChildren(task, taskIds, ref validationResults);
            ValidateParents(task, taskIds, ref validationResults);
            ValidateDeadlines(task, taskIds, ref validationResults);
        }

        public void ValidateTask(IEnumerable<PlanumTask> tasks, ref List<TaskValidationResult> validationResults)
        {
            foreach (var task in tasks)
                ValidateTask(task, tasks, ref validationResults);
        }
    }
}
