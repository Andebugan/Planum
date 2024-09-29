using System.Collections;
using Planum.Logger;
using Planum.Model;
using Planum.Model.Entities;
using Planum.Model.Exporters;

namespace PlanumModel_Test.Exporters_Test
{
    public class TaskMarkdownExporter_Test
    {
        class TaskMarkdownExporter_TestData : IEnumerable<object[]>
        {
            public object[] GenerateNameTask()
            {
                PlanumTask task = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName" 
                };

                var lines = new List<string>
                {
                    $"<planum:{task.Id.ToString()}>",
                    $"- [ ] n: {task.Name}",
                    "",
                };

                var tasks = new List<PlanumTask>
                {
                    task
                };

                return new object[] {lines, task, PlanumTask.UpdateRelatives(tasks)};
            }

            public object[] GenerateNameDescriptionTagsTask()
            {
                PlanumTask task = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName", 
                    Description = "Test Description",
                    Tags = new HashSet<string>()
                    {
                        "TestTag1",
                        "TestTag2"
                    }
                };

                var lines = new List<string>
                {
                    $"<planum:{task.Id.ToString()}>",
                    $"- [ ] n: {task.Name}",
                    $"- t: {string.Join(", ", task.Tags)}",
                    $"- d: {task.Description}",
                    "",
                };

                var tasks = new List<PlanumTask>
                {
                    task
                };

                return new object[] {lines, task, PlanumTask.UpdateRelatives(tasks)};
            }

            public object[] GenerateParentChildrenCheckists()
            {
                PlanumTask parentTask = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "ParentTask",
                };

