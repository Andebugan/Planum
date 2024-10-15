using Planum.Config;
using Planum.Logger;
using Planum.Model;
using Planum.Model.Entities;
using Planum.Model.Exporters;
using Planum.Repository;

namespace PlanumRepo_Test
{
    public class TaskFileManager_TestConfig
    {
        public static ILoggerWrapper Logger = new PlanumLogger(LogLevel.DEBUG, clearFile: true);
        public static ModelConfig ModelConfig = new ModelConfig("PlanumRepo_Test_ModelConfig.json");
        public static RepoConfig RepoConfig = new RepoConfig("PlanumRepo_Test_RepoConfig.json");
        public static TaskMarkdownReader reader = new TaskMarkdownReader(Logger, ModelConfig);
        public static TaskMarkdownExporter exporter = new TaskMarkdownExporter(ModelConfig, Logger);
    }

    public class TaskFileManager_Test : TaskFileManager
    {
        public TaskFileManager_Test() : base(
                TaskFileManager_TestConfig.ModelConfig,
                TaskFileManager_TestConfig.RepoConfig,
                TaskFileManager_TestConfig.reader,
                TaskFileManager_TestConfig.exporter,
                TaskFileManager_TestConfig.Logger)
        { }

        void AssertDeadlines(DeadlineDTO source, DeadlineDTO compared)
        {
            Assert.Equal(source.Id, compared.Id);
            Assert.Equal(source.deadline, compared.deadline);
            Assert.Equal(source.enabled, compared.enabled);
            Assert.Equal(source.warningTime, compared.warningTime);
            Assert.Equal(source.duration, compared.duration);
            Assert.Equal(source.next, compared.next);
            Assert.Equal(source.repeated, compared.repeated);
            Assert.Equal(source.repeatSpan, compared.repeatSpan);
        }

        void AssertTask(PlanumTaskDTO source, PlanumTaskDTO compared)
        {
            Assert.Equal(source.Id, compared.Id);
            Assert.Equal(source.Name, compared.Name);
            Assert.Equal(source.SaveFile, compared.SaveFile);
            Assert.Equal(source.Description, compared.Description);
            Assert.Equal(source.Tags, compared.Tags);
            Assert.Equal(source.Deadlines, compared.Deadlines);
            Assert.Equal(source.Parents, compared.Parents);
            Assert.Equal(source.Children, compared.Children);
        }

        [Fact]
        public void ReadFromFile_Simple_Test()
        {
            // Arrange
            var filepath = "ReadFromFile_Simple_Test.md";

            var firstExpectedTask = new PlanumTaskDTO
            {
                Id = Guid.NewGuid(),
                Name = "First task",
                Description = "Test description",
                SaveFile = filepath,
            };

            var secondExpectedTask = new PlanumTaskDTO
            {
                Id = Guid.NewGuid(),
                Name = "Second task",
                Description = "Test description",
                SaveFile = filepath,
            };

            var thirdExpectedTask = new PlanumTaskDTO
            {
                Id = Guid.NewGuid(),
                Name = "Third task",
                Description = "Test description",
                SaveFile = filepath,
            };

            var lines = new string[] {
                $"<planum:{firstExpectedTask.Id.ToString()}>",
                "- [ ] n: First task",
                "- d: Test description",
                "",
                "Test test test",
                "",
                $"<planum:{secondExpectedTask.Id.ToString()}>",
                "- [ ] n: Second task",
                "- d: Test description",
                "",
               "Test test test",
                "",
                $"<planum:{thirdExpectedTask.Id.ToString()}>",
                "- [ ] n: Third task",
                "- d: Test description",
                ""
            };

            File.WriteAllLines(filepath, lines);

            var tasks = new List<PlanumTaskDTO>();

            // Act
            ReadFromFile(filepath, ref tasks);

            // Assert
            Assert.Single(tasks.Where(x => x.Id == firstExpectedTask.Id));
            Assert.Single(tasks.Where(x => x.Id == secondExpectedTask.Id));
            Assert.Single(tasks.Where(x => x.Id == thirdExpectedTask.Id));

            var firstActualTask = tasks.First(x => x.Id == firstExpectedTask.Id);
            var secondActualTask = tasks.First(x => x.Id == secondExpectedTask.Id);
            var thirdActualTask = tasks.First(x => x.Id == thirdExpectedTask.Id);

            AssertTask(firstExpectedTask, firstActualTask);
            AssertTask(secondExpectedTask, secondActualTask);
            AssertTask(thirdExpectedTask, thirdActualTask);
        }

