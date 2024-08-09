using Planum.Config;
using Planum.Logger;
using Planum.Model.Entities;
using Planum.Repository;

namespace Planum.Tests;

public class Test_PlanumTaskFileManager
{
    bool FileComparator(string pathFirst, string pathSecond)
    {
        var first = File.ReadAllBytes(pathFirst);
        var second = File.ReadAllBytes(pathSecond);
        return first.SequenceEqual(second);
    }

    IEnumerable<PlanumTask> CreateTestTaskFiles(ILoggerWrapper logger, ref RepoConfig repoConfig, out string[] fnames)
    {
        // Arrange
        fnames = new string[] {
            "file_1.md",
            "file_2.md",
            "file_3.md"
        };

        foreach (var fname in fnames)
        {
            if (File.Exists(fname))
                File.Delete(fname);
            File.Create(fname).Close();
        }

        IList<PlanumTask> tasks = new List<PlanumTask>();
        int taskCounter = 0;
        foreach (var fname in fnames)
        {
            repoConfig.TaskLookupPaths[fname] = new HashSet<Guid>();
            for (int i = 0; i < 3; i++)
            {
                var task = new PlanumTask(Guid.NewGuid(), "task_" + taskCounter.ToString(), "task_" + taskCounter.ToString() + " description");
                repoConfig.TaskLookupPaths[fname].Add(task.Id);
                tasks.Add(task);
                taskCounter++;
            }
        }

        return tasks;
    }

    void RemoveFiles(string[] fnames)
    {
        foreach (var fname in fnames)
            if (File.Exists(fname))
                File.Delete(fname);
    }

    [Fact]
    public void TestWriteNewRead()
    {
        ILoggerWrapper logger = new PlanumLogger(LogLevel.DEBUG, clearFile: true);
        RepoConfig repoConfig = RepoConfig.Load(logger);

        PlanumTaskMarkdownWriter writer = new PlanumTaskMarkdownWriter(AppConfig.Load(logger), RepoConfig.Load(logger));
        PlanumTaskMarkdownReader reader = new PlanumTaskMarkdownReader(AppConfig.Load(logger), RepoConfig.Load(logger));
        PlanumTaskFileManager fileManager = new PlanumTaskFileManager(repoConfig, writer, reader, logger);

        TaskFileManagerWriteStatus writeStatus = new TaskFileManagerWriteStatus();
        TaskFileManagerReadStatus readStatus = new TaskFileManagerReadStatus();
        
        string[] fnames;

        var tasks = CreateTestTaskFiles(logger, ref repoConfig, out fnames);

        // Act
        fileManager.Write(tasks, ref writeStatus, ref readStatus);
        var actualTasks = fileManager.Read(ref readStatus);

        // Assert
        foreach (var task in tasks)
            Assert.Contains(task, actualTasks);
        foreach (var task in actualTasks)
            Assert.Contains(task, tasks);
        
        // Cleanup
        RemoveFiles(fnames);
    }

    [Fact]
    public void TestWriteUpdateRead()
    {
        ILoggerWrapper logger = new PlanumLogger(LogLevel.DEBUG, clearFile: true);
        RepoConfig repoConfig = RepoConfig.Load(logger);

        PlanumTaskMarkdownWriter writer = new PlanumTaskMarkdownWriter(AppConfig.Load(logger), RepoConfig.Load(logger));
        PlanumTaskMarkdownReader reader = new PlanumTaskMarkdownReader(AppConfig.Load(logger), RepoConfig.Load(logger));
        PlanumTaskFileManager fileManager = new PlanumTaskFileManager(repoConfig, writer, reader, logger);

        TaskFileManagerWriteStatus writeStatus = new TaskFileManagerWriteStatus();
        TaskFileManagerReadStatus readStatus = new TaskFileManagerReadStatus();
        
        string[] fnames;

        var tasks = CreateTestTaskFiles(logger, ref repoConfig, out fnames);
        fileManager.Write(tasks, ref writeStatus, ref readStatus);
        foreach (var task in tasks)
            task.Description = "new description\\new line";

        // Act
        fileManager.Write(tasks, ref writeStatus, ref readStatus);
        var actualTasks = fileManager.Read(ref readStatus);

        // Assert
        foreach (var task in tasks)
            Assert.Contains(task, actualTasks);
        foreach (var task in actualTasks)
            Assert.Contains(task, tasks);
        
        // Cleanup
        RemoveFiles(fnames);
    }

    [Fact]
    public void TestWriteDeleteRead()
    {
        ILoggerWrapper logger = new PlanumLogger(LogLevel.DEBUG, clearFile: true);
        RepoConfig repoConfig = RepoConfig.Load(logger);

        PlanumTaskMarkdownWriter writer = new PlanumTaskMarkdownWriter(AppConfig.Load(logger), RepoConfig.Load(logger));
        PlanumTaskMarkdownReader reader = new PlanumTaskMarkdownReader(AppConfig.Load(logger), RepoConfig.Load(logger));
        PlanumTaskFileManager fileManager = new PlanumTaskFileManager(repoConfig, writer, reader, logger);

        TaskFileManagerWriteStatus writeStatus = new TaskFileManagerWriteStatus();
        TaskFileManagerReadStatus readStatus = new TaskFileManagerReadStatus();
        
        string[] fnames;

        var tasks = CreateTestTaskFiles(logger, ref repoConfig, out fnames);
        fileManager.Write(tasks, ref writeStatus, ref readStatus);
        tasks = tasks.Take(tasks.Count() / 2);
 
        // Act
        fileManager.Write(tasks, ref writeStatus, ref readStatus);
        var actualTasks = fileManager.Read(ref readStatus);

        // Assert
        foreach (var task in tasks)
            Assert.Contains(task, actualTasks);
        foreach (var task in actualTasks)
            Assert.Contains(task, tasks);
        
        // Cleanup
        RemoveFiles(fnames);
    }
}
