using System.Collections;
using Planum.Logger;
using Planum.Model;
using Planum.Model.Entities;
using Planum.Repository;

namespace PlanumRepo_Test
{
    public class TaskMarkdownReader_Test : TaskMarkdownReader
    {
        public TaskMarkdownReader_Test() : base(new PlanumLogger(), new ModelConfig("")) { }

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

        void AssertTasks(PlanumTaskDTO source, PlanumTaskDTO compared)
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

        [Theory]
        [ClassData(typeof(TryParseTaskMarker_TestData))]
        public void TryParseTaskMarker_Test(string line, Guid taskId, Guid expected, bool expectedResult)
        {
            // Arrange
            var actual = new PlanumTaskDTO();
            actual.Id = taskId;

            // Act
            var actualResult = TryParseTaskMarker(line, ref actual);

            // Assert
            Assert.Equal(expectedResult, actualResult);
            if (actualResult)
                Assert.Equal(expected, actual.Id);
        }

        class TryParseTaskMarker_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "test", Guid.Empty, Guid.Empty, false};
                yield return new object[] { "<planum:>", Guid.Empty, Guid.Empty, true};
                yield return new object[] { "<planum:test>", Guid.Empty, Guid.Empty,  false};
                var id = Guid.NewGuid();
                yield return new object[] { $"<planum:{id.ToString()}>", Guid.Empty, id, true};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [InlineData("test", 0, "", false)]
        [InlineData("- [ ] n: TestName", 0, "TestName", true)]
        [InlineData("  - [ ] n: TestName", 1, "TestName", true)]
        [InlineData("- [w] n: TestName", 0, "TestName", true)]
        [InlineData("- [I] n: TestName", 0, "TestName", true)]
        [InlineData("- [O] n: TestName", 0, "TestName", true)]
        [InlineData("- [x] n: TestName", 0, "TestName", true)]
        public void TryParseName_Test(string line, int currentLevel, string expected, bool expectedResult)
        {
            // Arrange
            PlanumTaskDTO parsedTask = new PlanumTaskDTO();

            // Act
            var result = TryParseTaskName(line, ref parsedTask, currentLevel);

            // Assert
            Assert.Equal(expectedResult, result);
            if (result)
                Assert.Equal(expected, parsedTask.Name);
        }

        [Theory]
        [InlineData("test", 0, new string [] {}, false)]
        [InlineData("- t: Tag1", 0, new string[] { "Tag1" }, true)]
        [InlineData("  - t: Tag1", 1, new string[] { "Tag1" }, true)]
        [InlineData("- t: Tag1, Tag2, Tag3", 0, new string[] { "Tag1", "Tag2", "Tag3" }, true)]
        public void TryParseTags_Test(string line, int currentLevel, string[] expected, bool expectedResult)
        {
            // Arrange
            PlanumTaskDTO parsedTask = new PlanumTaskDTO();

            // Act
            var result = TryParseTags(line, ref parsedTask, currentLevel);

            // Assert
            Assert.Equal(expectedResult, result);
            if (result)
                Assert.Equal(expected.ToHashSet(), parsedTask.Tags);
        }
        
        [Theory]
        [InlineData("test", 0, "", false)]
        [InlineData("- d: Test description", 0, "Test description", true)]
        [InlineData("  - d: Test description", 1, "Test description", true)]
        public void TryParse_Description_Test(string line, int currentLevel, string expected, bool expectedResult)
        {
            // Arrange
            PlanumTaskDTO parsedTask = new PlanumTaskDTO();

            // Act
            var result = TryParseDescription(line, ref parsedTask, currentLevel);

            // Assert
            Assert.Equal(expectedResult, result);
            if (result)
                Assert.Equal(expected, parsedTask.Description);
        }

