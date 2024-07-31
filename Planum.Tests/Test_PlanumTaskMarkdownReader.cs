using Planum.Config;
using Planum.Model.Entities;
using Planum.Repository;

namespace Planum.Tests
{
    public class Test_TaskFileReader
    {
        [Theory]
        [ClassData(typeof(TestMarkdownTaskData))]
        public void TestReadTask(PlanumTask task, IList<PlanumTask> tasks, IEnumerable<string> lines, RepoConfig repoConfig)
        {
            // Arrange
            IEnumerator<string> linesEnumerator = (IEnumerator<string>)lines.GetEnumerator();
            linesEnumerator.MoveNext();

            PlanumTaskMarkdownReader reader = new PlanumTaskMarkdownReader(AppConfig.Load(), repoConfig);
            Dictionary<Guid, IList<string>> children = new Dictionary<Guid, IList<string>>();
            Dictionary<Guid, IList<string>> parents = new Dictionary<Guid, IList<string>>();
            Dictionary<Guid, IList<string>> next = new Dictionary<Guid, IList<string>>();

            List<PlanumTask> tasksActual = new List<PlanumTask>();
            tasks = tasks.Where(x => x.Id != task.Id).ToList();

            // Act
            var id = reader.ReadTask(linesEnumerator, tasksActual, children, parents, next);
            tasksActual = tasks.Concat(tasksActual).ToList();
            reader.ParseIdentities(tasksActual, children, parents, next);
            tasksActual = PlanumTask.UpdateRelatives(tasksActual).ToList();

            // Assert
            tasks.Add(task);
            Assert.Equal(tasks.Count(), tasksActual.Count());

            foreach (var expectedTask in tasks)
            {
                Assert.True(tasksActual.Exists(x => x.Id == expectedTask.Id));
                var actualTask = tasksActual.Where(x => x.Id == expectedTask.Id).First();

                Assert.Equal(expectedTask.Id, actualTask.Id);
                Assert.Equal(expectedTask.Name, actualTask.Name);
                Assert.Equal(expectedTask.Description, actualTask.Description);
                CompareHashSets<string>(expectedTask.Tags, actualTask.Tags);
                CompareHashSets<Guid>(expectedTask.Parents, actualTask.Parents);
                CompareHashSets<Guid>(expectedTask.Children, actualTask.Children);
                Assert.Equal(expectedTask.Deadlines.Count(), actualTask.Deadlines.Count());

                foreach (var deadlinePair in task.Deadlines.Zip(actualTask.Deadlines))
                {
                    Assert.Equal(deadlinePair.First.Id, deadlinePair.Second.Id);
                    Assert.Equal(deadlinePair.First.enabled, deadlinePair.Second.enabled);
                    Assert.Equal(deadlinePair.First.deadline, deadlinePair.Second.deadline);
                    Assert.Equal(deadlinePair.First.warningTime, deadlinePair.Second.warningTime);
                    Assert.Equal(deadlinePair.First.duration, deadlinePair.Second.duration);
                    Assert.Equal(deadlinePair.First.repeated, deadlinePair.Second.repeated);
                    Assert.Equal(deadlinePair.First.repeatSpan, deadlinePair.Second.repeatSpan);
                    Assert.Equal(deadlinePair.First.repeatYears, deadlinePair.Second.repeatYears);
                    Assert.Equal(deadlinePair.First.repeatMonths, deadlinePair.Second.repeatMonths);
                    CompareHashSets(deadlinePair.First.next, deadlinePair.Second.next);
                }
            }
        }

        void CompareHashSets<T>(HashSet<T> first, HashSet<T> second)
        {
            var firstSorted = new SortedSet<T>(first);
            var secondSorted = new SortedSet<T>(second);
            
            Assert.Equal(first.Count(), second.Count());

            foreach (var pair in firstSorted.Zip(secondSorted))
                Assert.Equal(pair.First, pair.Second);
        }
    }
}