                PlanumTask childTask = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "ChildTask",
                };

                PlanumTask checklistBaseTask = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Tags = new HashSet<string> { DefaultTags.Checklist },
                    Name = "ChecklistBaseTask"
                };

                PlanumTask checklistFirstLevelTask = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Tags = new HashSet<string> { DefaultTags.Checklist },
                    Name = "ChecklistFirstLevelTask",
                    Parents = new HashSet<Guid> { checklistBaseTask.Id }
                };

                PlanumTask task = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName",
                    Parents = new HashSet<Guid> { parentTask.Id },
                    Children = new HashSet<Guid> { childTask.Id, checklistBaseTask.Id }
                };

                var lines = new List<string>
                {
                    $"<planum:{task.Id.ToString()}>",
                    $"- [ ] n: {task.Name}",
                    $"- [ ] p: [{parentTask.Name}]({parentTask.SaveFile})",
                    $"- [ ] c: [{childTask.Name}]({childTask.SaveFile})",
                    $"- [ ] {checklistBaseTask.Name}",
                    $"  - [ ] {checklistFirstLevelTask.Name}",
                    "",
                };

                var tasks = new List<PlanumTask>
                {
                    task,
                    checklistBaseTask,
                    checklistFirstLevelTask,
                    parentTask,
                    childTask
                };

                return new object[] {lines, task, PlanumTask.UpdateRelatives(tasks)};
            }

            public object[] GenerateCompleteDeadlineTask()
            {
                var deadline = new Deadline {
                    enabled = false,
                    deadline = DateTime.Now.AddDays(5)
                };
               
                PlanumTask task = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName",
                    Deadlines = new HashSet<Deadline> 
                    {
                        deadline
                    }
                };

                var lines = new List<string>
                {
                    $"<planum:{task.Id.ToString()}>",
                    $"- [x] n: {task.Name}",
                    $"- [x] D: {deadline.deadline.ToString("H:m d.M.y")} | {deadline.Id}",
                    "",
                };

                var tasks = new List<PlanumTask>
                {
                    task
                };

                return new object[] {lines, task, PlanumTask.UpdateRelatives(tasks)};
            }

            public object[] GenerateNotStartedDeadlineTask()
            {
                var deadline = new Deadline {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(5),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                };
               
                PlanumTask task = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName",
                    Deadlines = new HashSet<Deadline> 
                    {
                        deadline
                    }
                };

                var lines = new List<string>
                {
                    $"<planum:{task.Id.ToString()}>",
                    $"- [ ] n: {task.Name}",
                    $"- [ ] D: {deadline.deadline.ToString("H:m d.M.y")} | {deadline.Id}",
                    $"  - w: {deadline.warningTime.ToString(@"d\.h\:m")}",
                    $"  - d: {deadline.duration.ToString(@"d\.h\:m")}",
                    "",
                };

                var tasks = new List<PlanumTask>
                {
                    task
                };

                return new object[] {lines, task, PlanumTask.UpdateRelatives(tasks)};
            } 

            public object[] GenerateWarningDeadlineTask()
            {
                var deadline = new Deadline {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(3),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                };
               
                PlanumTask task = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName",
                    Deadlines = new HashSet<Deadline> 
                    {
                        deadline
                    }
                };

                var lines = new List<string>
                {
                    $"<planum:{task.Id.ToString()}>",
                    $"- [w] n: {task.Name}",
                    $"- [w] D: {deadline.deadline.ToString("H:m d.M.y")} | {deadline.Id}",
                    $"  - w: {deadline.warningTime.ToString(@"d\.h\:m")}",
                    $"  - d: {deadline.duration.ToString(@"d\.h\:m")}",
                    "",
                };

                var tasks = new List<PlanumTask>
                {
                    task
                };

                return new object[] {lines, task, PlanumTask.UpdateRelatives(tasks)};
            }

            public object[] GenerateInProgressDeadlineTask()
            {
                var deadline = new Deadline {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(1),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                };
               
                PlanumTask task = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName",
                    Deadlines = new HashSet<Deadline> 
                    {
                        deadline
                    }
                };

                var lines = new List<string>
                {
                    $"<planum:{task.Id.ToString()}>",
                    $"- [I] n: {task.Name}",
                    $"- [I] D: {deadline.deadline.ToString("H:m d.M.y")} | {deadline.Id}",
                    $"  - w: {deadline.warningTime.ToString(@"d\.h\:m")}",
                    $"  - d: {deadline.duration.ToString(@"d\.h\:m")}",
                    "",
                };

                var tasks = new List<PlanumTask>
                {
                    task
                };

                return new object[] {lines, task, PlanumTask.UpdateRelatives(tasks)};
            }

            public object[] GenerateOverdueDeadlineTask()
            {
                var deadline = new Deadline {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(-1),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                };
               
                PlanumTask task = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName",
                    Deadlines = new HashSet<Deadline> 
                    {
                        deadline
                    }
                };

                var lines = new List<string>
                {
                    $"<planum:{task.Id.ToString()}>",
                    $"- [O] n: {task.Name}",
                    $"- [O] D: {deadline.deadline.ToString("H:m d.M.y")} | {deadline.Id}",
                    $"  - w: {deadline.warningTime.ToString(@"d\.h\:m")}",
                    $"  - d: {deadline.duration.ToString(@"d\.h\:m")}",
                    "",
                };

                var tasks = new List<PlanumTask>
                {
                    task
                };

                return new object[] {lines, task, PlanumTask.UpdateRelatives(tasks)};
            }

            public object[] GenerateRepeatedTask()
            {
                var deadline = new Deadline {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(5),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                    repeatSpan = new TimeSpan(1, 2, 3, 0),
                    repeatMonths = 1,
                    repeatYears = 2,
                    repeated = true
                };
               
                PlanumTask task = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName",
                    Deadlines = new HashSet<Deadline> 
                    {
                        deadline
                    }
                };

                var lines = new List<string>
                {
                    $"<planum:{task.Id.ToString()}>",
                    $"- [ ] n: {task.Name}",
                    $"- [ ] D: {deadline.deadline.ToString("H:m d.M.y")} | {deadline.Id}",
                    $"  - w: {deadline.warningTime.ToString(@"d\.h\:m")}",
                    $"  - d: {deadline.duration.ToString(@"d\.h\:m")}",
                    $"  - [x] r: 2 1 1.2:3", 
                    "",
                };

                var tasks = new List<PlanumTask>
                {
                    task
                };

                return new object[] {lines, task, PlanumTask.UpdateRelatives(tasks)};
            }

            public object[] GenerateNextDeadlineTask()
            {
                PlanumTask nextTask = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "NextName",
                };

                var deadline = new Deadline {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(5),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                    next = new HashSet<Guid> { nextTask.Id }
                };

                PlanumTask task = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName",
                    Deadlines = new HashSet<Deadline> 
                    {
                        deadline
                    }
                };

                var lines = new List<string>
                {
                    $"<planum:{task.Id.ToString()}>",
                    $"- [ ] n: {task.Name}",
                    $"- [ ] D: {deadline.deadline.ToString("H:m d.M.y")} | {deadline.Id}",
                    $"  - w: {deadline.warningTime.ToString(@"d\.h\:m")}",
                    $"  - d: {deadline.duration.ToString(@"d\.h\:m")}",
                    $"  - [ ] n: [{nextTask.Name}]({nextTask.SaveFile})",
                    "",
                };

                var tasks = new List<PlanumTask>
                {
                    task,
                    nextTask
                };

                return new object[] {lines, task, PlanumTask.UpdateRelatives(tasks)};
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return GenerateNameTask();
                yield return GenerateNameDescriptionTagsTask();
                yield return GenerateParentChildrenCheckists();

                yield return GenerateCompleteDeadlineTask();
                yield return GenerateNotStartedDeadlineTask();
                yield return GenerateWarningDeadlineTask();
                yield return GenerateInProgressDeadlineTask();
                yield return GenerateOverdueDeadlineTask();
                yield return GenerateRepeatedTask();
                yield return GenerateNextDeadlineTask();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TaskMarkdownExporter_TestData))]
        public void WriteTask_Test(List<string> expectedLines, PlanumTask task, IEnumerable<PlanumTask> tasks)
        {
            // Arrange
            ILoggerWrapper logger = new PlanumLogger();
            ModelConfig config = new ModelConfig("");
            TaskMarkdownExporter exporter = new TaskMarkdownExporter(config, logger);

            List<string> actualLines = new List<string>();

            // Act
            exporter.WriteTask(actualLines, task, tasks);

            // Assert
            Assert.Equal(expectedLines, actualLines);
        }
    }
}
