using Planum.Model.Repository;
using Planum.Model.Entities;

namespace Planum.Tests;

public class Test_TaskFileManager
{
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

        // Act
        taskFileManager.Restore();

        // Assert
        FileComparator(taskFileManager.FilePath, taskFileManager.BackupPath);
    }

    [Fact]
    public void TestDefaultTaskFileWrite()
    {
        // Arrange
        TaskFileManager taskFileManager = new TaskFileManager();
        string[] testLines = {
            "test 1",
            "test 2",
            "test 3"
        };

        File.WriteAllLines(taskFileManager.BackupPath, testLines);

        // Act
        taskFileManager.Restore();

        // Assert
        FileComparator(taskFileManager.FilePath, taskFileManager.BackupPath);

        // Cleanup
        File.Delete(taskFileManager.FilePath);
        File.Delete(taskFileManager.BackupPath);
    }

    [Fact]
    public void TestTaskFileWriteSingle()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public void TestTaskFileWriteNew()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public void TestTaskFileWriteUpdate()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public void TestTaskFileReadSingle()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public void TestTaskFileReadAll()
    {
        // Arrange

        // Act

        // Assert
    }
}
