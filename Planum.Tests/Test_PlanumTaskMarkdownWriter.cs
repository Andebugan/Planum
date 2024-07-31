using Planum.Config;
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
            PlanumTaskMarkdownWriter writer = new PlanumTaskMarkdownWriter(AppConfig.Load(), repoConfig);

            // Act
            writer.WriteTask(actual, task, tasks);

            // Assert
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected[i], actual[i]);
            }
        }
    }
}
