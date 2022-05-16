using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Planum.Models.BuisnessLayer.Managers;
using Planum.Models.BuisnessLayer.Interfaces;
using Planum.Models.DTO.ModelData;
using Planum.Models.DataLayer;

namespace Planum.Testing.UnitTesting
{
    [TestFixture]
    public class RepoUnitTesting
    {
        IUserRepo userRepo = new UserRepoFile();
        ITaskRepo taskRepo = new TaskRepoFile();
        ITagRepo tagRepo = new TagRepoFile();

        [SetUp]
        public void SetUp()
        {
            userRepo.Reset();
            taskRepo.Reset();
            tagRepo.Reset();
        }

        [Test]
        public void TestUserAddGet()
        {
            userRepo.Reset();
            UserDTO testDTO1 = new UserDTO(1, "login_1", "password_1");
            UserDTO testDTO2 = new UserDTO(2, "login_2", "password_2");
            UserDTO testDTO3 = new UserDTO(3, "login_3", "password_3");

            userRepo.Add(testDTO1);
            userRepo.Add(testDTO3);
            userRepo.Add(testDTO2);

            UserDTO testDTO = userRepo.Get(1);
            Assert.AreEqual(testDTO, testDTO1);

            testDTO = userRepo.Get(2);
            Assert.AreEqual(testDTO, testDTO2);

            testDTO = userRepo.Get(3);
            Assert.AreEqual(testDTO, testDTO3);
            userRepo.Reset();
        }

        [Test]
        public void TestUserGetAll()
        {
            userRepo.Reset();
            UserDTO testDTO1 = new UserDTO(1, "login_1", "password_1");
            UserDTO testDTO2 = new UserDTO(2, "login_2", "password_2");
            UserDTO testDTO3 = new UserDTO(3, "login_3", "password_3");

            userRepo.Add(testDTO1);
            userRepo.Add(testDTO3);
            userRepo.Add(testDTO2);

            List<UserDTO> testDTOs = userRepo.GetAll();

            Assert.AreEqual(testDTOs[0], testDTO1);
            Assert.AreEqual(testDTOs[1], testDTO3);
            Assert.AreEqual(testDTOs[2], testDTO2);
            userRepo.Reset();
        }

        [Test]
        public void TestUserUpdate()
        {
            userRepo.Reset();
            UserDTO testDTO1 = new UserDTO(1, "login_1", "password_1");
            UserDTO testDTO2 = new UserDTO(2, "login_2", "password_2");
            UserDTO testDTO3 = new UserDTO(3, "login_3", "password_3");

            userRepo.Add(testDTO1);
            userRepo.Add(testDTO3);
            userRepo.Add(testDTO2);

            testDTO3 = new UserDTO(3, "bebra", "password_3");
            userRepo.Update(testDTO3);
            UserDTO temp = userRepo.Get(3);
            Assert.AreEqual(temp, testDTO3);
            userRepo.Reset();
        }

        [Test]
        public void TestUserDelete()
        {
            userRepo.Reset();
            UserDTO testDTO1 = new UserDTO(1, "login_1", "password_1");
            UserDTO testDTO2 = new UserDTO(2, "login_2", "password_2");
            UserDTO testDTO3 = new UserDTO(3, "login_3", "password_3");

            userRepo.Add(testDTO1);
            userRepo.Add(testDTO3);
            userRepo.Add(testDTO2);

            userRepo.Delete(3);
            List<UserDTO> list = userRepo.GetAll();
            Assert.AreEqual(list[0], testDTO1);
            Assert.AreEqual(list[1], testDTO2);
            userRepo.Reset();
        }

        [Test]
        public void TestUserReset()
        {
            userRepo.Reset();
            UserDTO testDTO1 = new UserDTO(1, "login_1", "password_1");
            UserDTO testDTO2 = new UserDTO(2, "login_2", "password_2");
            UserDTO testDTO3 = new UserDTO(3, "login_3", "password_3");

            userRepo.Add(testDTO1);
            userRepo.Add(testDTO3);
            userRepo.Add(testDTO2);

            userRepo.Reset();
            Assert.Throws<UserDoesNotExist>(delegate { userRepo.Get(1); });
            userRepo.Reset();
        }


        [Test]
        public void TestTagAddGet()
        {
            tagRepo.Reset();
            TagDTO testDTO1 = new TagDTO(1, 1, 1, "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, 2, "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, 3, "tag_3", "some tag 3");

            tagRepo.Add(testDTO1);
            tagRepo.Add(testDTO3);
            tagRepo.Add(testDTO2);

            TagDTO testDTO = tagRepo.Get(1);
            Assert.AreEqual(testDTO, testDTO1);

            testDTO = tagRepo.Get(2);
            Assert.AreEqual(testDTO, testDTO2);

            testDTO = tagRepo.Get(3);
            Assert.AreEqual(testDTO, testDTO3);
            tagRepo.Reset();
        }

