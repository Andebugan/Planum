using System.Collections;
using Planum.Config;
using Planum.Logger;
using Planum.Model.Entities;

namespace Planum.Tests
{
    public class TestMarkdownTaskData : IEnumerable<object[]>
    {
        public void GetTaskNameDescriptionTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);
            task = new PlanumTask(Guid.NewGuid(), "test name", "test description");
            tasks = new PlanumTask[] { task };
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [ ] n: test name",
                    "- d: test description",
                    ""
                };
        }

        public void GetTaskNameDescriptionMultilineTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);
            task = new PlanumTask(Guid.NewGuid(), "test name", "test description\\new line 1\\new line 2\\new line 3");
            tasks = new PlanumTask[] { task };
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [ ] n: test name",
                    "- d: test description\\",
                    "  new line 1\\",
                    "  new line 2\\",
                    "  new line 3",
                    ""
                };
        }

        public void GetTaskTagTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);
            task = new PlanumTask(Guid.NewGuid(), "test name", tags: new string[] { "user tag 1", "user tag 2", "user tag 3" });
            tasks = new PlanumTask[] { task };
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [ ] n: test name",
                    "- t: user tag 1",
                    "- t: user tag 2",
                    "- t: user tag 3",
                    ""
                };
        }

        public void GetTaskCompleteTagTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);
            task = new PlanumTask(Guid.NewGuid(), "test name", tags: new string[] { DefaultTags.Complete });
            tasks = new PlanumTask[] { task };
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [x] n: test name",
                    ""
                };
        }

        public void GetTaskDeadlineFieldsTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);

            Deadline[] deadlines = new Deadline[] {
                    new Deadline(false, new DateTime(2024, 7, 25, 10, 0, 0), new TimeSpan(1, 2, 3, 0), new TimeSpan(2, 3, 4, 0)),
                    new Deadline(false, new DateTime(2024, 7, 25, 11, 0, 0), new TimeSpan(1, 2, 3, 0), new TimeSpan(2, 3, 4, 0), true, new TimeSpan(1, 2, 3, 0), 1, 2),
                    new Deadline(false, new DateTime(2024, 7, 25, 12, 0, 0), TimeSpan.Zero, TimeSpan.Zero, false, new TimeSpan(2, 3, 4, 0), 2, 3),
                };

            task = new PlanumTask(Guid.NewGuid(), "test name", deadlines: deadlines);
            tasks = new PlanumTask[] { task };
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [ ] n: test name",
                    "- [x] D: " + deadlines[0].Id,
                    "  - d: 10:0 25.7.24",
                    "  - w: 1.2:3",
                    "  - du: 2.3:4",
                    "- [x] D: " + deadlines[1].Id,
                    "  - d: 11:0 25.7.24",
                    "  - w: 1.2:3",
                    "  - du: 2.3:4",
                    "  - [x] r: 1 2 1.2:3",
                    "- [x] D: " + deadlines[2].Id,
                    "  - d: 12:0 25.7.24",
                    "  - [ ] r: 2 3 2.3:4",
                    ""
                };
        }

        public void GetTaskDeadlineOverdueTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);

            Deadline[] deadlines = new Deadline[] {
                    new Deadline(true, DateTime.Today.AddDays(-1), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)),
                };

            task = new PlanumTask(Guid.NewGuid(), "test name", deadlines: deadlines);
            tasks = new PlanumTask[] { task };
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [#] n: test name",
                    "- [#] D: " + deadlines[0].Id,
                    "  - d: " + deadlines[0].deadline.ToString(repoConfig.TaskDateTimeWriteFormat),
                    "  - w: 2.0:0",
                    "  - du: 2.0:0",
                    ""
                };
        }

        public void GetTaskDeadlineInProgressTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);

            Deadline[] deadlines = new Deadline[] {
                    new Deadline(true, DateTime.Today.AddDays(1), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)),
                };

            task = new PlanumTask(Guid.NewGuid(), "test name", deadlines: deadlines);
            tasks = new PlanumTask[] { task };
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [*] n: test name",
                    "- [*] D: " + deadlines[0].Id,
                    "  - d: " + deadlines[0].deadline.ToString(repoConfig.TaskDateTimeWriteFormat),
                    "  - w: 2.0:0",
                    "  - du: 2.0:0",
                    ""
                };

        }

        public void GetTaskDeadlineWarningTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);

            Deadline[] deadlines = new Deadline[] {
                    new Deadline(true, DateTime.Today.AddDays(3), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)),
                };

            task = new PlanumTask(Guid.NewGuid(), "test name", deadlines: deadlines);
            tasks = new PlanumTask[] { task };
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [.] n: test name",
                    "- [.] D: " + deadlines[0].Id,
                    "  - d: " + deadlines[0].deadline.ToString(repoConfig.TaskDateTimeWriteFormat),
                    "  - w: 2.0:0",
                    "  - du: 2.0:0",
                    ""
                };

        }

        public void GetTaskDeadlineNotStartedTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);

            Deadline[] deadlines = new Deadline[] {
                    new Deadline(true, DateTime.Today.AddDays(5), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)),
                };

            task = new PlanumTask(Guid.NewGuid(), "test name", deadlines: deadlines);
            tasks = new PlanumTask[] { task };
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [ ] n: test name",
                    "- [ ] D: " + deadlines[0].Id,
                    "  - d: " + deadlines[0].deadline.ToString(repoConfig.TaskDateTimeWriteFormat),
                    "  - w: 2.0:0",
                    "  - du: 2.0:0",
                    ""
                };
        }

        public void GetTaskParentsTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);

            PlanumTask[] parents = new PlanumTask[] {
                    new PlanumTask(Guid.NewGuid(), "normal"),
                    new PlanumTask(Guid.NewGuid(), "disabled", deadlines: new Deadline[] { new Deadline(false, DateTime.Today) }),
                    new PlanumTask(Guid.NewGuid(), "notStarted", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(5), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                    new PlanumTask(Guid.NewGuid(), "warning", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(3), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                    new PlanumTask(Guid.NewGuid(), "inProgress", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(1), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                    new PlanumTask(Guid.NewGuid(), "overdue", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(-1), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                };

            foreach (var parent in parents)
                repoConfig.TaskLookupPaths.Add(parent.Name + "_path", new HashSet<Guid> { parent.Id });

            task = new PlanumTask(Guid.NewGuid(), "test name", parents: parents.Select(x => x.Id));
            tasks = new PlanumTask[] { task };
            tasks = tasks.Concat(parents);
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [ ] n: test name",
                    "- [ ] p: [normal](normal_path)",
                    "- [ ] p: [disabled](disabled_path)",
                    "- [ ] p: [notStarted](notStarted_path)",
                    "- [.] p: [warning](warning_path)",
                    "- [*] p: [inProgress](inProgress_path)",
                    "- [#] p: [overdue](overdue_path)",
                    ""
                };
        }

        public void GetTaskChildTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);

            PlanumTask[] children = new PlanumTask[] {
                    new PlanumTask(Guid.NewGuid(), "normal"),
                    new PlanumTask(Guid.NewGuid(), "disabled", deadlines: new Deadline[] { new Deadline(false, DateTime.Today) }),
                    new PlanumTask(Guid.NewGuid(), "notStarted", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(5), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                    new PlanumTask(Guid.NewGuid(), "warning", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(3), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                    new PlanumTask(Guid.NewGuid(), "inProgress", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(1), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                    new PlanumTask(Guid.NewGuid(), "overdue", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(-1), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                };

            foreach (var child in children)
                repoConfig.TaskLookupPaths.Add(child.Name + "_path", new HashSet<Guid>()  { child.Id });

            task = new PlanumTask(Guid.NewGuid(), "test name", children: children.Select(x => x.Id));
            tasks = new PlanumTask[] { task };
            tasks = tasks.Concat(children);
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [#] n: test name",
                    "- [ ] c: [normal](normal_path)",
                    "- [ ] c: [disabled](disabled_path)",
                    "- [ ] c: [notStarted](notStarted_path)",
                    "- [.] c: [warning](warning_path)",
                    "- [*] c: [inProgress](inProgress_path)",
                    "- [#] c: [overdue](overdue_path)",
                    ""
                };
        }

        public void GetTaskChildTestOverdueData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);

            PlanumTask[] children = new PlanumTask[] {
                    new PlanumTask(Guid.NewGuid(), "notStarted", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(5), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                    new PlanumTask(Guid.NewGuid(), "warning", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(3), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                    new PlanumTask(Guid.NewGuid(), "inProgress", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(1), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                    new PlanumTask(Guid.NewGuid(), "overdue", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(-1), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                };

            foreach (var child in children)
                repoConfig.TaskLookupPaths.Add(child.Name + "_path", new HashSet<Guid>()  { child.Id });

            task = new PlanumTask(Guid.NewGuid(), "test name", children: children.Select(x => x.Id));
            tasks = new PlanumTask[] { task };
            tasks = tasks.Concat(children);
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [#] n: test name",
                    "- [ ] c: [notStarted](notStarted_path)",
                    "- [.] c: [warning](warning_path)",
                    "- [*] c: [inProgress](inProgress_path)",
                    "- [#] c: [overdue](overdue_path)",
                    ""
                };
        }

        public void GetTaskChildTestInProgressData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);

            PlanumTask[] children = new PlanumTask[] {
                    new PlanumTask(Guid.NewGuid(), "notStarted", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(5), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                    new PlanumTask(Guid.NewGuid(), "warning", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(3), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                    new PlanumTask(Guid.NewGuid(), "inProgress", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(1), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                };

            foreach (var child in children)
                repoConfig.TaskLookupPaths.Add(child.Name + "_path", new HashSet<Guid>()  { child.Id });

            task = new PlanumTask(Guid.NewGuid(), "test name", children: children.Select(x => x.Id));
            tasks = new PlanumTask[] { task };
            tasks = tasks.Concat(children);
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [*] n: test name",
                    "- [ ] c: [notStarted](notStarted_path)",
                    "- [.] c: [warning](warning_path)",
                    "- [*] c: [inProgress](inProgress_path)",
                    ""
                };
        }

        public void GetTaskChildTestWarningData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);

            PlanumTask[] children = new PlanumTask[] {
                    new PlanumTask(Guid.NewGuid(), "warning", deadlines: new Deadline[] { new Deadline(true, DateTime.Today.AddDays(3), new TimeSpan(2, 0, 0, 0), new TimeSpan(2, 0, 0, 0)) }),
                };

            foreach (var child in children)
                repoConfig.TaskLookupPaths.Add(child.Name + "_path", new HashSet<Guid>()  { child.Id });

            task = new PlanumTask(Guid.NewGuid(), "test name", children: children.Select(x => x.Id));
            tasks = new PlanumTask[] { task };
            tasks = tasks.Concat(children);
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [.] n: test name",
                    "- [.] c: [warning](warning_path)",
                    ""
                };
        }

        public void GetChecklistTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);

            Deadline deadline = new Deadline(false, DateTime.Today, new TimeSpan(1, 1, 1, 0), new TimeSpan(1, 1, 1, 0), false, new TimeSpan(1, 1, 1, 0), 1, 1);
            PlanumTask checklist = new PlanumTask(Guid.NewGuid(), "checklist", "test checklist", deadlines: new Deadline[] { deadline }, tags: new string[] { DefaultTags.Checklist });

            task = new PlanumTask(Guid.NewGuid(), "test name", children: new HashSet<Guid>()  { checklist.Id });
            tasks = new PlanumTask[] { task, checklist };
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                     "<planum:" + task.Id.ToString() + ">",
                    "- [ ] n: test name",
                    "- [ ] checklist",
                    "  - d: test checklist",
                    "  - [x] D: " + deadline.Id,
                    "    - d: " + deadline.deadline.ToString(repoConfig.TaskDateTimeWriteFormat),
                    "    - w: " + deadline.warningTime.ToString(repoConfig.TaskTimeSpanWriteFormat),
                    "    - du: " + deadline.duration.ToString(repoConfig.TaskTimeSpanWriteFormat),
                    "    - [ ] r: 1 1 " + deadline.repeatSpan.ToString(repoConfig.TaskTimeSpanWriteFormat),
                    ""
                };
        }

        public void GetChecklistMultilevelTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);

            PlanumTask checklist_1_1_1 = new PlanumTask(Guid.NewGuid(), "checklist 1 1 1", tags: new string[] { DefaultTags.Checklist });
            PlanumTask checklist_1_1_2 = new PlanumTask(Guid.NewGuid(), "checklist 1 1 2", tags: new string[] { DefaultTags.Checklist });
            PlanumTask checklist_1_1_3 = new PlanumTask(Guid.NewGuid(), "checklist 1 1 3", tags: new string[] { DefaultTags.Checklist });

            PlanumTask checklist_1_2_1 = new PlanumTask(Guid.NewGuid(), "checklist 1 2 1", tags: new string[] { DefaultTags.Checklist });
            PlanumTask checklist_1_2_2 = new PlanumTask(Guid.NewGuid(), "checklist 1 2 2", tags: new string[] { DefaultTags.Checklist });
            PlanumTask checklist_1_2_3 = new PlanumTask(Guid.NewGuid(), "checklist 1 2 3", tags: new string[] { DefaultTags.Checklist });

            PlanumTask checklist_1_3_1 = new PlanumTask(Guid.NewGuid(), "checklist 1 3 1", tags: new string[] { DefaultTags.Checklist });
            PlanumTask checklist_1_3_2 = new PlanumTask(Guid.NewGuid(), "checklist 1 3 2", tags: new string[] { DefaultTags.Checklist });
            PlanumTask checklist_1_3_3 = new PlanumTask(Guid.NewGuid(), "checklist 1 3 3", tags: new string[] { DefaultTags.Checklist });

            PlanumTask checklist_1_1 = new PlanumTask(Guid.NewGuid(), "checklist 1 1", children: new HashSet<Guid>()  { checklist_1_1_1.Id, checklist_1_1_2.Id, checklist_1_1_3.Id }, tags: new string[] { DefaultTags.Checklist });
            PlanumTask checklist_1_2 = new PlanumTask(Guid.NewGuid(), "checklist 1 2", children: new HashSet<Guid>()  { checklist_1_2_1.Id, checklist_1_2_2.Id, checklist_1_2_3.Id }, tags: new string[] { DefaultTags.Checklist });
            PlanumTask checklist_1_3 = new PlanumTask(Guid.NewGuid(), "checklist 1 3", children: new HashSet<Guid>()  { checklist_1_3_1.Id, checklist_1_3_2.Id, checklist_1_3_3.Id }, tags: new string[] { DefaultTags.Checklist });

            PlanumTask checklist_1 = new PlanumTask(Guid.NewGuid(), "checklist 1", children: new HashSet<Guid>()  { checklist_1_1.Id, checklist_1_2.Id, checklist_1_3.Id }, tags: new string[] { DefaultTags.Checklist });

            task = new PlanumTask(Guid.NewGuid(), "test name", children: new HashSet<Guid>()  { checklist_1.Id });
            tasks = new PlanumTask[] { task, checklist_1, checklist_1_1, checklist_1_2, checklist_1_3, checklist_1_1_1, checklist_1_1_2, checklist_1_1_3, checklist_1_2_1, checklist_1_2_2, checklist_1_2_3, checklist_1_3_1, checklist_1_3_2, checklist_1_3_3 };
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                     "<planum:" + task.Id.ToString() + ">",
                    "- [ ] n: test name",
                    "- [ ] checklist 1",
                    "  - [ ] checklist 1 1",
                    "    - [ ] checklist 1 1 1",
                    "    - [ ] checklist 1 1 2",
                    "    - [ ] checklist 1 1 3",
                    "  - [ ] checklist 1 2",
                    "    - [ ] checklist 1 2 1",
                    "    - [ ] checklist 1 2 2",
                    "    - [ ] checklist 1 2 3",
                    "  - [ ] checklist 1 3",
                    "    - [ ] checklist 1 3 1",
                    "    - [ ] checklist 1 3 2",
                    "    - [ ] checklist 1 3 3",
                    ""
                };
        }

        public void GetDeadlineNextTestData(ILoggerWrapper logger, out RepoConfig repoConfig, out PlanumTask task, out IEnumerable<PlanumTask> tasks, out string[] expected)
        {
            repoConfig = RepoConfig.Load(logger);

            PlanumTask next_1 = new PlanumTask(Guid.NewGuid(), "next 1");
            PlanumTask next_2 = new PlanumTask(Guid.NewGuid(), "next 2");
            PlanumTask next_3 = new PlanumTask(Guid.NewGuid(), "next 3");

            repoConfig.TaskLookupPaths.Add("next_file_1", new HashSet<Guid>()  { next_1.Id });
            repoConfig.TaskLookupPaths.Add("next_file_2", new HashSet<Guid>()  { next_2.Id });
            repoConfig.TaskLookupPaths.Add("next_file_3", new HashSet<Guid>()  { next_3.Id });

            Deadline[] deadlines = new Deadline[] {
                    new Deadline(false, DateTime.Today.AddDays(5), next: new HashSet<Guid>() { next_1.Id, next_2.Id, next_3.Id } ),
                };

            task = new PlanumTask(Guid.NewGuid(), "test name", deadlines: deadlines);
            tasks = new PlanumTask[] { task, next_1, next_2, next_3 };
            tasks = PlanumTask.UpdateRelatives(tasks).ToList();

            expected = new string[] {
                    "<planum:" + task.Id.ToString() + ">",
                    "- [ ] n: test name",
                    "- [x] D: " + deadlines[0].Id,
                    "  - d: " + deadlines[0].deadline.ToString(repoConfig.TaskDateTimeWriteFormat),
                    "  - [ ] n: [next 1](next_file_1)",
                    "  - [ ] n: [next 2](next_file_2)",
                    "  - [ ] n: [next 3](next_file_3)",
                    ""
                };
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            RepoConfig repoConfig;
            PlanumTask task;
            IEnumerable<PlanumTask> tasks;
            string[] expected;

            ILoggerWrapper logger = new PlanumLogger(LogLevel.INFO, clearFile: true);

            GetTaskNameDescriptionTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };
            GetTaskNameDescriptionMultilineTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };

            GetTaskTagTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };
            GetTaskCompleteTagTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };

            GetTaskDeadlineFieldsTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };
            GetTaskDeadlineOverdueTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };
            GetTaskDeadlineInProgressTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };
            GetTaskDeadlineWarningTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };
            GetTaskDeadlineNotStartedTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };

            GetTaskParentsTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };
            GetTaskChildTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };
            GetTaskChildTestOverdueData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };
            GetTaskChildTestInProgressData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };
            GetTaskChildTestWarningData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };

            GetChecklistTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };
            GetChecklistMultilevelTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };

            GetDeadlineNextTestData(logger, out repoConfig, out task, out tasks, out expected);
            yield return new object[] { task, tasks, expected, repoConfig };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
