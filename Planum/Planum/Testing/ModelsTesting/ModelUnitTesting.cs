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

namespace Planum.Testing.ModelsTesting
{
    internal class ModelUnitTesting
    {
        public void TestTag(int num)
        {
            ITagRepo tagRepoFile = new TagRepoFile();
            ITaskRepo taskRepoFile = new TaskRepoFile();
            IUserRepo userRepoFile = new UserRepoFile();

            // manager constructor testing
            int cnt = 1;

            string pass = "not passed";

            try
            {
                ITagRepo tagRepo = null;
                TaskManager taskManager = null;
                TagManager tagManager = new TagManager(ref tagRepo, ref taskManager);
            }
            catch (ArgumentNullException)
            {
                pass = "passed";
            }
            Debug.Print(num.ToString() + "." + cnt.ToString() + " - " + pass);
            cnt += 1;

            pass = "not passed";
            try
            {
                ITagRepo tagRepo = new TagRepoFile();
                TaskManager taskManager = null;
                TagManager tagManager = new TagManager(ref tagRepo, ref taskManager);
            }
            catch (ArgumentNullException)
            {
                pass = "passed";
            }
            Debug.Print(num.ToString() + "." + cnt.ToString() + " - " + pass);
            cnt += 1;

            pass = "not passed";
            try
            {
                ITagRepo tagRepo = null;
                TaskManager taskManager = new TaskManager(ref taskRepoFile);
                TagManager tagManager = new TagManager(ref tagRepo, ref taskManager);
            }
            catch (ArgumentNullException)
            {
                pass = "passed";
            }
            Debug.Print(num.ToString() + "." + cnt.ToString() + " - " + pass);

            cnt += 1;

            pass = "passed";
            try
            {
                TaskManager taskManager = new TaskManager(ref taskRepoFile);
                TagManager tagManager = new TagManager(ref tagRepoFile, ref taskManager);
            }
            catch(Exception ex)
            {
                pass = "not passed: " + ex.ToString();
            }
            Debug.Print(num.ToString() + "." + cnt.ToString() + " - " + pass);
        }

        public void TestUser(int num)
        {
            ITagRepo tagRepoFile = new TagRepoFile();
            ITaskRepo taskRepoFile = new TaskRepoFile();
            IUserRepo userRepoFile = new UserRepoFile();

            int cnt = 1;

            string pass = "passed";

            // manager constructor testing
            // 1
            try
            {
                User user = null;
                IUserRepo userRepo = userRepoFile;
                TaskManager taskManager = new TaskManager(ref taskRepoFile);
                TagManager tagManager = new TagManager(ref tagRepoFile, ref taskManager);

                UserManager userManager = new UserManager(ref user, ref userRepo, ref tagManager, ref taskManager);
            }
            catch (Exception ex)
            {
                pass = "not passed: " + ex.ToString();
            }
            Debug.Print(num.ToString() + "." + cnt.ToString() + " - " + pass);
            cnt += 1;

            // 2
            pass = "not passed";
            try
            {
                User user = null;
                IUserRepo userRepo = null;
                TaskManager taskManager = new TaskManager(ref taskRepoFile);
                TagManager tagManager = new TagManager(ref tagRepoFile, ref taskManager);

                UserManager userManager = new UserManager(ref user, ref userRepo, ref tagManager, ref taskManager);
            }
            catch (ArgumentNullException)
            {
                pass = "passed";
            }
            Debug.Print(num.ToString() + "." + cnt.ToString() + " - " + pass);
            cnt += 1;

            // 3
            pass = "not passed";
            try
            {
                User user = null;
                IUserRepo userRepo = userRepoFile;
                TaskManager taskManager = new TaskManager(ref taskRepoFile);
                TagManager tagManager = new TagManager(ref tagRepoFile, ref taskManager);

                TaskManager taskManager1 = null;

                UserManager userManager = new UserManager(ref user, ref userRepo, ref tagManager, ref taskManager1);
            }
            catch (ArgumentNullException)
            {
                pass = "passed";
            }
            Debug.Print(num.ToString() + "." + cnt.ToString() + " - " + pass);
            cnt += 1;

            // 4
            pass = "not passed";
            try
            {
                User user = null;
                IUserRepo userRepo = userRepoFile;
                TaskManager taskManager = new TaskManager(ref taskRepoFile);
                TagManager tagManager = null;

                UserManager userManager = new UserManager(ref user, ref userRepo, ref tagManager, ref taskManager);
            }
            catch (ArgumentNullException)
            {
                pass = "passed";
            }
            Debug.Print(num.ToString() + "." + cnt.ToString() + " - " + pass);
        }

        public void TestTask(int num)
        {
            ITagRepo tagRepoFile = new TagRepoFile();
            ITaskRepo taskRepoFile = new TaskRepoFile();
            IUserRepo userRepoFile = new UserRepoFile();

            int cnt = 1;
            string pass = "passed";
            // manager constructor testing
            // manager constructor testing
            // 1
            try
            {
                ITaskRepo taskRepo = taskRepoFile;
                TaskManager taskManager = new TaskManager(ref taskRepo);
            }
            catch (Exception ex)
            {
                pass = "not passed: " + ex.ToString();
            }
            Debug.Print(num.ToString() + "." + cnt.ToString() + " - " + pass);
            cnt += 1;

            // 2
            pass = "not passed";
            try
            {
                ITaskRepo taskRepo = null;
                TaskManager taskManager = new TaskManager(ref taskRepo);
            }
            catch (ArgumentNullException)
            {
                pass = "passed";
            }
            Debug.Print(num.ToString() + "." + cnt.ToString() + " - " + pass);
        }

        public void Test()
        {
            Debug.Print("Starting tests:");
            TestTag(1);
            TestUser(2);
            TestTask(3);
        }
    }
}
