using Moq;
using NUnit.Framework;
using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;

namespace Planum.Testing.UnitTesting
{
    [TestFixture]
    public class ModelUnitTesting
    {
        IUserManager userManager;
        ITaskManager taskManager;
        ITagManager tagManager;

        [SetUp]
        public void SetUp()
        {
            var userRepo = new Mock<IUserRepo>();
            var tagRepo = new Mock<ITagRepo>();
            var taskRepo = new Mock<ITaskRepo>();

            userManager = new UserManager(userRepo.Object, new UserConverter());
            taskManager = new TaskManager(taskRepo.Object, new TaskConverter(), userManager);
            tagManager = new TagManager(tagRepo.Object, taskManager, new TagConverter(), userManager);
        }

        [Test]
        public void TestTagUserCheckups()
        {
            userManager.CurrentUser = null;
            Assert.Throws<CurrentUserIsNullException>(delegate { tagManager.CreateTag("test", "test", "test"); });
            Assert.Throws<CurrentUserIsNullException>(delegate { tagManager.FindTag(1); });
            Assert.Throws<CurrentUserIsNullException>(delegate { tagManager.DeleteTag(1); });
            Assert.Throws<CurrentUserIsNullException>(delegate { tagManager.UpdateTag(new Tag(1, 4, "test", "test", "test")); });
            Assert.Throws<CurrentUserIsNullException>(delegate { tagManager.GetAllTags(); });
            Assert.Throws<CurrentUserIsNullException>(delegate { tagManager.GetTag(1); });
        }

        [Test]
        public void TestTaskUserCheckups()
        {
            userManager.CurrentUser = null;
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.ArchiveTask(1); });
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.UnarchiveTask(1); });
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.GetAllTasks(); });
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.FindTask(1); });
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.GetTask(1); });
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.DeleteTask(1); });
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.ClearParents(1); });
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.ClearChildren(1); });
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.AddChildToTask(1, 2); });
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.AddParentToTask(1, 2); });
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.AddTagToTask(1, 1); });
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.ClearTags(1); });
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.CreateTask(DateTime.Now, DateTime.Now,
                TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_1", false, "task_1 description", false); });
            Assert.Throws<CurrentUserIsNullException>(delegate { taskManager.UpdateTask(new Task(0, DateTime.Now, DateTime.Now, TimeSpan.Zero,
                new List<int>(), new List<int>(), new List<int>(), "test"));
            });
        }
    }
}