        [Theory]
        [ClassData(typeof(TryParseTaskReference_TestData))]
        public void TryParseTaskReference_Test(string line, string symbol, int currentLevel, string expectedId, string expectedName, bool expectedResult)
        {
            // Arrange
            HashSet<Tuple<string, string>> actual = new HashSet<Tuple<string, string>>();

            // Act
            var result = TryParseTaskReference(line, ref actual, symbol, currentLevel);

            // Assert
            Assert.Equal(expectedResult, result);
            if (result)
            {
                Assert.Equal(expectedId, actual.First().Item1);
                Assert.Equal(expectedName, actual.First().Item2);
            }
        }

        class TryParseTaskReference_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                ModelConfig config = new ModelConfig("");

                yield return new object[] { "test", "", 0, "", "", false };
                var guid = Guid.NewGuid();
                yield return new object[] { "- [ ] p: TestParent [->](test_path)", config.TaskParentSymbol, 0, "", "TestParent", true };
                yield return new object[] { "  - [ ] p: TestParent [->](test_path)", config.TaskParentSymbol, 1, "", "TestParent", true };
                yield return new object[] { $"- [ ] p: TestParent | {guid.ToString()} [->](test_path)", config.TaskParentSymbol, 0, guid.ToString(), "TestParent", true };

                yield return new object[] { "- [ ] c: TestChild [->](test_path)", config.TaskChildSymbol, 0, "", "TestChild", true };
                yield return new object[] { "  - [ ] c: TestChild [->](test_path)", config.TaskChildSymbol, 1, "", "TestChild", true };
                yield return new object[] { $"- [ ] c: TestChild | {guid.ToString()} [->](test_path)", config.TaskChildSymbol, 0, guid.ToString(), "TestChild", true };

