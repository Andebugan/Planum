using Planum.Model.Entities;
using Planum.Parser;

namespace PlanumModel_Test.Parsers_Test
{
    public class TaskValueParser_Test
    {
        [Fact]
        public void ParseIdentity_Test()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "TestName";

            var fullMatchTask = new PlanumTask(id, name);
            var idMatchTask = new PlanumTask(id, "name");
            var nameMatchTask = new PlanumTask(Guid.NewGuid(), name);
            var notMatchedTask = new PlanumTask(Guid.NewGuid(), "name");

            IEnumerable<PlanumTask> tasks = new List<PlanumTask> {
                fullMatchTask,
                nameMatchTask,
                idMatchTask,
                notMatchedTask,
            };

            IEnumerable<Guid> expectedTasks = new List<Guid> {
                fullMatchTask.Id,
            };

            Dictionary<Guid, string> tasksDict = new Dictionary<Guid, string>();
            foreach (var task in tasks) tasksDict[task.Id] = task.Name;

            // Act
            var actualTasks = TaskValueParser.ParseIdentity(id.ToString(), name, tasksDict); 

            // Assert
            Assert.Equal(expectedTasks, actualTasks);
        }

        [Theory]
        [InlineData("5 10 1.2:3", 5, 10, 1, 2, 3)]
        [InlineData("10 1.2:3", 0, 10, 1, 2, 3)]
        [InlineData("1.2:3", 0, 0, 1, 2, 3)]
        public void TryParseRepeat_Test(string data, int year, int months, int days, int hours, int minutes)
        {
            // Arrange
            var expectedSpan = new RepeatSpan(year, months, new TimeSpan(days, hours, minutes, 0));

            RepeatSpan actualSpan = new RepeatSpan();

            // Act
            var result = TaskValueParser.TryParseRepeat(ref actualSpan, data);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedSpan, actualSpan);
        }
    }
}
