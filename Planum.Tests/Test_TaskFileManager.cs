using Planum.Model.Repository;
using Planum.Model.Entities;
using Planum.Config;

namespace Planum.Tests;

public class Test_PlanumTaskFileManager
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
        testPlanumTaskCollection = PlanumTask.FillRelatives(testPlanumTaskCollection).ToList();
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

    [Fact]
    public void TestBackup()
    {
        /*
        // Arrange
        PlanumTaskFileManager planumTaskFileManager = new PlanumTaskFileManager();
        string[] testLines = {
            "test 1",
            "test 2",
            "test 3"
        };

        File.WriteAllLines(planumTaskFileManager.FilePath, testLines);

        // Act
        planumTaskFileManager.Backup();

        // Assert
        FileComparator(planumTaskFileManager.FilePath, planumTaskFileManager.BackupPath);

        // Cleanup
        File.Delete(planumTaskFileManager.FilePath);
        File.Delete(planumTaskFileManager.BackupPath);
        */
    }

    [Fact]
    public void TestRestore()
    {
        /*
        // Arrange
        PlanumTaskFileManager planumTaskFileManager = new PlanumTaskFileManager();
        string[] testLines = {
            "test 1",
            "test 2",
            "test 3"
        };

        File.WriteAllLines(planumTaskFileManager.FilePath, testLines);

        // Act
        planumTaskFileManager.Restore();

        // Assert
        FileComparator(planumTaskFileManager.FilePath, planumTaskFileManager.BackupPath);

        // Cleanup
        File.Delete(planumTaskFileManager.FilePath);
        File.Delete(planumTaskFileManager.BackupPath);
        */
    }

    [Fact]
    public void TestDefaultTaskFileWriteRead()
    {
        /*
        // Arrange
        PlanumTaskFileManager planumTaskFileManager = new PlanumTaskFileManager();
        planumTaskFileManager.Clear();
        var planumTasks = CreateTestTasks();

        // Act
        planumTaskFileManager.Write(planumTasks);
        var tasks = planumTaskFileManager.Read();

        // Assert
        TasksAssertEqual(planumTasks, tasks);

        // Cleanup
        File.Delete(planumTaskFileManager.FilePath);
        File.Delete(planumTaskFileManager.BackupPath);
        */
    }

    [Fact]
    public void TestUserTaskFileWriteRead()
    {
        /*
        // Arrange
        AppConfig appConfig = ConfigLoader.LoadConfig<AppConfig>(ConfigLoader.AppConfigPath, new AppConfig());
        RepoConfig repoConfig = ConfigLoader.LoadConfig<RepoConfig>(appConfig.RepoConfigPath, new RepoConfig());

        PlanumTaskFileManager planumTaskFileManager = new PlanumTaskFileManager();
        planumTaskFileManager.Clear();

        var dirPath = Path.GetDirectoryName(planumTaskFileManager.FilePath);
        if (dirPath is null)
            throw new Exception($"Unable to get task file directory: {planumTaskFileManager.FilePath}");

        List<PlanumTask> planumTasks = CreateTestTasks().ToList();
        var lookupFilenames = new string[] {
            Path.Join(dirPath, "tasks_1.md"),
            Path.Join(dirPath, "tasks_2.md"),
            Path.Join(dirPath, "tasks_3.md")
        };

        foreach (var fname in lookupFilenames)
            File.Create(fname).Close();
        repoConfig.TaskLookupPaths = new Dictionary<string, IEnumerable<Guid>>();

        for (int i = 0; i < planumTasks.Count(); i++)
        {
            var path = lookupFilenames[i % lookupFilenames.Count()];
            if (!repoConfig.TaskLookupPaths.ContainsKey(path))
                repoConfig.TaskLookupPaths[path] = new HashSet<Guid>();
            repoConfig.TaskLookupPaths[path] = repoConfig.TaskLookupPaths[path].Append(planumTasks[i].Id);
        }

        ConfigLoader.SaveConfig<RepoConfig>(appConfig.RepoConfigPath, repoConfig);

        planumTaskFileManager = new PlanumTaskFileManager(); // reload after config update
        planumTaskFileManager.Write(planumTasks);
        var tasks = planumTaskFileManager.Read();

        // Act
        planumTaskFileManager.Write(tasks);
        var updatedTasks = planumTaskFileManager.Read();

        // Assert
        TasksAssertEqual(planumTasks, tasks);
        TasksAssertEqual(tasks, updatedTasks);

        // Cleanup
        File.Delete(planumTaskFileManager.FilePath);
        File.Delete(planumTaskFileManager.BackupPath);
        foreach (var filename in lookupFilenames)
            File.Delete(filename);

        repoConfig.TaskLookupPaths = new Dictionary<string, IEnumerable<Guid>>();
        ConfigLoader.SaveConfig<RepoConfig>(appConfig.RepoConfigPath, repoConfig);
        */
    }

    [Fact]
    public void TestUserTaskFileUpdate()
    {
        /*
        // Arrange
        AppConfig appConfig = ConfigLoader.LoadConfig<AppConfig>(ConfigLoader.AppConfigPath, new AppConfig());
        RepoConfig repoConfig = ConfigLoader.LoadConfig<RepoConfig>(appConfig.RepoConfigPath, new RepoConfig());

        PlanumTaskFileManager planumTaskFileManager = new PlanumTaskFileManager();
        planumTaskFileManager.Clear();

        var dirPath = Path.GetDirectoryName(planumTaskFileManager.FilePath);
        if (dirPath is null)
            throw new Exception($"Unable to get task file directory: {planumTaskFileManager.FilePath}");

        List<PlanumTask> planumTasks = CreateTestTasks().ToList();
        var lookupFilenames = new string[] {
            Path.Join(dirPath, "tasks_1.md"),
            Path.Join(dirPath, "tasks_2.md"),
            Path.Join(dirPath, "tasks_3.md")
        };

        foreach (var fname in lookupFilenames)
            File.Create(fname).Close();
        repoConfig.TaskLookupPaths = new Dictionary<string, IEnumerable<Guid>>();

        for (int i = 0; i < planumTasks.Count(); i++)
        {
            var path = lookupFilenames[i % lookupFilenames.Count()];
            if (!repoConfig.TaskLookupPaths.ContainsKey(path))
                repoConfig.TaskLookupPaths[path] = new HashSet<Guid>();
            repoConfig.TaskLookupPaths[path] = repoConfig.TaskLookupPaths[path].Append(planumTasks[i].Id);
        }

        ConfigLoader.SaveConfig<RepoConfig>(appConfig.RepoConfigPath, repoConfig);

        planumTaskFileManager = new PlanumTaskFileManager(); // reload after config update

        // Act
        planumTaskFileManager.Write(planumTasks);
        var tasks = planumTaskFileManager.Read();

        // Assert
        TasksAssertEqual(planumTasks, tasks);

        // Cleanup
        File.Delete(planumTaskFileManager.FilePath);
        File.Delete(planumTaskFileManager.BackupPath);
        foreach (var filename in lookupFilenames)
            File.Delete(filename);

        repoConfig.TaskLookupPaths = new Dictionary<string, IEnumerable<Guid>>();
        ConfigLoader.SaveConfig<RepoConfig>(appConfig.RepoConfigPath, repoConfig);
        */
    }
}
