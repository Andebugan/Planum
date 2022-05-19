using System;
using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;
using Planum.Models.DTO;
using Planum.Models.DataLayer;
using Planum.Models.BuisnessLayer.RepoInterfaces;

namespace Planum.Testing.UnitTesting
{
    [TestFixture]
    public class RepoUnitTesting
    {
        IUserRepo userRepo = new UserRepoFile();
        ITaskRepo taskRepo = new TaskRepoFile();
        ITagRepo tagRepo = new TagRepoFile();

        protected bool CompareTagDTOs(int id_1, TagDTO tagDTO_1, int id_2, TagDTO tagDTO_2)
        {
            if (id_1 != id_2)
                return false;
            if (tagDTO_1.UserId != tagDTO_2.UserId)
                return false;
            if (tagDTO_1.Category != tagDTO_2.Category)
                return false;
            if (tagDTO_1.Description != tagDTO_2.Description)
                return false;
            if (tagDTO_1.Name != tagDTO_2.Name)
                return false;
            return true;
        }

        protected bool CompareUserDTOs(int id_1, UserDTO userDTO_1, int id_2, UserDTO userDTO_2)
        {
            if (id_1 != id_2)
                return false;
            if (userDTO_1.Login != userDTO_2.Login)
                return false;
            if (userDTO_1.Password != userDTO_2.Password)
                return false;
            return true;
        }

        protected bool CompareTaskDTOs(int id_1, TaskDTO taskDTO_1, int id_2, TaskDTO taskDTO_2)
        {
            if (id_1 != id_2)
                return false;
            if (taskDTO_1.UserId != taskDTO_2.UserId)
                return false;
            if (taskDTO_1.ParentId != taskDTO_2.ParentId)
                return false;
            if (taskDTO_1.Name != taskDTO_2.Name)
                return false;
            if (taskDTO_1.Description != taskDTO_2.Description)
                return false;
            if (taskDTO_1.Timed != taskDTO_2.Timed)
                return false;
            if (Math.Abs((taskDTO_1.StartTime - taskDTO_2.StartTime).TotalSeconds) > 1)
                return false;
            if (Math.Abs((taskDTO_1.Deadline - taskDTO_2.Deadline).TotalSeconds) > 1)
                return false;
            List<int> temp_1 = (List<int>)taskDTO_1.TagIds;
            List<int> temp_2 = (List<int>)taskDTO_2.TagIds;
            if (!temp_1.SequenceEqual(temp_2))
                return false;
            if (taskDTO_1.IsRepeated != taskDTO_2.IsRepeated)
                return false;
            if (Math.Abs((taskDTO_1.RepeatPeriod - taskDTO_2.RepeatPeriod).TotalSeconds) > 1)
                return false;
            return true;
        }

        [SetUp]
        public void SetUp()
        {
            ((TagRepoFile)tagRepo).Reset();
            ((TaskRepoFile)taskRepo).Reset();
            ((UserRepoFile)userRepo).Reset();
        }

        [Test]
        public void TestUserAddGet()
        {
            ((UserRepoFile)userRepo).Reset();
            UserDTO testDTO1 = new UserDTO(1, "login_1", "password_1");
            UserDTO testDTO2 = new UserDTO(2, "login_2", "password_2");
            UserDTO testDTO3 = new UserDTO(3, "login_3", "password_3");

            int id_1 = userRepo.AddUser(testDTO1);
            int id_3 = userRepo.AddUser(testDTO3);
            int id_2 = userRepo.AddUser(testDTO2);

            UserDTO testDTO = userRepo.GetUser(id_1);
            Assert.IsTrue(CompareUserDTOs(id_1, testDTO1, testDTO.Id, testDTO));

            testDTO = userRepo.GetUser(id_2);
            Assert.IsTrue(CompareUserDTOs(id_2, testDTO2, testDTO.Id, testDTO));

            testDTO = userRepo.GetUser(id_3);
            Assert.IsTrue(CompareUserDTOs(id_3, testDTO3, testDTO.Id, testDTO));
        }

        [Test]
        public void TestUserGetAll()
        {
            ((UserRepoFile)userRepo).Reset();
            UserDTO testDTO1 = new UserDTO(1, "login_1", "password_1");
            UserDTO testDTO2 = new UserDTO(2, "login_2", "password_2");
            UserDTO testDTO3 = new UserDTO(3, "login_3", "password_3");

            int id_1 = userRepo.AddUser(testDTO1);
            int id_3 = userRepo.AddUser(testDTO3);
            int id_2 = userRepo.AddUser(testDTO2);

            List<UserDTO> testDTOs = userRepo.GetAllUsers();

            Assert.IsTrue(CompareUserDTOs(id_1, testDTO1, testDTOs[0].Id, testDTOs[0]));
            Assert.IsTrue(CompareUserDTOs(id_2, testDTO2, testDTOs[2].Id, testDTOs[2]));
            Assert.IsTrue(CompareUserDTOs(id_3, testDTO3, testDTOs[1].Id, testDTOs[1]));
            ((UserRepoFile)userRepo).Reset();
        }

