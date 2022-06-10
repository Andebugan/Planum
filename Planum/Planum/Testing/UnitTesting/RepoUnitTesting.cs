using System;
using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;
using Planum.Models.DTO;
using Planum.Models.DataModels;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.DataModels;

namespace Planum.Testing.UnitTesting
{
    [TestFixture]
    public class RepoUnitTesting
    {
        IUserRepo userRepo = new UserRepoFile(new UserDTOComparator());
        ITaskRepo taskRepo = new TaskRepoFile(new TaskDTOComparator());
        ITagRepo tagRepo = new TagRepoFile(new TagDTOComparator());

        protected bool CompareTagDTOs(int firstId, TagDTO firstDTO, int secondId, TagDTO secondDTO)
        {
            if (firstId != secondId)
                return false;
            if (firstDTO.UserId != secondDTO.UserId)
                return false;
            if (firstDTO.Category != secondDTO.Category)
                return false;
            if (firstDTO.Description != secondDTO.Description)
                return false;
            if (firstDTO.Name != secondDTO.Name)
                return false;
            return true;
        }

        protected bool CompareUserDTOs(int firstId, UserDTO firstDTO, int secondId, UserDTO secondDTO)
        {
            if (firstId != secondId)
                return false;
            if (firstDTO.Login != secondDTO.Login)
                return false;
            if (firstDTO.Password != secondDTO.Password)
                return false;
            return true;
        }

        protected bool CompareTaskDTOs(int firstId, TaskDTO firstDTO, int secondId, TaskDTO secondDTO)
        {
            if (firstId != secondId)
                return false;
            if (firstDTO.UserId != secondDTO.UserId)
                return false;
            if (firstDTO.Name != secondDTO.Name)
                return false;
            if (firstDTO.Description != secondDTO.Description)
                return false;
            if (firstDTO.Timed != secondDTO.Timed)
                return false;
            if (Math.Abs((firstDTO.StartTime - secondDTO.StartTime).TotalSeconds) > 1)
                return false;
            if (Math.Abs((firstDTO.Deadline - secondDTO.Deadline).TotalSeconds) > 1)
                return false;
            List<int> temp_1 = (List<int>)firstDTO.TagIds;
            List<int> temp_2 = (List<int>)secondDTO.TagIds;
            if (!temp_1.SequenceEqual(temp_2))
                return false;
            temp_1 = (List<int>)firstDTO.ChildIds;
            temp_2 = (List<int>)secondDTO.ChildIds;
            if (!temp_1.SequenceEqual(temp_2))
                return false;
            temp_1 = (List<int>)firstDTO.ParentIds;
            temp_2 = (List<int>)secondDTO.ParentIds;
            if (!temp_1.SequenceEqual(temp_2))
                return false;
            if (firstDTO.IsRepeated != secondDTO.IsRepeated)
                return false;
            if (Math.Abs((firstDTO.RepeatPeriod - secondDTO.RepeatPeriod).TotalSeconds) > 1)
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
            TagDTO testDTO1 = new TagDTO(1, 1, "category_1", "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, "category_2", "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, "category_3", "tag_3", "some tag 3");

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
            TagDTO testDTO1 = new TagDTO(1, 1, "category_1", "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, "category_2", "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, "category_3", "tag_3", "some tag 3");

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
            TagDTO testDTO1 = new TagDTO(1, 1, "category_1", "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, "category_2", "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, "category_3", "tag_3", "some tag 3");

            int id_1 = tagRepo.AddTag(testDTO1);
            int id_3 = tagRepo.AddTag(testDTO3);
            int id_2 = tagRepo.AddTag(testDTO2);

            testDTO3 = new TagDTO(2, 3, "category_2", "tag_2", "new description");
            tagRepo.UpdateTag(testDTO3);
            TagDTO temp = tagRepo.GetTag(id_2);
            Assert.IsTrue(CompareTagDTOs(temp.Id, temp, testDTO3.Id, testDTO3));
            ((TagRepoFile)tagRepo).Reset();
        }

        [Test]
        public void TestTagDelete()
        {
            ((TagRepoFile)tagRepo).Reset();
            TagDTO testDTO1 = new TagDTO(1, 1, "category_1", "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, "category_2", "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, "category_3", "tag_3", "some tag 3");

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
            TagDTO testDTO1 = new TagDTO(1, 1, "category_1", "tag_1", "some tag 1");
            TagDTO testDTO2 = new TagDTO(2, 2, "category_2", "tag_2", "some tag 2");
            TagDTO testDTO3 = new TagDTO(3, 3, "category_3", "tag_3", "some tag 3");

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
            TaskDTO testDTO1 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_1", false, 1, "task_1 description", false);
            TaskDTO testDTO2 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_2", false, 1, "task_2 description", false);
            TaskDTO testDTO3 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_3", false, 1, "task_3 description", false);

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
            TaskDTO testDTO1 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_1", false, 1, "task_1 description", false);
            TaskDTO testDTO2 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_2", false, 1, "task_2 description", false);
            TaskDTO testDTO3 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_3", false, 1, "task_3 description", false);

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
            TaskDTO testDTO1 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_1", false, 1, "task_1 description", false);
            TaskDTO testDTO2 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_2", false, 1, "task_2 description", false);
            TaskDTO testDTO3 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_3", false, 1, "task_3 description", false);

            int id_1 = taskRepo.AddTask(testDTO1);
            int id_3 = taskRepo.AddTask(testDTO3);
            int id_2 = taskRepo.AddTask(testDTO2);

            testDTO3 = new TaskDTO(id_3, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_3 new", false, 1, "task_3 new description", false);
            taskRepo.UpdateTask(testDTO3);
            TaskDTO temp = taskRepo.GetTask(id_3);
            Assert.IsTrue(CompareTaskDTOs(temp.Id, temp, id_3, testDTO3));
            ((TaskRepoFile)taskRepo).Reset();
        }

        [Test]
        public void TestTaskDelete()
        {
            ((TaskRepoFile)taskRepo).Reset();
            TaskDTO testDTO1 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_1", false, 1, "task_1 description", false);
            TaskDTO testDTO2 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_2", false, 1, "task_2 description", false);
            TaskDTO testDTO3 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_3", false, 1, "task_3 description", false);

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
            TaskDTO testDTO1 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_1", false, 1, "task_1 description", false);
            TaskDTO testDTO2 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_2", false, 1, "task_2 description", false);
            TaskDTO testDTO3 = new TaskDTO(1, DateTime.Now, DateTime.Now, TimeSpan.Zero, new List<int>(), new List<int>(), new List<int>(),
                "task_3", false, 1, "task_3 description", false);

            int id_1 = taskRepo.AddTask(testDTO1);
            int id_3 = taskRepo.AddTask(testDTO3);
            int id_2 = taskRepo.AddTask(testDTO2);

            ((TaskRepoFile)taskRepo).Reset();
            Assert.Throws<TaskDoesNotExistException>(delegate { taskRepo.GetTask(id_1); });
            ((TaskRepoFile)taskRepo).Reset();
        }
    }
}