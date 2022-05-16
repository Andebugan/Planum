using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.BuisnessLayer.Interfaces;
using Planum.Models.BuisnessLayer.Managers;
using Planum.Models.DataLayer;
using Planum.Models.DTO.ModelData;
using System.Diagnostics;

using NUnit.Framework;

namespace Planum.Testing.UnitTesting
{
    [TestFixture]
    public class ModelUnitTesting
    {
        private TagManager? _tagManager;
        private TaskManager? _taskManager;
        private UserManager? _userManager;

        private ITagRepo? _tagRepoFile;
        private ITaskRepo? _taskRepoFile;
        private IUserRepo? _userRepoFile;

        [SetUp]
        public void SetUp()
        {
            _tagRepoFile = new TagRepoFile();
            _taskRepoFile = new TaskRepoFile();
            _userRepoFile = new UserRepoFile();

            _taskManager = new TaskManager(ref _taskRepoFile);
            _tagManager = new TagManager(ref _tagRepoFile, ref _taskManager);
            _userManager = new UserManager(ref _userRepoFile, ref _tagManager, ref _taskManager);
        }

        [Test]
        public void TestTaskConstructor()
        {
            ITaskRepo? taskRepo = null;
            Assert.Throws<ArgumentNullException>(delegate { new TaskManager(ref taskRepo); });
            taskRepo = _taskRepoFile;
            Assert.IsInstanceOf<TaskManager>(new TaskManager(ref taskRepo));
        }

        [Test]
        public void TestTagConstructor()
        {
            ITagRepo tagRepo = null;
            TaskManager taskManager = null;
            Assert.Throws<ArgumentNullException>(delegate { new TagManager(ref tagRepo, ref taskManager); });

            tagRepo = _tagRepoFile;
            taskManager = null;
            Assert.Throws<ArgumentNullException>(delegate { new TagManager(ref tagRepo, ref taskManager); });

            tagRepo = null;
            taskManager = _taskManager;
            Assert.Throws<ArgumentNullException>(delegate { new TagManager(ref tagRepo, ref taskManager); });

            tagRepo = _tagRepoFile;
            taskManager = _taskManager;
            Assert.IsInstanceOf<TagManager>(new TagManager(ref tagRepo, ref taskManager));
        }

        [Test]
        public void TestUserConstructor()
        {
            IUserRepo userRepo =  null;
            TaskManager taskManager = null;
            TagManager tagManager = null;

            Assert.Throws<ArgumentNullException>(delegate { new UserManager(ref userRepo, ref tagManager, ref taskManager); });

            userRepo = null;
            taskManager = _taskManager;
            tagManager = _tagManager;

            Assert.Throws<ArgumentNullException>(delegate { new UserManager(ref userRepo, ref tagManager, ref taskManager); });

            userRepo = _userRepoFile;
            taskManager = null;
            tagManager = _tagManager;

            Assert.Throws<ArgumentNullException>(delegate { new UserManager(ref userRepo, ref tagManager, ref taskManager); });

            userRepo = _userRepoFile;
            taskManager = _taskManager;
            tagManager = null;

            Assert.Throws<ArgumentNullException>(delegate { new UserManager(ref userRepo, ref tagManager, ref taskManager); });

            userRepo = _userRepoFile;
            taskManager = _taskManager;
            tagManager = _tagManager;

            Assert.IsInstanceOf<UserManager>(new UserManager(ref userRepo, ref tagManager, ref taskManager));
        }
    }
}
