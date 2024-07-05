using Planum.Model.Repository;
using Planum.Model.Entities;

namespace Planum.Tests;

public class Test_TaskFileManager
{
    int relationTasks = 3;
    int relatedTasks = 3;

    List<PlanumTask> testEmptyPlanumTaskCollection;
    List<PlanumTask> testOnePlanumTaskCollection;
    List<PlanumTask> testPlanumTaskCollection;

    public Test_TaskFileManager()
    {
        testEmptyPlanumTaskCollection = new List<PlanumTask>();
        testOnePlanumTaskCollection = new List<PlanumTask> { new PlanumTask(Guid.NewGuid()) };

        testPlanumTaskCollection = new List<PlanumTask>();

        // add empty task
        // add task no deadline
        // add tasks with multiple deadlines

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

    [Fact]
    [InlineData()]
    public void TestDefaultTaskFileWrite(PlanumTask[] tasks)
    {
        // Arrange
        TaskFileManager taskFileManager = new TaskFileManager();

        // Act
        taskFileManager.Restore();

        // Assert
    }

    [Fact]
    public void TestDefaultTaskFileRead()
    {
        // Arrange
        PlanumTask task = new PlanumTask();

        // Act

        // Assert
    }

    [Fact] void TestUserTaskFileWrite()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact] void TestUserTaskFileRead()
    {
        // Arrange

        // Act

        // Assert
    }
}
