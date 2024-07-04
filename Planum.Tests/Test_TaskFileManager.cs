using Planum.Model.Repository;

namespace Planum.Tests;

public class Test_TaskFileManager 
{
    [Fact]
    public void TestBackup()
    {
        // Arrange
        TaskFileManager taskFileManager = new TaskFileManager();
        
        // Act
        taskFileManager.Backup();

        // Assert
    }

    [Fact]
    public void TestRestore()
    {
        // Arrange
        TaskFileManager taskFileManager = new TaskFileManager();
        
        // Act
        taskFileManager.Restore();

        // Assert
    }

    [Fact]
    public void TestDefaultTaskFileWrite()
    {
        // Arrange
        
        // Act

        // Assert
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
}
