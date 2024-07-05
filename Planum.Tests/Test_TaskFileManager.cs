using Planum.Model.Repository;
using Planum.Model.Entities;

namespace Planum.Tests;

public class Test_TaskFileManager
{
    public static IEnumerable<object[]> TaskTestData()
    {
        int relationTasks = 3;
        int relatedTasks = 3;
        int deadlineCnt = 3;

        List<PlanumTask> testEmptyPlanumTaskCollection;
        List<PlanumTask> testOnePlanumTaskCollection;
        List<PlanumTask> testPlanumTaskCollection;

        testEmptyPlanumTaskCollection = new List<PlanumTask>();
        testOnePlanumTaskCollection = new List<PlanumTask> { new PlanumTask(Guid.NewGuid()) };

        testPlanumTaskCollection = new List<PlanumTask>();

        // add empty task
        testPlanumTaskCollection.Add(new PlanumTask(Guid.NewGuid()));
        // add tasks with multiple deadlines
        List<Deadline> deadlines = new List<Deadline>();
        for (int i = 0; i < deadlineCnt; i++)
        {
            deadlines.Clear();
            for (int j = 0; j < deadlineCnt; j++)
            {
                deadlines.Add(new Deadline(
                            enabled: i % 2 == 0,
                            deadline: DateTime.Now,
                            warningTime: new TimeSpan(1, 0, 0),
                            duration: new TimeSpan(1, 1, 0),
                            repeated: i % 2 == 0,
                            repeatSpan: new TimeSpan(1, 1, 1),
                            repeatYears: i % 2,
                            repeatMonths: i % 2));
            }
            testPlanumTaskCollection.Add(new PlanumTask(Guid.NewGuid(),
                    name: Guid.NewGuid().ToString(),
                    description: Guid.NewGuid().ToString(),
                    deadlines: deadlines.ToList()));
        }

        List<PlanumTask> testRelationsTaskCollection = new List<PlanumTask>();
        for (int i = 0; i < relationTasks; i++)
        {
            PlanumTask baseTask = new PlanumTask(Guid.NewGuid());
            if (testRelationsTaskCollection.Any())
                testRelationsTaskCollection.Last().Children = testRelationsTaskCollection.Last().Children.Append(baseTask.Id);
            for (int j = 0; j < relatedTasks; j++)
            {
                PlanumTask childTask = new PlanumTask(Guid.NewGuid());
                PlanumTask parentTask = new PlanumTask(Guid.NewGuid());
                baseTask.Parents = baseTask.Parents.Append(childTask.Id);
                baseTask.Children = baseTask.Children.Append(parentTask.Id);
                testRelationsTaskCollection.Add(childTask);
                testRelationsTaskCollection.Add(parentTask);
            }
        }
        testPlanumTaskCollection.Concat(testRelationsTaskCollection);

        return new List<object[]>
        {
            new object[] { testEmptyPlanumTaskCollection.ToArray() },
            new object[] { testPlanumTaskCollection.ToArray() },
            new object[] { testRelationsTaskCollection.ToArray() }
        };
    }

    bool FileComparator(string pathFirst, string pathSecond)
    {
        var first = File.ReadAllBytes(pathFirst);
        var second = File.ReadAllBytes(pathSecond);

        return first.SequenceEqual(second);
    }

    [Fact]
    public void TestBackup()
    {
        // Arrange
        TaskFileManager taskFileManager = new TaskFileManager();
        string[] testLines = {
            "test 1",
            "test 2",
            "test 3"
        };

        File.WriteAllLines(taskFileManager.FilePath, testLines);

        // Act
        taskFileManager.Backup();

        // Assert
        FileComparator(taskFileManager.FilePath, taskFileManager.BackupPath);

        // Cleanup
        File.Delete(taskFileManager.FilePath);
        File.Delete(taskFileManager.BackupPath);
    }

    [Fact]
    public void TestRestore()
    {
        // Arrange
        TaskFileManager taskFileManager = new TaskFileManager();
        string[] testLines = {
            "test 1",
            "test 2",
            "test 3"
        };

        File.WriteAllLines(taskFileManager.FilePath, testLines);

        // Act
        taskFileManager.Restore();

        // Assert
        FileComparator(taskFileManager.FilePath, taskFileManager.BackupPath);

        // Cleanup
        File.Delete(taskFileManager.FilePath);
        File.Delete(taskFileManager.BackupPath);
    }

    [Theory]
    [MemberData(nameof(TaskTestData))]
    public void TestDefaultTaskFileWrite(IEnumerable<PlanumTask> planumTasks)
    {
        // Arrange
        TaskFileManager taskFileManager = new TaskFileManager();
        taskFileManager.Clear();

        // Act
        taskFileManager.Write(planumTasks);

        // Assert

        // Cleanup
    }

    [Fact]
    public void TestDefaultTaskFileRead()
    {
        // Arrange
        PlanumTask task = new PlanumTask();

        // Act

        // Assert
    }

    [Fact]
    void TestUserTaskFileWrite()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    void TestUserTaskFileRead()
    {
        // Arrange

        // Act

        // Assert
    }
}
