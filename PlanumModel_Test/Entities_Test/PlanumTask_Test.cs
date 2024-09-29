using System.Collections;
using Planum.Model.Entities;

namespace PlanumModel_Test.Entities_Test
{
    public class PlanumTask_Test
    {
        class GetDeadlineStatus_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] {
                    new Deadline { enabled = false },
                    PlanumTaskStatus.COMPLETE
                };
                yield return new object[] {
                    new Deadline {
                        enabled = true,
                        deadline = DateTime.Now.AddDays(+5),
                        warningTime = new TimeSpan(2, 0, 0, 0),
                        duration = new TimeSpan(2, 0, 0, 0),
                    },
                    PlanumTaskStatus.NOT_STARTED
                };
                yield return new object[] {
                    new Deadline {
                        enabled = true,
                        deadline = DateTime.Now.AddDays(+3),
                        warningTime = new TimeSpan(2, 0, 0, 0),
                        duration = new TimeSpan(2, 0, 0, 0),
                    },
                    PlanumTaskStatus.WARNING
                };
                yield return new object[] {
                    new Deadline {
                        enabled = true,
                        deadline = DateTime.Now.AddDays(+1),
                        warningTime = new TimeSpan(2, 0, 0, 0),
                        duration = new TimeSpan(2, 0, 0, 0),
                    },
                    PlanumTaskStatus.IN_PROGRESS
                };
                yield return new object[] {
                    new Deadline {
                        enabled = true,
                        deadline = DateTime.Now.AddDays(-1),
                        warningTime = new TimeSpan(2, 0, 0, 0),
                        duration = new TimeSpan(2, 0, 0, 0),
                    },
                    PlanumTaskStatus.OVERDUE
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(GetDeadlineStatus_TestData))]
        public void GetDeadlineStatus_Test(Deadline deadline, PlanumTaskStatus expectedStatus)
        {
            // Arrange
            // Act
            var actualStatus = deadline.GetDeadlineStatus();

            // Assert
            Assert.Equal(expectedStatus, actualStatus);
        }

        class GetTaskStatus_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var completeDeadline = new Deadline();
                var notStartedDeadline = new Deadline
                {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(5),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                };
                var warningDeadline = new Deadline
                {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(3),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                };
                var inProgressDeadline = new Deadline
                {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(1),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                };
                var overdueDeadline = new Deadline
                {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(-1),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                };

                yield return new object[]
                {
                    new PlanumTask(deadlines: new List<Deadline>()
                            {
                            }),
                    PlanumTaskStatus.DISABLED
                };

                yield return new object[]
                {
                    new PlanumTask(deadlines: new List<Deadline>()
                            {
                                completeDeadline
                            }),
                    PlanumTaskStatus.COMPLETE
                };

                yield return new object[]
                {
                    new PlanumTask(deadlines: new List<Deadline>()
                            {
                                completeDeadline,
                                notStartedDeadline
                            }),
                    PlanumTaskStatus.NOT_STARTED
                };

                yield return new object[]
                {
                    new PlanumTask(deadlines: new List<Deadline>()
                            {
                                completeDeadline,
                                notStartedDeadline,
                                warningDeadline
                            }),
                    PlanumTaskStatus.WARNING
                };

                yield return new object[]
                {
                    new PlanumTask(deadlines: new List<Deadline>()
                            {
                                completeDeadline,
                                notStartedDeadline,
                                warningDeadline,
                                inProgressDeadline
                            }),
                    PlanumTaskStatus.IN_PROGRESS
                };

                yield return new object[]
                {
                    new PlanumTask(deadlines: new List<Deadline>()
                            {
                                completeDeadline,
                                notStartedDeadline,
                                warningDeadline,
                                inProgressDeadline,
                                overdueDeadline
                            }),
                    PlanumTaskStatus.OVERDUE
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(GetTaskStatus_TestData))]
        public void GetTaskStatus_Test(PlanumTask task, PlanumTaskStatus expectedStatus)
        {
            // Arrange
            // Act
            var actualStatus = task.GetTaskStatus();

            // Assert
            Assert.Equal(expectedStatus, actualStatus);
        }

        class GetTaskStatuses_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var completeDeadlines = new HashSet<Deadline> { new Deadline {
                    enabled = false
                }};

                var notStartedDeadlines = new HashSet<Deadline> { new Deadline {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(5),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                }};

                var warningDeadlines = new HashSet<Deadline> { new Deadline {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(3),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                }};

                var inProgressDeadlines = new HashSet<Deadline> { new Deadline {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(1),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                }};

                var overdueDeadlines = new HashSet<Deadline> { new Deadline {
                    enabled = true,
                    deadline = DateTime.Now.AddDays(-1),
                    warningTime = new TimeSpan(2, 0, 0, 0),
                    duration = new TimeSpan(2, 0, 0, 0),
                }};

                var rootGuid = Guid.NewGuid();
                var firstLevelGuid = Guid.NewGuid();

                var disabledGuid = Guid.NewGuid();
                var completeGuid = Guid.NewGuid();
                var notStartedGuid = Guid.NewGuid();
                var warningGuid = Guid.NewGuid();
                var inProgressGuid = Guid.NewGuid();
                var overdueGuid = Guid.NewGuid();

                // children first level
                yield return new object[]
                {
                    PlanumTask.UpdateRelatives(new List<PlanumTask>
                    {
                        new PlanumTask(id: rootGuid, children: new Guid[] { disabledGuid }),
                        new PlanumTask(id: disabledGuid, parents: new Guid[] { rootGuid }, deadlines: new Deadline[] {})
                    }),
                    rootGuid,
                    PlanumTaskStatus.DISABLED
                };
                yield return new object[]
                {
                    PlanumTask.UpdateRelatives(new List<PlanumTask>
                    {
                        new PlanumTask(id: rootGuid, children: new Guid[] { completeGuid }),
                        new PlanumTask(id: completeGuid, parents: new Guid[] { rootGuid }, deadlines: completeDeadlines)
                    }),
                    rootGuid,
                    PlanumTaskStatus.COMPLETE
                };
                yield return new object[]
                {
                    PlanumTask.UpdateRelatives(new List<PlanumTask>
                    {
                        new PlanumTask(id: rootGuid, children: new Guid[] { completeGuid, notStartedGuid }),
                        new PlanumTask(id: completeGuid, parents: new Guid[] { rootGuid }, deadlines: completeDeadlines),
                        new PlanumTask(id: notStartedGuid, parents: new Guid[] { rootGuid }, deadlines: notStartedDeadlines)
                    }),
                    rootGuid,
                    PlanumTaskStatus.NOT_STARTED
                };
                yield return new object[]
                {
                    PlanumTask.UpdateRelatives(new List<PlanumTask>
                    {
                        new PlanumTask(id: rootGuid, children: new Guid[] { completeGuid, notStartedGuid, warningGuid }),
                        new PlanumTask(id: completeGuid, parents: new Guid[] { rootGuid }, deadlines: completeDeadlines),
                        new PlanumTask(id: notStartedGuid, parents: new Guid[] { rootGuid }, deadlines: notStartedDeadlines),
                        new PlanumTask(id: warningGuid, parents: new Guid[] { rootGuid }, deadlines: warningDeadlines)
                    }),
                    rootGuid,
                    PlanumTaskStatus.WARNING
                };
                yield return new object[]
                {
                    PlanumTask.UpdateRelatives(new List<PlanumTask>
                    {
                        new PlanumTask(id: rootGuid, children: new Guid[] { completeGuid, notStartedGuid, warningGuid, inProgressGuid }),
                        new PlanumTask(id: completeGuid, parents: new Guid[] { rootGuid }, deadlines: completeDeadlines),
                        new PlanumTask(id: notStartedGuid, parents: new Guid[] { rootGuid }, deadlines: notStartedDeadlines),
                        new PlanumTask(id: warningGuid, parents: new Guid[] { rootGuid }, deadlines: warningDeadlines),
                        new PlanumTask(id: inProgressGuid, parents: new Guid[] { rootGuid }, deadlines: inProgressDeadlines)
                    }),
                    rootGuid,
                    PlanumTaskStatus.IN_PROGRESS
                };
                yield return new object[]
                {
                    PlanumTask.UpdateRelatives(new List<PlanumTask>
                    {
                        new PlanumTask(id: rootGuid, children: new Guid[] { completeGuid, notStartedGuid, warningGuid, inProgressGuid, overdueGuid }),
                        new PlanumTask(id: completeGuid, parents: new Guid[] { rootGuid }, deadlines: completeDeadlines),
                        new PlanumTask(id: notStartedGuid, parents: new Guid[] { rootGuid }, deadlines: notStartedDeadlines),
                        new PlanumTask(id: warningGuid, parents: new Guid[] { rootGuid }, deadlines: warningDeadlines),
                        new PlanumTask(id: inProgressGuid, parents: new Guid[] { rootGuid }, deadlines: inProgressDeadlines),
                        new PlanumTask(id: overdueGuid, parents: new Guid[] { rootGuid }, deadlines: overdueDeadlines)
                    }),
                    rootGuid,
                    PlanumTaskStatus.OVERDUE
                };

                // complete task
                yield return new object[]
                {
                    PlanumTask.UpdateRelatives(new List<PlanumTask>
                    {
                        new PlanumTask(id: rootGuid, children: new Guid[] { completeGuid, notStartedGuid, warningGuid, inProgressGuid, overdueGuid }, tags: new string[] { DefaultTags.Complete }),
                        new PlanumTask(id: completeGuid, parents: new Guid[] { rootGuid }, deadlines: completeDeadlines),
                        new PlanumTask(id: notStartedGuid, parents: new Guid[] { rootGuid }, deadlines: notStartedDeadlines),
                        new PlanumTask(id: warningGuid, parents: new Guid[] { rootGuid }, deadlines: warningDeadlines),
                        new PlanumTask(id: inProgressGuid, parents: new Guid[] { rootGuid }, deadlines: inProgressDeadlines),
                        new PlanumTask(id: overdueGuid, parents: new Guid[] { rootGuid }, deadlines: overdueDeadlines)
                    }),
                    rootGuid,
                    PlanumTaskStatus.COMPLETE
                };

                // second level
                yield return new object[]
                {
                    PlanumTask.UpdateRelatives(new List<PlanumTask>
                    {
                        new PlanumTask(id: rootGuid, children: new Guid[] { firstLevelGuid }),
                        new PlanumTask(id: firstLevelGuid, parents: new Guid[] { rootGuid }, children: new Guid[] { completeGuid, notStartedGuid, warningGuid, inProgressGuid, overdueGuid }),
                        new PlanumTask(id: completeGuid, parents: new Guid[] { firstLevelGuid }, deadlines: completeDeadlines),
                        new PlanumTask(id: notStartedGuid, parents: new Guid[] { firstLevelGuid }, deadlines: notStartedDeadlines),
                        new PlanumTask(id: warningGuid, parents: new Guid[] { firstLevelGuid }, deadlines: warningDeadlines),
                        new PlanumTask(id: inProgressGuid, parents: new Guid[] { firstLevelGuid }, deadlines: inProgressDeadlines),
                        new PlanumTask(id: overdueGuid, parents: new Guid[] { firstLevelGuid }, deadlines: overdueDeadlines)
                    }),
                    rootGuid,
                    PlanumTaskStatus.OVERDUE
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(GetTaskStatuses_TestData))]
        public void GetTaskStatuses_Test(List<PlanumTask> tasks, Guid expectedTaskGuid, PlanumTaskStatus expectedStatus)
        {
            // Arrange
            // Act
            var actualStatuses = PlanumTask.GetTaskStatuses(tasks);

            // Assert
            Assert.True(actualStatuses.ContainsKey(expectedTaskGuid));
            Assert.Equal(expectedStatus, actualStatuses[expectedTaskGuid]);
        }

        [Fact]
        public void UpdateRelatives_Test()
        {
            // Arrange
            Guid parentId = Guid.NewGuid();
            Guid taskId = Guid.NewGuid();
            Guid childId = Guid.NewGuid();

            List<PlanumTask> tasks = new List<PlanumTask>
            {
                new PlanumTask(parentId, children: new Guid[] { taskId }),
                new PlanumTask(taskId),
                new PlanumTask(childId, parents: new Guid[] { taskId })
            };

            IEnumerable<PlanumTask> expectedTasks = new List<PlanumTask>
            {
                new PlanumTask(parentId, children: new Guid[] { taskId }),
                new PlanumTask(taskId, parents: new Guid[] { parentId }, children: new Guid[] { childId }),
                new PlanumTask(childId, parents: new Guid[] { taskId })
            };

            // Act
            var actualTasks = PlanumTask.UpdateRelatives(tasks);

            // Assert
            Assert.Equal(actualTasks, expectedTasks);
        }
    }
}
