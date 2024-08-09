using Planum.Config;
using Planum.Logger;
using Planum.Model.Entities;
using Planum.Repository;

namespace Planum.Tests
{
    public class Test_PlanumTaskMarkdownWriter
    {
        [Theory]
        [ClassData(typeof(TestMarkdownTaskData))]
        public void TestWriteTask(PlanumTask task, IEnumerable<PlanumTask> tasks, string[] expected, RepoConfig repoConfig)
        {
            // Arrange
            List<string> actual = new List<string>();
            PlanumTaskMarkdownWriter writer = new PlanumTaskMarkdownWriter(AppConfig.Load(new PlanumLogger(LogLevel.INFO, clearFile: true)), repoConfig);

            // Act
            writer.WriteTask(actual, task, tasks);

            // Assert
            for (int i = 0; i < actual.Count(); i++)
            {
                Assert.True(i < expected.Count());
                Assert.Equal(expected[i], actual[i]);
            }

            Assert.Equal(expected.Count(), actual.Count());
        }
    }
}