        [Test]
        public void TestTagGetAll()
        {
            tagRepo.Reset();
            TagDTO testDTO1 = new TagDTO(1, 1, 1, "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, 2, "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, 3, "tag_3", "some tag 3");

            tagRepo.Add(testDTO1);
            tagRepo.Add(testDTO3);
            tagRepo.Add(testDTO2);

            List<TagDTO> testDTOs = tagRepo.GetAll();

            Assert.AreEqual(testDTOs[0], testDTO1);
            Assert.AreEqual(testDTOs[1], testDTO3);
            Assert.AreEqual(testDTOs[2], testDTO2);
            tagRepo.Reset();
        }

        [Test]
        public void TestTagUpdate()
        {
            tagRepo.Reset();
            TagDTO testDTO1 = new TagDTO(1, 1, 1, "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, 2, "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, 3, "tag_3", "some tag 3");

            tagRepo.Add(testDTO1);
            tagRepo.Add(testDTO3);
            tagRepo.Add(testDTO2);

            testDTO3 = new TagDTO(2, 3, 2, "tag_2", "new description");
            tagRepo.Update(testDTO3);
            TagDTO temp = tagRepo.Get(2);
            Assert.AreEqual(temp, testDTO3);
            tagRepo.Reset();
        }

        [Test]
        public void TestTagDelete()
        {
            tagRepo.Reset();
            TagDTO testDTO1 = new TagDTO(1, 1, 1, "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, 2, "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, 3, "tag_3", "some tag 3");

            tagRepo.Add(testDTO1);
            tagRepo.Add(testDTO3);
            tagRepo.Add(testDTO2);

            tagRepo.Delete(3);
            List<TagDTO> list = tagRepo.GetAll();
            Assert.AreEqual(list[1], testDTO2);
            Assert.AreEqual(list[0], testDTO1);
            tagRepo.Reset();
        }

        [Test]
        public void TestTagReset()
        {
            tagRepo.Reset();
            TagDTO testDTO1 = new TagDTO(1, 1, 1, "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, 2, "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, 3, "tag_3", "some tag 3");

            tagRepo.Add(testDTO1);
            tagRepo.Add(testDTO3);
            tagRepo.Add(testDTO2);

            tagRepo.Reset();
            Assert.Throws<TagDoesNotExist>(delegate { tagRepo.Get(1); });
            tagRepo.Reset();
        }


        [Test]
        public void TestTaskAddGet()
        {
            taskRepo.Reset();
            TaskDTO testDTO1 = new TaskDTO(1, 1, "task_1", false, "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(2, 1, "task_2", DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int> { 1, 2, 4},
                true, "some task 2", 1, true);
            TaskDTO testDTO3 = new TaskDTO(3, 2, "task_3", DateTime.Now, DateTime.Now, TimeSpan.FromHours(10), new List<int> { 1, 3, 4 },
                true, "some task 3", 1, true);

            taskRepo.Add(testDTO1);
            taskRepo.Add(testDTO3);
            taskRepo.Add(testDTO2);

            TaskDTO testDTO = taskRepo.Get(1);
            Assert.AreEqual(testDTO, testDTO1);

            testDTO = taskRepo.Get(2);
            Assert.AreEqual(testDTO, testDTO2);

            testDTO = taskRepo.Get(3);
            Assert.AreEqual(testDTO, testDTO3);
            taskRepo.Reset();
        }

        [Test]
        public void TestTaskGetAll()
        {
            taskRepo.Reset();
            TaskDTO testDTO1 = new TaskDTO(1, 1, "task_1", false, "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(2, 1, "task_2", DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int> { 1, 2, 4},
                true, "some task 2", 1, true);
            TaskDTO testDTO3 = new TaskDTO(3, 2, "task_3", DateTime.Now, DateTime.Now, TimeSpan.FromHours(10), new List<int> { 1, 3, 4 },
                true, "some task 3", 1, true);

            taskRepo.Add(testDTO1);
            taskRepo.Add(testDTO3);
            taskRepo.Add(testDTO2);

            List<TaskDTO> testDTOs = taskRepo.GetAll();

            Assert.AreEqual(testDTOs[0], testDTO1);
            Assert.AreEqual(testDTOs[1], testDTO3);
            Assert.AreEqual(testDTOs[2], testDTO2);
            taskRepo.Reset();
        }

