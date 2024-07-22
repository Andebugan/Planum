using System.Collections;
using Planum.Model.Entities;
using Planum.Parser;

namespace Planum.Tests
{
    public class Test_TaskValueParser
    {
        public class ParseIdentityTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                Guid taskId = Guid.NewGuid();
                string name = "taskname";

                // test both match
                yield return new object[] { 
                    0,
                    taskId.ToString(),
                    name,
                    new PlanumTask[] {
                        new PlanumTask(taskId, name: name),
                        new PlanumTask(Guid.NewGuid(), name: "test"),
                        new PlanumTask(Guid.NewGuid(), name: "test")
                    },
                    new PlanumTask[] {
                        new PlanumTask(taskId, name: name),
                    },
                };

                // test id full match
                yield return new object[] { 
                    1,
                    taskId.ToString(),
                    name,
                    new PlanumTask[] {
                        new PlanumTask(taskId, name: "test"),
                        new PlanumTask(Guid.NewGuid(), name: "test"),
                        new PlanumTask(Guid.NewGuid(), name: "test")
                    },
                    new PlanumTask[] {
                        new PlanumTask(taskId, name: "test"),
                    },
                };

                // test id parital match
                yield return new object[] { 
                    2,
                    taskId.ToString().Remove(taskId.ToString().Length / 2, taskId.ToString().Length / 2),
                    name,
                    new PlanumTask[] {
                        new PlanumTask(taskId, name: "test"),
                        new PlanumTask(Guid.NewGuid(), name: "test"),
                        new PlanumTask(Guid.NewGuid(), name: "test")
                    },
                    new PlanumTask[] {
                        new PlanumTask(taskId, name: "test"),
                    },
                };

                // test name full match
                yield return new object[] { 
                    3,
                    "",
                    name,
                    new PlanumTask[] {
                        new PlanumTask(Guid.Empty, name: name),
                        new PlanumTask(Guid.NewGuid(), name: "test"),
                        new PlanumTask(Guid.NewGuid(), name: "test")
                    },
                    new PlanumTask[] {
                        new PlanumTask(Guid.Empty, name: name),
                    },
                };

                // test name partial match
                yield return new object[] { 
                    4,
                    "",
                    name.Remove(name.Length / 2, name.Length / 2),
                    new PlanumTask[] {
                        new PlanumTask(Guid.Empty, name: name),
                        new PlanumTask(Guid.NewGuid(), name: "test"),
                        new PlanumTask(Guid.NewGuid(), name: "test")
                    },
                    new PlanumTask[] {
                        new PlanumTask(Guid.Empty, name: name),
                    },
                };

                // test no match
                yield return new object[] { 
                    5,
                    taskId.ToString(),
                    name,
                    new PlanumTask[] {
                        new PlanumTask(Guid.NewGuid(), name: "test"),
                        new PlanumTask(Guid.NewGuid(), name: "test"),
                        new PlanumTask(Guid.NewGuid(), name: "test")
                    },
                    new PlanumTask[] {
                    },
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(ParseIdentityTestData))]
        public void Test_ParseIdentity(int testId, string id, string name, IEnumerable<PlanumTask> taskBuffer, IEnumerable<PlanumTask> expected)
        {
            // Arrange

            // Act
            var tasks = TaskValueParser.ParseIdentity(id, name, taskBuffer);

            // Assert
            Assert.True(expected.SequenceEqual(tasks));
        }
    }
}
