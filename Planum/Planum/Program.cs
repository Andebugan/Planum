using Newtonsoft.Json.Linq;
using Planum.Config;
using Planum.ConsoleUI;
using Planum.ConsoleUI.UI;
using Planum.Model.Entities;
using Planum.Model.Managers;
using Planum.Model.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

/*
 * v1.3.1
 * move to new parser
 * move to console ui
 * add async access support
 * move repo to plain text markdown style storage (I believe in plait text storage supremacy)
 *  - develop task format
 *  - add config for specifying what folders needed to be searched for markdown files for task extraction
 * add saving only when tasks are changed, otherwise use buffer
 * update logging system
 * add unit and functional testing
 * task time comparisons into methods
 * calendar fix
 *  use buffers
 *  add group by parent with search depth
 * streamline options, remove/replace useless ones and extend avaliable formats - combine name and ids into one
 * update and extend alias
 * add text search and search functionality (tab)
 */

namespace Planum
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AppConfig appConfig = new AppConfig();
            appConfig.LoadConfig();

            TaskManager taskManager = new TaskManager();
            taskManager.Backup();

            ConsoleShell consoleShell = new ConsoleShell(taskManager, appConfig);
            consoleShell.MainLoop(args);
        }
    }
}