        [Test]
        public void TestUserUpdate()
        {
            ((UserRepoFile)userRepo).Reset();
            UserDTO testDTO1 = new UserDTO(1, "login_1", "password_1");
            UserDTO testDTO2 = new UserDTO(2, "login_2", "password_2");
            UserDTO testDTO3 = new UserDTO(3, "login_3", "password_3");

            int id_1 = userRepo.AddUser(testDTO1);
            int id_3 = userRepo.AddUser(testDTO3);
            int id_2 = userRepo.AddUser(testDTO2);

            testDTO3 = new UserDTO(3, "bebra", "password_3");
            userRepo.UpdateUser(testDTO3);
            UserDTO temp = userRepo.GetUser(id_3);
            Assert.IsTrue(CompareUserDTOs(id_3, testDTO3, temp.Id, temp));
            ((UserRepoFile)userRepo).Reset();
        }

        [Test]
        public void TestUserDelete()
        {
            ((UserRepoFile)userRepo).Reset();
            UserDTO testDTO1 = new UserDTO(1, "login_1", "password_1");
            UserDTO testDTO2 = new UserDTO(2, "login_2", "password_2");
            UserDTO testDTO3 = new UserDTO(3, "login_3", "password_3");

            int id_1 = userRepo.AddUser(testDTO1);
            int id_3 = userRepo.AddUser(testDTO3);
            int id_2 = userRepo.AddUser(testDTO2);

            userRepo.DeleteUser(id_3);
            List<UserDTO> list = userRepo.GetAllUsers();
            Assert.IsTrue(CompareUserDTOs(list[0].Id, list[0], id_1, testDTO1));
            Assert.IsTrue(CompareUserDTOs(list[1].Id, list[1], id_2, testDTO2));
            ((UserRepoFile)userRepo).Reset();
        }

        [Test]
        public void TestUserReset()
        {
            ((UserRepoFile)userRepo).Reset();
            UserDTO testDTO1 = new UserDTO(1, "login_1", "password_1");
            UserDTO testDTO2 = new UserDTO(2, "login_2", "password_2");
            UserDTO testDTO3 = new UserDTO(3, "login_3", "password_3");

            int id_1 = userRepo.AddUser(testDTO1);
            int id_3 = userRepo.AddUser(testDTO3);
            int id_2 = userRepo.AddUser(testDTO2);

            ((UserRepoFile)userRepo).Reset();
            Assert.Throws<UserDoesNotExist>(delegate { userRepo.GetUser(id_1); });
            ((UserRepoFile)userRepo).Reset();
        }


        [Test]
        public void TestTagAddGet()
        {
            ((TagRepoFile)tagRepo).Reset();
            TagDTO testDTO1 = new TagDTO(1, 1, 1, "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, 2, "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, 3, "tag_3", "some tag 3");

            int id_1 = tagRepo.AddTag(testDTO1);
            int id_3 = tagRepo.AddTag(testDTO3);
            int id_2 = tagRepo.AddTag(testDTO2);

            TagDTO testDTO = tagRepo.GetTag(id_1);
            Assert.IsTrue(CompareTagDTOs(id_1, testDTO1, testDTO.Id, testDTO));

            testDTO = tagRepo.GetTag(id_2);
            Assert.IsTrue(CompareTagDTOs(id_2, testDTO2, testDTO.Id, testDTO));

            testDTO = tagRepo.GetTag(id_3);
            Assert.IsTrue(CompareTagDTOs(id_3, testDTO3, testDTO.Id, testDTO));
            ((TagRepoFile)tagRepo).Reset();
        }

        [Test]
        public void TestTagGetAll()
        {
            ((TagRepoFile)tagRepo).Reset();
            TagDTO testDTO1 = new TagDTO(1, 1, 1, "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, 2, "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, 3, "tag_3", "some tag 3");

            int id_1 = tagRepo.AddTag(testDTO1);
            int id_3 = tagRepo.AddTag(testDTO3);
            int id_2 = tagRepo.AddTag(testDTO2);

            List<TagDTO> testDTOs = tagRepo.GetAllTags();

            Assert.IsTrue(CompareTagDTOs(id_1, testDTO1, testDTOs[0].Id, testDTOs[0]));
            Assert.IsTrue(CompareTagDTOs(id_2, testDTO2, testDTOs[2].Id, testDTOs[2]));
            Assert.IsTrue(CompareTagDTOs(id_3, testDTO3, testDTOs[1].Id, testDTOs[1]));
            ((TagRepoFile)tagRepo).Reset();
        }