        [Test]
        public void TestTaskUpdate()
        {
            taskRepo.Reset();
            TaskDTO testDTO1 = new TaskDTO(1, 1, "task_1", false, "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(2, 1, "task_2", DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int> { 1, 2, 4 },
                true, "some task 2", 1, true);
            TaskDTO testDTO3 = new TaskDTO(3, 2, "task_3", DateTime.Now, DateTime.Now, TimeSpan.FromHours(10), new List<int> { 1, 3, 4 },
                true, "some task 3", 1, true);

            taskRepo.Add(testDTO1);
            taskRepo.Add(testDTO3);
            taskRepo.Add(testDTO2);

            testDTO3 = new TaskDTO(2, 2, "task_1", false, "task_1 new description", 5, false);
            taskRepo.Update(testDTO3);
            TaskDTO temp = taskRepo.Get(2);
            Assert.AreEqual(temp, testDTO3);
            taskRepo.Reset();
        }

        [Test]
        public void TestTaskDelete()
        {
            taskRepo.Reset();
            TaskDTO testDTO1 = new TaskDTO(1, 1, "task_1", false, "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(2, 1, "task_2", DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int> { 1, 2, 4 },
                true, "some task 2", 1, true);
            TaskDTO testDTO3 = new TaskDTO(3, 2, "task_3", DateTime.Now, DateTime.Now, TimeSpan.FromHours(10), new List<int> { 1, 3, 4 },
                true, "some task 3", 1, true);

            taskRepo.Add(testDTO1);
            taskRepo.Add(testDTO3);
            taskRepo.Add(testDTO2);

            taskRepo.Delete(3);
            List<TaskDTO> list = taskRepo.GetAll();
            Assert.AreEqual(list[1], testDTO2);
            Assert.AreEqual(list[0], testDTO1);
            taskRepo.Reset();
        }

        [Test]
        public void TestTaskReset()
        {
            taskRepo.Reset();
            TaskDTO testDTO1 = new TaskDTO(1, 1, "task_1", false, "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(2, 1, "task_2", DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int> { 1, 2, 4 },
                true, "some task 2", 1, true);
            TaskDTO testDTO3 = new TaskDTO(3, 2, "task_3", DateTime.Now, DateTime.Now, TimeSpan.FromHours(10), new List<int> { 1, 3, 4 },
                true, "some task 3", 1, true);

            taskRepo.Add(testDTO1);
            taskRepo.Add(testDTO3);
            taskRepo.Add(testDTO2);

            taskRepo.Reset();
            Assert.Throws<TaskDoesNotExist>(delegate { taskRepo.Get(1); });
            taskRepo.Reset();
        }

        [Test]
        public void TestTaskArchive()
        {
            taskRepo.Reset();
            TaskDTO testDTO1 = new TaskDTO(1, 1, "task_1", false, "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(2, 1, "task_2", DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int> { 1, 2, 4 },
                true, "some task 2", 1, true);
            TaskDTO testDTO3 = new TaskDTO(3, 2, "task_3", DateTime.Now, DateTime.Now, TimeSpan.FromHours(10), new List<int> { 1, 3, 4 },
                true, "some task 3", 1, true);

            taskRepo.Add(testDTO1);
            taskRepo.Add(testDTO3);
            taskRepo.Add(testDTO2);

            taskRepo.Archive(1);
            taskRepo.Archive(2);

            List<TaskDTO> tasks = taskRepo.GetAll();
            List<TaskDTO> archived = taskRepo.GetAllArchived();

            Assert.AreEqual(archived[0], testDTO1);
            Assert.AreEqual(archived[1], testDTO2);
            Assert.AreEqual(tasks[0], testDTO3);

            taskRepo.Reset();
        }

        [Test]
        public void TestTaskUnarchive()
        {
            taskRepo.Reset();
            TaskDTO testDTO1 = new TaskDTO(1, 1, "task_1", false, "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(2, 1, "task_2", DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int> { 1, 2, 4 },
                true, "some task 2", 1, true);
            TaskDTO testDTO3 = new TaskDTO(3, 2, "task_3", DateTime.Now, DateTime.Now, TimeSpan.FromHours(10), new List<int> { 1, 3, 4 },
                true, "some task 3", 1, true);

            taskRepo.Add(testDTO1);
            taskRepo.Add(testDTO3);
            taskRepo.Add(testDTO2);

            taskRepo.Archive(1);
            taskRepo.Archive(2);

            taskRepo.Unarchive(1);

            List<TaskDTO> tasks = taskRepo.GetAll();
            List<TaskDTO> archived = taskRepo.GetAllArchived();

            Assert.AreEqual(archived[0], testDTO2);
            Assert.AreEqual(tasks[1], testDTO1);
            Assert.AreEqual(tasks[0], testDTO3);

            taskRepo.Reset();
        }
    }
}
