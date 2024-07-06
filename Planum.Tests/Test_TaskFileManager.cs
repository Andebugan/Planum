using Planum.Model.Repository;
using Planum.Model.Entities;
using Planum.Model.Managers;
using Planum.Config;

namespace Planum.Tests;

public class Test_TaskFileManager
{
    public static IEnumerable<PlanumTask> CreateTestTasks(int taskCount = 3, int childCount = 3, int parentCount = 3, int deadlineCnt = 3)
    {
        List<PlanumTask> testPlanumTaskCollection;
        List<Deadline> deadlines = new List<Deadline>();

        testPlanumTaskCollection = new List<PlanumTask>();

        Guid oldId = Guid.Empty;
        for (int taskNum = 0; taskNum < taskCount; taskNum++)
        {
            deadlines.Clear();
            for (int deadlineNum = 0; deadlineNum < deadlineCnt; deadlineNum++)
            {
                deadlines.Add(new Deadline(
                            enabled: deadlineNum % 2 == 0,
                            deadline: new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0),
                            warningTime: new TimeSpan(1, 0, 0),
                            duration: new TimeSpan(1, 1, 0),
                            repeated: deadlineNum % 2 == 0,
                            repeatSpan: new TimeSpan(2, 1, 0),
                            repeatYears: deadlineNum % 2,
                            repeatMonths: deadlineNum % 2));
            }

            PlanumTask baseTask = new PlanumTask(Guid.NewGuid(),
                name: Guid.NewGuid().ToString(),
                description: Guid.NewGuid().ToString(),
                deadlines: deadlines.ToHashSet());

            for (int childNum = 0; childNum < childCount; childNum++)
            {
                PlanumTask childTask = new PlanumTask(Guid.NewGuid(),
                    name: Guid.NewGuid().ToString(),
                    description: Guid.NewGuid().ToString(),
                    deadlines: deadlines.ToHashSet());
                testPlanumTaskCollection.Add(childTask);
                baseTask.AddChild(childTask.Id);
            }

            for (int parentNum = 0; parentNum < parentCount; parentNum++)
            {
                PlanumTask parentTask = new PlanumTask(Guid.NewGuid(),
                                    name: Guid.NewGuid().ToString(),
                                    description: Guid.NewGuid().ToString(),
                                    deadlines: deadlines.ToHashSet());
                testPlanumTaskCollection.Add(parentTask);
                baseTask.AddParent(parentTask.Id);
            }

            testPlanumTaskCollection.Add(baseTask);
        }
        testPlanumTaskCollection = TaskManager.ValidateRelatives(testPlanumTaskCollection).ToList();
        return testPlanumTaskCollection;
    }

    public static IEnumerable<object[]> TaskTestData()
    {
        IEnumerable<PlanumTask> testTasks = CreateTestTasks(taskCount: 9);

        return new List<object[]>
        {
            new object[] { testTasks.Take(3), "tasks_1.md" },
            new object[] { testTasks.Skip(3).Take(3), "tasks_2.md" },
            new object[] { testTasks.Skip(6).Take(3), "tasks_3.md" }
        };
    }

    bool FileComparator(string pathFirst, string pathSecond)
    {
        var first = File.ReadAllBytes(pathFirst);
        var second = File.ReadAllBytes(pathSecond);
        return first.SequenceEqual(second);
    }

    void TasksAssertEqual(IEnumerable<PlanumTask> original, IEnumerable<PlanumTask> compared)
    {
        Assert.Equal(original.Count(), compared.Count());
        foreach (var task in compared)
        {
            Assert.True(original.Where(x => x.Id == task.Id).Count() == 1);
            var expected = original.Where(x => x.Id == task.Id).First();
            var actual = task;
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Description, actual.Description);
            Assert.True(!expected.Parents.Except(actual.Parents).Any());
            Assert.True(!expected.Children.Except(actual.Children).Any());
            Assert.Equal(expected.Deadlines.Count(), actual.Deadlines.Count());

            foreach (var pair in expected.Deadlines.Zip(actual.Deadlines))
            {
                Assert.Equal(pair.First.enabled, pair.Second.enabled);
                Assert.Equal(pair.First.deadline, pair.Second.deadline);
                Assert.Equal(pair.First.duration, pair.Second.duration);
                Assert.Equal(pair.First.warningTime, pair.Second.warningTime);
                Assert.Equal(pair.First.repeated, pair.Second.repeated);
                Assert.Equal(pair.First.repeatSpan, pair.Second.repeatSpan);
                Assert.Equal(pair.First.repeatYears, pair.Second.repeatYears);
                Assert.Equal(pair.First.repeatMonths, pair.Second.repeatMonths);
            }
        }
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
    public void TestDefaultTaskFileWriteRead()
    {
        // Arrange
        TaskFileManager taskFileManager = new TaskFileManager();
        taskFileManager.Clear();
        var planumTasks = CreateTestTasks();

        // Act
        taskFileManager.Write(planumTasks);
        var tasks = taskFileManager.Read();

        // Assert
        TasksAssertEqual(planumTasks, tasks);

        // Cleanup
        File.Delete(taskFileManager.FilePath);
        File.Delete(taskFileManager.BackupPath);
    }

    [Fact]
    void TestUserTaskFileWriteRead()
    {
        // Arrange
        AppConfig appConfig = ConfigLoader.LoadConfig<AppConfig>(ConfigLoader.AppConfigPath, new AppConfig());
        RepoConfig repoConfig = ConfigLoader.LoadConfig<RepoConfigDto>(appConfig.RepoConfigPath, new RepoConfigDto()).FromDto();

        TaskFileManager taskFileManager = new TaskFileManager();
        taskFileManager.Clear();

        var dirPath = Path.GetDirectoryName(taskFileManager.FilePath);
        if (dirPath is null)
            throw new Exception($"Unable to get task file directory: {taskFileManager.FilePath}");

        List<PlanumTask> planumTasks = CreateTestTasks().ToList();
        var lookupFilenames = new string[] {
            Path.Join(dirPath, "tasks_1.md"),
            Path.Join(dirPath, "tasks_2.md"),
            Path.Join(dirPath, "tasks_3.md")
        };

        foreach (var fname in lookupFilenames)
            File.Create(fname).Close();
        repoConfig.TaskLookupPaths = new Dictionary<string, HashSet<Guid>>();

        for (int i = 0; i < planumTasks.Count(); i++)
        {
            var path = lookupFilenames[i % lookupFilenames.Count()];
            if (!repoConfig.TaskLookupPaths.ContainsKey(path))
                repoConfig.TaskLookupPaths[path] = new HashSet<Guid>();
            repoConfig.TaskLookupPaths[path].Add(planumTasks[i].Id);
        }

        ConfigLoader.SaveConfig<RepoConfig>(appConfig.RepoConfigPath, repoConfig);

        taskFileManager = new TaskFileManager(); // reload after config update
        taskFileManager.Write(planumTasks);
        var tasks = taskFileManager.Read();

        // Act
        taskFileManager.Write(tasks);
        var updatedTasks = taskFileManager.Read();

        // Assert
        TasksAssertEqual(planumTasks, tasks);
        TasksAssertEqual(tasks, updatedTasks);

        // Cleanup
        File.Delete(taskFileManager.FilePath);
        File.Delete(taskFileManager.BackupPath);
        foreach (var filename in lookupFilenames)
            File.Delete(filename);

        repoConfig.TaskLookupPaths = new Dictionary<string, HashSet<Guid>>();
        ConfigLoader.SaveConfig<RepoConfig>(appConfig.RepoConfigPath, repoConfig);
    }

    [Fact]
    void TestUserTaskFileUpdate()
    {
        // Arrange
        AppConfig appConfig = ConfigLoader.LoadConfig<AppConfig>(ConfigLoader.AppConfigPath, new AppConfig());
        RepoConfig repoConfig = ConfigLoader.LoadConfig<RepoConfigDto>(appConfig.RepoConfigPath, new RepoConfigDto()).FromDto();

        TaskFileManager taskFileManager = new TaskFileManager();
        taskFileManager.Clear();

        var dirPath = Path.GetDirectoryName(taskFileManager.FilePath);
        if (dirPath is null)
            throw new Exception($"Unable to get task file directory: {taskFileManager.FilePath}");

        List<PlanumTask> planumTasks = CreateTestTasks().ToList();
        var lookupFilenames = new string[] {
            Path.Join(dirPath, "tasks_1.md"),
            Path.Join(dirPath, "tasks_2.md"),
            Path.Join(dirPath, "tasks_3.md")
        };

        foreach (var fname in lookupFilenames)
            File.Create(fname).Close();
        repoConfig.TaskLookupPaths = new Dictionary<string, HashSet<Guid>>();

        for (int i = 0; i < planumTasks.Count(); i++)
        {
            var path = lookupFilenames[i % lookupFilenames.Count()];
            if (!repoConfig.TaskLookupPaths.ContainsKey(path))
                repoConfig.TaskLookupPaths[path] = new HashSet<Guid>();
            repoConfig.TaskLookupPaths[path].Add(planumTasks[i].Id);
        }

        ConfigLoader.SaveConfig<RepoConfig>(appConfig.RepoConfigPath, repoConfig);

        taskFileManager = new TaskFileManager(); // reload after config update

        // Act
        taskFileManager.Write(planumTasks);
        var tasks = taskFileManager.Read();

        // Assert
        TasksAssertEqual(planumTasks, tasks);

        // Cleanup
        File.Delete(taskFileManager.FilePath);
        File.Delete(taskFileManager.BackupPath);
        foreach (var filename in lookupFilenames)
            File.Delete(filename);

        repoConfig.TaskLookupPaths = new Dictionary<string, HashSet<Guid>>();
        ConfigLoader.SaveConfig<RepoConfig>(appConfig.RepoConfigPath, repoConfig);
    }
}