        [Test]
        public void TestTagUpdate()
        {
            ((TagRepoFile)tagRepo).Reset();
            TagDTO testDTO1 = new TagDTO(1, 1, 1, "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, 2, "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, 3, "tag_3", "some tag 3");

            int id_1 = tagRepo.AddTag(testDTO1);
            int id_3 = tagRepo.AddTag(testDTO3);
            int id_2 = tagRepo.AddTag(testDTO2);

            testDTO3 = new TagDTO(2, 3, 2, "tag_2", "new description");
            tagRepo.UpdateTag(testDTO3);
            TagDTO temp = tagRepo.GetTag(id_2);
            Assert.IsTrue(CompareTagDTOs(temp.Id, temp, testDTO3.Id, testDTO3));
            ((TagRepoFile)tagRepo).Reset();
        }

        [Test]
        public void TestTagDelete()
        {
            ((TagRepoFile)tagRepo).Reset();
            TagDTO testDTO1 = new TagDTO(1, 1, 1, "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, 2, "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, 3, "tag_3", "some tag 3");

            int id_1 = tagRepo.AddTag(testDTO1);
            int id_3 = tagRepo.AddTag(testDTO3);
            int id_2 = tagRepo.AddTag(testDTO2);

            tagRepo.DeleteTag(id_3);
            List<TagDTO> list = tagRepo.GetAllTags();

            Assert.IsTrue(CompareTagDTOs(list[0].Id, list[0], id_1, testDTO1));
            Assert.IsTrue(CompareTagDTOs(list[1].Id, list[1], id_2, testDTO2));

            ((TagRepoFile)tagRepo).Reset();
        }

        [Test]
        public void TestTagReset()
        {
            ((TagRepoFile)tagRepo).Reset();
            TagDTO testDTO1 = new TagDTO(1, 1, 1, "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, 2, "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, 3, "tag_3", "some tag 3");

            int id_1 = tagRepo.AddTag(testDTO1);
            int id_3 = tagRepo.AddTag(testDTO3);
            int id_2 = tagRepo.AddTag(testDTO2);

            ((TagRepoFile)tagRepo).Reset();
            Assert.Throws<TagDoesNotExistException>(delegate { tagRepo.GetTag(1); });
            ((TagRepoFile)tagRepo).Reset();
        }


        [Test]
        public void TestTaskAddGet()
        {
            ((TaskRepoFile)taskRepo).Reset();
            TaskDTO testDTO1 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 1, "task_1", "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int>(), false, 2, "task_2", "task_2 description", 5, false);
            TaskDTO testDTO3 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 3, "task_3", "task_3 description", 5, true);

            int id_1 = taskRepo.AddTask(testDTO1);
            int id_3 = taskRepo.AddTask(testDTO3);
            int id_2 = taskRepo.AddTask(testDTO2);

            TaskDTO testDTO = taskRepo.GetTask(id_1);
            Assert.IsTrue(CompareTaskDTOs(testDTO.Id, testDTO, id_1, testDTO1));

            testDTO = taskRepo.GetTask(id_2);
            Assert.IsTrue(CompareTaskDTOs(testDTO.Id, testDTO, id_2, testDTO2));

            testDTO = taskRepo.GetTask(id_3);
            Assert.IsTrue(CompareTaskDTOs(testDTO.Id, testDTO, id_3, testDTO3));
            ((TaskRepoFile)taskRepo).Reset();
        }

        [Test]
        public void TestTaskGetAll()
        {
            ((TaskRepoFile)taskRepo).Reset();
            TaskDTO testDTO1 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 1, "task_1", "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int>(), false, 2, "task_2", "task_2 description", 5, false);
            TaskDTO testDTO3 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 3, "task_3", "task_3 description", 5, true);

            int id_1 = taskRepo.AddTask(testDTO1);
            int id_3 = taskRepo.AddTask(testDTO3);
            int id_2 = taskRepo.AddTask(testDTO2);

            List<TaskDTO> testDTOs = taskRepo.GetAllTasks();