        [Fact]
        public void ReadFromFile_Relatives_Test()
        {
            // Arrange
            var filepath = "ReadFromFile_Simple_Test.md";

            var parentExpectedTask = new PlanumTaskDTO
            {
                Id = Guid.NewGuid(),
                Name = "parent",
                SaveFile = filepath,
            };

            var expectedTask = new PlanumTaskDTO
            {
                Id = Guid.NewGuid(),
                Name = "task",
                SaveFile = filepath,
            };

            var childExpectedTask = new PlanumTaskDTO
            {
                Id = Guid.NewGuid(),
                Name = "child",
                SaveFile = filepath,
            };

            parentExpectedTask.Children.Add(new Tuple<string, string>("", expectedTask.Name));

            expectedTask.Parents.Add(new Tuple<string, string>("", parentExpectedTask.Name));
            expectedTask.Children.Add(new Tuple<string, string>("", childExpectedTask.Name));

            childExpectedTask.Parents.Add(new Tuple<string, string>("", expectedTask.Name));

            var lines = new string[] {
                $"<planum:{parentExpectedTask.Id.ToString()}>",
                "- [ ] n: parent",
                "- [ ] c: task",
                "",
                "Test test test",
                "",
                $"<planum:{expectedTask.Id.ToString()}>",
                "- [ ] n: task",
                "- [ ] p: parent",
                "- [ ] c: child",
                "",
               "Test test test",
                "",
                $"<planum:{childExpectedTask.Id.ToString()}>",
                "- [ ] n: child",
                "- [ ] p: task",
                ""
            };

            File.WriteAllLines(filepath, lines);

            var tasks = new List<PlanumTaskDTO>();

            // Act
            ReadFromFile(filepath, ref tasks);

            // Assert
            Assert.Single(tasks.Where(x => x.Id == parentExpectedTask.Id));
            Assert.Single(tasks.Where(x => x.Id == expectedTask.Id));
            Assert.Single(tasks.Where(x => x.Id == childExpectedTask.Id));

            var parentActualTask = tasks.First(x => x.Id == parentExpectedTask.Id);
            var actualTask = tasks.First(x => x.Id == expectedTask.Id);
            var childActualTask = tasks.First(x => x.Id == childExpectedTask.Id);

            AssertTask(parentExpectedTask, parentActualTask);
            AssertTask(expectedTask, actualTask);
            AssertTask(childExpectedTask, childActualTask);
        }

        HashSet<string> GenerateTestDirectory(string startPath, List<PlanumTask> tasks)
        {
            HashSet<string> paths = new HashSet<string>();
            if (Directory.Exists(startPath))
                Directory.Delete(startPath, true);

            var path = startPath;
            Directory.CreateDirectory(path);
            var filepath = "";

            for (int i = 0; i < tasks.Count(); i++)
            {
                path = Path.Join(path, $"{i}");
                filepath = Path.Join(path, $"file_{i}.md");
                Directory.CreateDirectory(path);
                paths.Add(Path.GetRelativePath(startPath, new FileInfo(filepath).FullName));
                tasks[i].SaveFile = new FileInfo(filepath).FullName;
            }

            foreach (var task in tasks)
            {
                var lines = new List<string>();
                TaskFileManager_TestConfig.exporter.WriteTask(lines, task, tasks);
                File.WriteAllLines(task.SaveFile, lines);
            }

            return paths;
        }

        [Fact]
        public void SearchForMarkdownFiles_Test()
        {
            // Arrange
            var startPath = "SearchForMarkdownFiles_dir";
            var parentId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var childId = Guid.NewGuid();

            var tasks = new List<PlanumTask>
            {
                new PlanumTask
                {
                    Id = parentId,
                    Name = "parent",
                    Children = new HashSet<Guid> { taskId }
                },
                new PlanumTask
                {
                    Id = taskId,
                    Name = "task",
                    Parents = new HashSet<Guid> { parentId },
                    Children = new HashSet<Guid> { childId }
                },
                new PlanumTask
                {
                    Id = childId,
                    Name = "child",
                    Parents = new HashSet<Guid> { taskId }
                },
            };

            HashSet<string> expectedPaths = GenerateTestDirectory(startPath, tasks);

            // Act
            var actualPaths = SearchForMarkdownFiles(startPath, new HashSet<string>());

            // Assert
            Assert.Equal(expectedPaths, actualPaths.Select(x => Path.GetRelativePath(startPath, x)));
        }

        [Fact]
        public void Read_Test()
        {
            // Arrange
            var startPath = "Read_Test_dir";
            var parentId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var childId = Guid.NewGuid();

            TaskFileManager_TestConfig.RepoConfig.TaskLookupPaths.Add("Read_Test_dir");

            var expectedTasks = new List<PlanumTask>
            {
                new PlanumTask
                {
                    Id = parentId,
                    Name = "parent",
                    Children = new HashSet<Guid> { taskId }
                },
                new PlanumTask
                {
                    Id = taskId,
                    Name = "task",
                    Parents = new HashSet<Guid> { parentId },
                    Children = new HashSet<Guid> { childId }
                },
                new PlanumTask
                {
                    Id = childId,
                    Name = "child",
                    Parents = new HashSet<Guid> { taskId }
                },
            };

            GenerateTestDirectory(startPath, expectedTasks);

            // Act
            var actualTasks = Read();

            // Assert
            Assert.Equal(expectedTasks, actualTasks);

            // Clean
            TaskFileManager_TestConfig.RepoConfig.TaskLookupPaths.Clear();
        }