                yield return new object[] { "- [ ] n: TestNext [->](test_path)", config.TaskNextSymbol, 0, "", "TestNext", true };
                yield return new object[] { "  - [ ] n: TestNext [->](test_path)", config.TaskNextSymbol, 1, "", "TestNext", true };
                yield return new object[] { $"- [ ] n: TestNext | {guid.ToString()} [->](test_path)", config.TaskNextSymbol, 0, guid.ToString(), "TestNext", true };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TryParseDeadlineHeader_TestData))]
        public void TryParseDeadlineHeader_Test(string line, int currentLevel, Guid expectedId, DateTime expectedDate)
        {
            // Arrange
            var actual = new DeadlineDTO();
            actual.Id = expectedId;
            actual.deadline = DateTime.Today;

            // Act
            ParseDeadlineHeader(ref actual, line, currentLevel);

            // Assert
            Assert.Equal(expectedId, actual.Id);
            Assert.Equal(expectedDate, actual.deadline);
        }

        class TryParseDeadlineHeader_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "- [ ] D: ", 0, Guid.NewGuid(), DateTime.Today };
                yield return new object[] { "- [ ] D: 12:00", 0, Guid.NewGuid(), DateTime.Today.AddHours(12) };
                yield return new object[] { "- [ ] D: 1.10.24", 0, Guid.NewGuid(), new DateTime(2024, 10, 1, 0, 0, 0) };
                yield return new object[] { "  - [ ] D: 1.10.24", 1, Guid.NewGuid(), new DateTime(2024, 10, 1, 0, 0, 0) };
                yield return new object[] { "- [ ] D: 12:00 1.10.24", 0, Guid.NewGuid(), new DateTime(2024, 10, 1, 12, 0, 0) };
                var id = Guid.NewGuid();
                yield return new object[] { $"- [ ] D: 12:00 1.10.24 | {id.ToString()}", 0, id, new DateTime(2024, 10, 1, 12, 0, 0) };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TryParse_TimeSpan_TestData))]
        public void TryParse_TimeSpan_Test(string line, string symbol, int currentLevel, int days, int hours, int minutes, bool expectedResult)
        {
            // Arrange
            var expected = new TimeSpan(days, hours, minutes, 0);
            TimeSpan actual = TimeSpan.Zero;

            // Act
            var result = TryParseTimeSpanItem(ref actual, line, symbol, currentLevel);

            // Assert
            Assert.Equal(expectedResult, result);
            if (result)
                Assert.Equal(expected, actual);
        }

        class TryParse_TimeSpan_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                ModelConfig config = new ModelConfig("");
                yield return new object[] { "test", "", 0, 0, 12, 0, false};
                yield return new object[] { "- w: 12:00", config.TaskWarningMarkerSymbol, 0, 0, 12, 0, true};
                yield return new object[] { "  - w: 12:00", config.TaskWarningMarkerSymbol, 1, 0, 12, 0, true};
                yield return new object[] { "- w: 10.12:30", config.TaskWarningMarkerSymbol, 0, 10, 12, 30, true};
                yield return new object[] { "- w: 10.12", config.TaskWarningMarkerSymbol, 0, 10, 12, 0, true};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TryParseRepeatSpan_TestData))]
        public void TryParseRepeatSpan_Test(string line, int currentLevel, string symbol, RepeatSpan expectedSpan, bool expectedEnabled, bool expectedResult)
        {
            // Arrange
            RepeatSpan actualSpan = new RepeatSpan();
            bool actualEnabled = false;

            // Act
            var result = TryParseRepeatSpan(ref actualSpan, ref actualEnabled, line, symbol, currentLevel);

            // Assert
            Assert.Equal(expectedResult, result);
            if (result)
            {
                Assert.Equal(expectedSpan, actualSpan);
                Assert.Equal(expectedEnabled, actualEnabled);
            }
        }

        class TryParseRepeatSpan_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                ModelConfig config = new ModelConfig("");
                yield return new object[] { "test", 0, "", new RepeatSpan(), false, false };
                yield return new object[] { "- [ ] r: 12:00", 0, config.TaskRepeatTimeSymbol, new RepeatSpan(0, 0, new TimeSpan(0, 12, 0, 0)), false, true };
                yield return new object[] { "- [x] r: 12:00", 0, config.TaskRepeatTimeSymbol, new RepeatSpan(0, 0, new TimeSpan(0, 12, 0, 0)), true, true };
                yield return new object[] { "  - [ ] r: 12:00", 1, config.TaskRepeatTimeSymbol, new RepeatSpan(0, 0, new TimeSpan(0, 12, 0, 0)), false, true };
                yield return new object[] { "- [ ] r: 12:00 1", 0, config.TaskRepeatTimeSymbol, new RepeatSpan(0, 1, new TimeSpan(0, 12, 0, 0)), false, true };
                yield return new object[] { "- [ ] r: 12:00 1 2", 0, config.TaskRepeatTimeSymbol, new RepeatSpan(2, 1, new TimeSpan(0, 12, 0, 0)), false, true };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Fact]
        public void TryParseDeadline_Test()
        {
            // Arrange
            DeadlineDTO expectedDeadline = new DeadlineDTO();
            expectedDeadline.Id = Guid.NewGuid();
            expectedDeadline.enabled = true;
            expectedDeadline.deadline = new DateTime(2024, 10, 5, 12, 30, 0);
            expectedDeadline.warningTime = new TimeSpan(5, 12, 30, 0);
            expectedDeadline.duration = new TimeSpan(5, 12, 30, 0);
            expectedDeadline.repeated = true;
            expectedDeadline.repeatSpan = new RepeatSpan(5, 4, new TimeSpan(1, 2, 3, 0));

            var lines = new string[] {
                $"- [ ] D: 12:30 5.10.24 | {expectedDeadline.Id}",
                "  - w: 5.12:30",
                "  - d: 5.12:30",
                "  - [x] r: 1.2:3 4 5", 
            };
            IEnumerator<string> enumerator = lines.ToList().GetEnumerator();
            enumerator.MoveNext();

            PlanumTaskDTO task = new PlanumTaskDTO();

            // Act
            var result = TryParseDeadline(ref enumerator, ref task, 0);

            // Assert
            Assert.True(result);
            var actualDeadline = task.Deadlines.First();
            AssertDeadlines(expectedDeadline, actualDeadline);
        }

        [Fact]
        public void TryParseChecklist_Test()
        {
            // Arrange
            DeadlineDTO deadline = new DeadlineDTO
            {
                Id = Guid.NewGuid(),
                deadline = new DateTime(2024, 10, 1, 12, 0, 0),
                enabled = false
            };

            var lines = new string[] {
                "- [ ] TestChecklist",
                "  - d: Test checklist description",
                $"  - [x] D: 12:00 1.10.24 | {deadline.Id.ToString()}",
                "  - [ ] TestInternalChecklist"
            };
            IEnumerator<string> enumerator = lines.ToList().GetEnumerator();
            enumerator.MoveNext();

            PlanumTaskDTO actualTask = new PlanumTaskDTO();
            List<PlanumTaskDTO> actualTasks = new List<PlanumTaskDTO>();

            // Act
            var result = TryParseChecklist(ref enumerator, ref actualTask, ref actualTasks, 0);

            // Assert
            Assert.Single(actualTasks.Where(x => x.Name == "TestChecklist"));
            Assert.Single(actualTasks.Where(x => x.Name == "TestInternalChecklist"));

            var testChecklistTask = actualTasks.First(x => x.Name == "TestChecklist");
            var internalChecklistTask = actualTasks.Find(x => x.Name == "TestInternalChecklist");

            Assert.NotNull(testChecklistTask);
            Assert.NotNull(internalChecklistTask);

            Assert.Contains(DefaultTags.Checklist, testChecklistTask.Tags);
            Assert.Contains(DefaultTags.Checklist, internalChecklistTask.Tags);

            Assert.Contains(new Tuple<string, string>(testChecklistTask.Id.ToString(), testChecklistTask.Name), actualTask.Children);
            Assert.Contains(new Tuple<string, string>(internalChecklistTask.Id.ToString(), internalChecklistTask.Name), testChecklistTask.Children);

            Assert.Equal("TestChecklist", testChecklistTask.Name);
            Assert.Equal("Test checklist description", testChecklistTask.Description);
            Assert.Contains(deadline, testChecklistTask.Deadlines);

            Assert.Equal("TestInternalChecklist", internalChecklistTask.Name);
        }

        [Fact]
        public void ReadTask_Test()
        {
            DeadlineDTO deadline = new DeadlineDTO
            {
                Id = Guid.NewGuid(),
                deadline = new DateTime(2024, 10, 1, 12, 0, 0),
                enabled = false
            };

            var taskId = Guid.NewGuid();

            var lines = new string[] {
                $"<planum:{taskId}>",
                "- [ ] n: Test name",
                "- [ ] p: Test parent",
                "- [ ] c: Test child",
                "- d: Test description",
                $"- [x] D: 12:00 1.10.24 | {deadline.Id.ToString()}",
                "- [ ] TestChecklist",
            };
            IEnumerator<string> enumerator = lines.ToList().GetEnumerator();
            enumerator.MoveNext();

            List<PlanumTaskDTO> actualTasks = new List<PlanumTaskDTO>();

            // Act
            var actualId = ReadTask(ref enumerator, ref actualTasks);

            // Assert
            var task = actualTasks.First(x => x.Id == taskId);
            Assert.NotNull(task);
            Assert.Equal("Test name", task.Name);
            Assert.Contains(new Tuple<string, string>("", "Test parent"), task.Parents);
            Assert.Contains(new Tuple<string, string>("", "Test child"), task.Children);
            Assert.Equal("Test description", task.Description);
            Assert.Contains(deadline, task.Deadlines);
            var checklist = actualTasks.First(x => x.Name == "TestChecklist");
            Assert.Contains(new Tuple<string, string>(task.Id.ToString(), task.Name.ToString()), checklist.Parents);
        }
    }
}