            Assert.IsTrue(CompareTaskDTOs(testDTOs[0].Id, testDTOs[0], id_1, testDTO1));
            Assert.IsTrue(CompareTaskDTOs(testDTOs[2].Id, testDTOs[2], id_2, testDTO2));
            Assert.IsTrue(CompareTaskDTOs(testDTOs[1].Id, testDTOs[1], id_3, testDTO3));
            ((TaskRepoFile)taskRepo).Reset();
        }

        [Test]
        public void TestTaskUpdate()
        {
            ((TaskRepoFile)taskRepo).Reset();
            TaskDTO testDTO1 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 1, "task_1", "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int>(), false, 2, "task_2", "task_2 description", 5, false);
            TaskDTO testDTO3 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 3, "task_3", "task_3 description", 5, true);

            int id_1 = taskRepo.AddTask(testDTO1);
            int id_3 = taskRepo.AddTask(testDTO3);
            int id_2 = taskRepo.AddTask(testDTO2);

            testDTO3 = new TaskDTO(id_3, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, -1, "task_1", "task_1 new description", 2, true);
            taskRepo.UpdateTask(testDTO3);
            TaskDTO temp = taskRepo.GetTask(id_3);
            Assert.IsTrue(CompareTaskDTOs(temp.Id, temp, id_3, testDTO3));
            ((TaskRepoFile)taskRepo).Reset();
        }

        [Test]
        public void TestTaskDelete()
        {
            ((TaskRepoFile)taskRepo).Reset();
            TaskDTO testDTO1 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 1, "task_1", "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int>(), false, 2, "task_2", "task_2 description", 5, false);
            TaskDTO testDTO3 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 3, "task_3", "task_3 description", 5, true);

            int id_1 = taskRepo.AddTask(testDTO1);
            int id_3 = taskRepo.AddTask(testDTO3);
            int id_2 = taskRepo.AddTask(testDTO2);

            taskRepo.DeleteTask(id_3);
            List<TaskDTO> list = taskRepo.GetAllTasks();

            Assert.IsTrue(CompareTaskDTOs(list[0].Id, list[0], id_1, testDTO1));
            Assert.IsTrue(CompareTaskDTOs(list[1].Id, list[1], id_2, testDTO2));
            ((TaskRepoFile)taskRepo).Reset();
        }

        [Test]
        public void TestTaskReset()
        {
            ((TaskRepoFile)taskRepo).Reset();
            TaskDTO testDTO1 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 1, "task_1", "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int>(), false, 2, "task_2", "task_2 description", 5, false);
            TaskDTO testDTO3 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 3, "task_3", "task_3 description", 5, true);

            int id_1 = taskRepo.AddTask(testDTO1);
            int id_3 = taskRepo.AddTask(testDTO3);
            int id_2 = taskRepo.AddTask(testDTO2);

            ((TaskRepoFile)taskRepo).Reset();
            Assert.Throws<TaskDoesNotExistException>(delegate { taskRepo.GetTask(id_1); });
            ((TaskRepoFile)taskRepo).Reset();
        }

        [Test]
        public void TestTaskArchive()
        {
            ((TaskRepoFile)taskRepo).Reset();
            TaskDTO testDTO1 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 1, "task_1", "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int>(), false, 2, "task_2", "task_2 description", 5, false);
            TaskDTO testDTO3 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 3, "task_3", "task_3 description", 5, true);

            int id_1 = taskRepo.AddTask(testDTO1);
            int id_3 = taskRepo.AddTask(testDTO3);
            int id_2 = taskRepo.AddTask(testDTO2);

            taskRepo.ArchiveTask(id_1);
            taskRepo.ArchiveTask(id_2);

            List<TaskDTO> tasks = taskRepo.GetAllTasks();
            List<TaskDTO> archived = taskRepo.GetAllArchivedTasks();

            Assert.IsTrue(CompareTaskDTOs(archived[0].Id, archived[0], id_1, testDTO1));
            Assert.IsTrue(CompareTaskDTOs(archived[1].Id, archived[1], id_2, testDTO2));
            Assert.IsTrue(CompareTaskDTOs(tasks[0].Id, tasks[0], id_3, testDTO3));

            ((TaskRepoFile)taskRepo).Reset();
        }

        [Test]
        public void TestTaskUnarchive()
        {
            ((TaskRepoFile)taskRepo).Reset();
            TaskDTO testDTO1 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 1, "task_1", "task_1 description", 5, false);
            TaskDTO testDTO2 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.FromHours(5), new List<int>(), false, 2, "task_2", "task_2 description", 5, false);
            TaskDTO testDTO3 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), false, 3, "task_3", "task_3 description", 5, true);

            int id_1 = taskRepo.AddTask(testDTO1);
            int id_3 = taskRepo.AddTask(testDTO3);
            int id_2 = taskRepo.AddTask(testDTO2);

            taskRepo.ArchiveTask(id_1);
            taskRepo.ArchiveTask(id_2);

            taskRepo.UnarchiveTask(id_1);

            List<TaskDTO> tasks = taskRepo.GetAllTasks();
            List<TaskDTO> archived = taskRepo.GetAllArchivedTasks();

            Assert.IsTrue(CompareTaskDTOs(archived[0].Id, archived[0], id_2, testDTO2));
            Assert.IsTrue(CompareTaskDTOs(tasks[1].Id, tasks[1], id_1, testDTO1));
            Assert.IsTrue(CompareTaskDTOs(tasks[0].Id, tasks[0], id_3, testDTO3));

            ((TaskRepoFile)taskRepo).Reset();
        }
    }
}