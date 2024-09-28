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
            public IEnumerator<object[]> GetEnumerator()
            {
                PlanumTask parentTask = new PlanumTask(Guid.NewGuid(), "ParentTask");
                PlanumTask childTask = new PlanumTask(Guid.NewGuid(), "ChildTask");
                PlanumTask nextTask = new PlanumTask(Guid.NewGuid(), "ChildTask");

                PlanumTask task = new PlanumTask
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName"
                };
                yield return new object[]
                {
                    new List<string>
                    {
                        $"<planum:{task.Id.ToString()}>",
                        $"- [ ] n: {task.Name}",
                        "",
                    },
                    task,
                    new List<PlanumTask>
                    {
                        task
                    }
                };
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