        [Fact]
        public void WriteToFile_Test()
        {
            // Arrange
            var filepath = Path.GetFullPath("WriteToFile_Test.md");
            if (File.Exists(filepath))
                File.Delete(filepath);

            var parentId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var newTaskId = Guid.NewGuid();
            var childId = Guid.NewGuid();

            var parent = new PlanumTask
            {
                Id = parentId,
                Name = "parent",
                Children = new HashSet<Guid> { taskId },
                SaveFile = filepath
            };
            var task = new PlanumTask
            {
                Id = taskId,
                Name = "task",
                Parents = new HashSet<Guid> { parentId },
                Children = new HashSet<Guid> { childId },
                SaveFile = filepath
            };
            var child = new PlanumTask
            {
                Id = childId,
                Name = "child",
                Parents = new HashSet<Guid> { taskId },
                SaveFile = filepath
            };
            var newTask = new PlanumTask
            {
                Id = newTaskId,
                Name = "new task",
                SaveFile = filepath
            };

            var tasks = new List<PlanumTask>
            {
                parent,
                task,
                child,
            };

            var lines = new List<string>();
            TaskFileManager_TestConfig.exporter.WriteTask(lines, parent, tasks);
            TaskFileManager_TestConfig.exporter.WriteTask(lines, task, tasks);
            TaskFileManager_TestConfig.exporter.WriteTask(lines, child, tasks);
            File.WriteAllLines(filepath, lines);

            parent.Name = "New parent";
            task.Description = "New task description";
            task.Children.Remove(childId);

            tasks = new List<PlanumTask>()
            {
                parent,
                task,
                newTask
            };
            
            Dictionary<string, IEnumerable<string>> fileLines = new Dictionary<string, IEnumerable<string>>();
            var expectedLines = new List<string>
            {
                $"<planum:{parent.Id.ToString()}>",
                $"- [ ] n: New parent [->]({Path.GetFullPath(filepath)})",
                $"- [ ] c: task [->]({Path.GetFullPath(filepath)})",
                "",
                $"<planum:{task.Id.ToString()}>",
                $"- [ ] n: task [->]({Path.GetFullPath(filepath)})",
                "- d: New task description",
                $"- [ ] p: New parent [->]({Path.GetFullPath(filepath)})",
                "",
                $"<planum:{newTask.Id.ToString()}>",
                $"- [ ] n: new task [->]({Path.GetFullPath(filepath)})",
                ""
            };

            // Act
            WriteToFile(filepath, tasks, ref fileLines);

            // Assert
            Assert.Contains(filepath, fileLines.Keys);
            foreach (var linePair in expectedLines.Zip(fileLines[filepath]))
                Assert.Equal(linePair.First, linePair.Second);

            // Clean
            TaskFileManager_TestConfig.RepoConfig.TaskLookupPaths.Clear();
        }

        [Fact]
        public void Write_Test()
        {
            // Arrange
            var filepath = Path.GetFullPath("Write_Test.md");
            if (File.Exists(filepath))
                File.Delete(filepath);
            File.Create(filepath).Close();

            TaskFileManager_TestConfig.RepoConfig.TaskLookupPaths.Add("./");

            var parentId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var newTaskId = Guid.NewGuid();
            var childId = Guid.NewGuid();

            var parent = new PlanumTask
            {
                Id = parentId,
                Name = "parent",
                Children = new HashSet<Guid> { taskId },
                SaveFile = filepath
            };
            var task = new PlanumTask
            {
                Id = taskId,
                Name = "task",
                Parents = new HashSet<Guid> { parentId },
                Children = new HashSet<Guid> { childId },
                SaveFile = filepath
            };
            var newTask = new PlanumTask
            {
                Id = newTaskId,
                Name = "new task",
                SaveFile = filepath
            };

            parent.Name = "New parent";
            task.Description = "New task description";
            task.Children.Remove(childId);

            var tasks = new List<PlanumTask>()
            {
                parent,
                task,
                newTask
            };
            
            Dictionary<string, IEnumerable<string>> fileLines = new Dictionary<string, IEnumerable<string>>();
            var expectedLines = new List<string>
            {
                $"<planum:{parent.Id.ToString()}>",
                $"- [ ] n: New parent [->]({Path.GetFullPath(filepath)})",
                $"- [ ] c: task [->]({Path.GetFullPath(filepath)})",
                "",
                $"<planum:{task.Id.ToString()}>",
                $"- [ ] n: task [->]({Path.GetFullPath(filepath)})",
                "- d: New task description",
                $"- [ ] p: New parent [->]({Path.GetFullPath(filepath)})",
                "",
                $"<planum:{newTask.Id.ToString()}>",
                $"- [ ] n: new task [->]({Path.GetFullPath(filepath)})",
                ""
            };

            // Act
            Write(tasks);

            // Assert
            var actualLines = File.ReadAllLines(filepath);
            foreach (var linePair in expectedLines.Zip(actualLines))
                Assert.Equal(linePair.First, linePair.Second);

            // Clean
            TaskFileManager_TestConfig.RepoConfig.TaskLookupPaths.Clear();
        }
    }
}
