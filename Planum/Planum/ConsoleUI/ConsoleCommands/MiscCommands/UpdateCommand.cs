using Alba.CsConsoleFormat;
using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class UpdateCommand: ICommand
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public UpdateCommand(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
        }

        public void UpdateUser(int id)
        {
            Log.Information("update user command was called");

            User? user = _userManager.FindUser(id);
            if (user == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"user with id={id}" +
                    $" does not exist\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("login: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(_userManager.CurrentUser.Login);
            string? login = _userManager.CurrentUser.Login;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change login (y/n): ");
            Console.ForegroundColor = ConsoleColor.White;
            string? input = null;
            input = Console.ReadLine();
            if (input != "y" && input != "n")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("should have entered y or x\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (input == "y")
            {
                login = "";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("new login: ");
                Console.ForegroundColor = ConsoleColor.White;
                login = Console.ReadLine();
                if (string.IsNullOrEmpty(login) || string.IsNullOrWhiteSpace(login))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("login can't be empty\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            if (_userManager.FindUser(login) != null && login != _userManager.CurrentUser.Login)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("user with this login already exist\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("password: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(_userManager.CurrentUser.Password);

            string? password = _userManager.CurrentUser.Password;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change login (y/n): ");
            Console.ForegroundColor = ConsoleColor.White;
            input = null;
            input = Console.ReadLine();
            if (input != "y" && input != "n")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("should have entered y or x\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (input == "y")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("new password: ");
                Console.ForegroundColor = ConsoleColor.White;
                password = "";
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        if (password != null && password.Length > 0)
                            password = password.Substring(0, password.Length - 1);
                    }
                    else
                        password += key.KeyChar;
                }

                if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("password can't be null\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("repeat password: ");
                Console.ForegroundColor = ConsoleColor.White;

                string? checkPassword = null;
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        if (checkPassword != null && checkPassword.Length > 0)
                            checkPassword = checkPassword.Substring(0, checkPassword.Length - 1);
                    }
                    else
                        checkPassword += key.KeyChar;
                }
                Console.WriteLine();

                if (password != checkPassword)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("passwords do not match\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            _userManager.UpdateUser(new User(user.Id, login, password));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("user updated successfully\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void UpdateTag(List<string> filters, Dictionary<string, bool> boolParams)
        {
            Log.Information("update tag command was called");

            bool updateAll = true;
            foreach(var val in boolParams.Values)
            {
                if (val == true)
                    updateAll = false;
            }

            int id = -1;
            foreach (var filter in filters)
            {
                if (filter.Substring(0, 2) == "-i")
                {
                    id = int.Parse(filter.Substring("-i".Length));
                }
            }

            Tag? tag = _tagManager.FindTag(id);
            if (tag == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"tag with id={id} does not exist\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            string? input = null;

            string? name = tag.Name;
            if (!updateAll && boolParams["updateName"])
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("name: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(tag.Name);

                name = "";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("new name: ");
                Console.ForegroundColor = ConsoleColor.White;
                name = Console.ReadLine();
                if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("name can't be empty\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            if (updateAll)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("name: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(tag.Name);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change name (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                
                input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                if (input == "y")
                {
                    name = "";
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("new name: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    name = Console.ReadLine();
                    if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("name can't be empty\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
            }


            string? category = tag.Category;
            if (!updateAll && boolParams["updateCategory"])
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("category: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(tag.Category);

                category = "";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("new category: ");
                Console.ForegroundColor = ConsoleColor.White;
                category = Console.ReadLine();
                if (category == null)
                    category = "";
            }

            if (updateAll)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("category: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(tag.Category);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change category (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = null;
                input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                if (input == "y")
                {
                    category = "";
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("new category: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    category = Console.ReadLine();
                    if (category == null)
                        category = "";
                }
            }

            string? description = tag.Description;
            if (!updateAll && boolParams["updateDescription"])
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("description: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(tag.Description);

                description = "";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("new description: ");
                Console.ForegroundColor = ConsoleColor.White;
                description = Console.ReadLine();
                if (description == null)
                    description = "";
            }

            if (updateAll)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("description: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(tag.Description);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change description (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = null;
                input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                if (input == "y")
                {
                    description = "";
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("new description: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    description = Console.ReadLine();
                    if (description == null)
                        description = "";
                }
            }

            _tagManager.UpdateTag(new Tag(tag.Id, tag.UserId, category, name, description));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"successfully updated tag with id={tag.Id}\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void UpdateTask(List<string> filters, Dictionary<string, bool> boolParams)
        {
            Log.Information("update task command was called");

            bool updateAll = true;
            foreach (var val in boolParams.Values)
            {
                if (val == true)
                    updateAll = false;
            }

            int id = -1;
            foreach (var filter in filters)
            {
                if (filter.Substring(0, 2) == "-i")
                {
                    id = int.Parse(filter.Substring("-i".Length));
                }
            }

            Task? task = _taskManager.FindTask(id);
            if (task == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("task with specified id does not exists\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            string? input;

            // name
            string? name = task.Name;
            if (!updateAll && boolParams["updateName"])
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("name: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(task.Name);

                name = "";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("new name: ");
                Console.ForegroundColor = ConsoleColor.White;
                name = Console.ReadLine();
                if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("name can't be null or empty\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            if (updateAll)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("name: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(task.Name);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change name (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = null;
                input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                if (input == "y")
                {
                    name = "";
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("new name: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    name = Console.ReadLine();
                    if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("name can't be null or empty\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
            }

            // description
            string? description = task.Description;
            if (!updateAll && boolParams["updateDescription"])
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("description: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(task.Description);

                description = "";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("new description: ");
                Console.ForegroundColor = ConsoleColor.White;
                description = Console.ReadLine();
                if (description == null)
                {
                    description = "";
                }
            }
            
            if (updateAll)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("description: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(task.Description);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change description (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = null;
                input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                if (input == "y")
                {
                    description = "";
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("new description: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    description = Console.ReadLine();
                    if (description == null)
                    {
                        description = "";
                    }
                }
            }


            List<int> tagIds = (List<int>)task.TagIds;

            // tag ids

            var doc = new Document();
            Grid grid = new Grid();

            if (!updateAll && boolParams["updateTags"])
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("tags:\n");
                Console.ForegroundColor = ConsoleColor.White;
                doc = new Document();

                grid = new Grid();
                grid.Color = ConsoleColor.DarkGray;

                grid.Columns.Add(GridLength.Auto);
                grid.Columns.Add(GridLength.Auto);

                foreach (var tag in task.TagIds)
                {
                    grid.Children.Add(new Cell(_tagManager.GetTag(tag).Id) { Align = Align.Left, Color = ConsoleColor.White });
                    grid.Children.Add(new Cell(_tagManager.GetTag(tag).Name) { Align = Align.Left, Color = ConsoleColor.White });
                }

                doc.Children.Add(grid);
                ConsoleRenderer.RenderDocument(doc);
                Console.WriteLine();

                tagIds = new List<int>();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter new tag ids: ");
                Console.ForegroundColor = ConsoleColor.White;

                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    string[] temp = input.Split(' ');
                    foreach (string s in temp)
                    {
                        int tempId;
                        if (!int.TryParse(s, out tempId))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("incorrect input\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }
                        tagIds.Add(tempId);
                    }
                }

                foreach (int tagId in tagIds)
                {
                    if (_tagManager.FindTag(tagId) == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect tag id\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
            }

            if (updateAll)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("tags:\n");
                Console.ForegroundColor = ConsoleColor.White;
                doc = new Document();

                grid = new Grid();
                grid.Color = ConsoleColor.DarkGray;

                grid.Columns.Add(GridLength.Auto);
                grid.Columns.Add(GridLength.Auto);

                foreach (var tag in task.TagIds)
                {
                    grid.Children.Add(new Cell(_tagManager.GetTag(tag).Id) { Align = Align.Left, Color = ConsoleColor.White });
                    grid.Children.Add(new Cell(_tagManager.GetTag(tag).Name) { Align = Align.Left, Color = ConsoleColor.White });
                }

                doc.Children.Add(grid);
                ConsoleRenderer.RenderDocument(doc);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change tags (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = null;
                input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                if (input == "y")
                {
                    tagIds = new List<int>();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("enter new tag ids: ");
                    Console.ForegroundColor = ConsoleColor.White;

                    input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input))
                    {
                        string[] temp = input.Split(' ');
                        foreach (string s in temp)
                        {
                            int tempId;
                            if (!int.TryParse(s, out tempId))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("incorrect input\n");
                                Console.ForegroundColor = ConsoleColor.White;
                                return;
                            }
                            tagIds.Add(tempId);
                        }
                    }

                    foreach (int tagId in tagIds)
                    {
                        if (_tagManager.FindTag(tagId) == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("incorrect tag id\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }
                    }
                }
            }

            // parent ids
            List<int> parentIds = (List<int>)task.ParentIds;
            if (!updateAll && boolParams["updateParent"])
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("parents:\n");
                Console.ForegroundColor = ConsoleColor.White;
                doc = new Document();

                grid = new Grid();
                grid.Color = ConsoleColor.DarkGray;

                grid.Columns.Add(GridLength.Auto);
                grid.Columns.Add(GridLength.Auto);

                foreach (var parent in task.ParentIds)
                {
                    grid.Children.Add(new Cell(_taskManager.GetTask(parent).Id) { Align = Align.Left, Color = ConsoleColor.White });
                    grid.Children.Add(new Cell(_taskManager.GetTask(parent).Name) { Align = Align.Left, Color = ConsoleColor.White });
                }

                doc.Children.Add(grid);
                ConsoleRenderer.RenderDocument(doc);
                Console.WriteLine();

                parentIds = new List<int>();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter new parent ids: ");
                Console.ForegroundColor = ConsoleColor.White;

                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    string[] temp = input.Split(' ');
                    foreach (string s in temp)
                    {
                        int tempId;
                        if (!int.TryParse(s, out tempId))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("incorrect input\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }
                        parentIds.Add(tempId);
                    }
                }

                foreach (int parentId in parentIds)
                {
                    if (_taskManager.FindTask(parentId) == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect parent id\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
            }

            if (updateAll)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("parents:\n");
                Console.ForegroundColor = ConsoleColor.White;
                doc = new Document();

                grid = new Grid();
                grid.Color = ConsoleColor.DarkGray;

                grid.Columns.Add(GridLength.Auto);
                grid.Columns.Add(GridLength.Auto);

                foreach (var parent in task.ParentIds)
                {
                    grid.Children.Add(new Cell(_taskManager.GetTask(parent).Id) { Align = Align.Left, Color = ConsoleColor.White });
                    grid.Children.Add(new Cell(_taskManager.GetTask(parent).Name) { Align = Align.Left, Color = ConsoleColor.White });
                }

                doc.Children.Add(grid);
                ConsoleRenderer.RenderDocument(doc);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change parents (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = null;
                input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                if (input == "y")
                {
                    parentIds = new List<int>();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("enter new parent ids: ");
                    Console.ForegroundColor = ConsoleColor.White;

                    input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input))
                    {
                        string[] temp = input.Split(' ');
                        foreach (string s in temp)
                        {
                            int tempId;
                            if (!int.TryParse(s, out tempId))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("incorrect input\n");
                                Console.ForegroundColor = ConsoleColor.White;
                                return;
                            }
                            parentIds.Add(tempId);
                        }
                    }

                    foreach (int parentId in parentIds)
                    {
                        if (_taskManager.FindTask(parentId) == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("incorrect parent id\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }
                    }
                }
            }

            // child ids
            List<int> childIds = (List<int>)task.ChildIds;
            if (!updateAll && boolParams["updateChild"])
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("children:\n");
                Console.ForegroundColor = ConsoleColor.White;
                doc = new Document();

                grid = new Grid();
                grid.Color = ConsoleColor.DarkGray;

                grid.Columns.Add(GridLength.Auto);
                grid.Columns.Add(GridLength.Auto);

                foreach (var children in task.ChildIds)
                {
                    grid.Children.Add(new Cell(_taskManager.GetTask(children).Id) { Align = Align.Left, Color = ConsoleColor.White });
                    grid.Children.Add(new Cell(_taskManager.GetTask(children).Name) { Align = Align.Left, Color = ConsoleColor.White });
                }

                doc.Children.Add(grid);
                ConsoleRenderer.RenderDocument(doc);
                Console.WriteLine();

                parentIds = new List<int>();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter new child ids: ");
                Console.ForegroundColor = ConsoleColor.White;

                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    string[] temp = input.Split(' ');
                    foreach (string s in temp)
                    {
                        int tempId;
                        if (!int.TryParse(s, out tempId))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("incorrect input\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }
                        childIds.Add(tempId);
                    }
                }

                foreach (int childId in childIds)
                {
                    if (_taskManager.FindTask(childId) == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect child id\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
            }

            if (updateAll)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("children:\n");
                Console.ForegroundColor = ConsoleColor.White;
                doc = new Document();

                grid = new Grid();
                grid.Color = ConsoleColor.DarkGray;

                grid.Columns.Add(GridLength.Auto);
                grid.Columns.Add(GridLength.Auto);

                foreach (var children in task.ChildIds)
                {
                    grid.Children.Add(new Cell(_taskManager.GetTask(children).Id) { Align = Align.Left, Color = ConsoleColor.White });
                    grid.Children.Add(new Cell(_taskManager.GetTask(children).Name) { Align = Align.Left, Color = ConsoleColor.White });
                }

                doc.Children.Add(grid);
                ConsoleRenderer.RenderDocument(doc);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change children (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = null;
                input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                if (input == "y")
                {
                    parentIds = new List<int>();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("enter new child ids: ");
                    Console.ForegroundColor = ConsoleColor.White;

                    input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input))
                    {
                        string[] temp = input.Split(' ');
                        foreach (string s in temp)
                        {
                            int tempId;
                            if (!int.TryParse(s, out tempId))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("incorrect input\n");
                                Console.ForegroundColor = ConsoleColor.White;
                                return;
                            }
                            childIds.Add(tempId);
                        }
                    }

                    foreach (int childId in childIds)
                    {
                        if (_taskManager.FindTask(childId) == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("incorrect child id\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }
                    }
                }
            }


            // status ids
            int currentStatusIndex = task.CurrentStatusIndex;
            List<int> statusQueueIds = (List<int>)task.StatusQueueIds;
            if (!updateAll && boolParams["updateStatus"])
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("status queue:\n");
                Console.ForegroundColor = ConsoleColor.White;
                doc = new Document();

                grid = new Grid();
                grid.Color = ConsoleColor.DarkGray;

                grid.Columns.Add(GridLength.Auto);
                grid.Columns.Add(GridLength.Auto);

                foreach (var status in task.StatusQueueIds)
                {
                    grid.Children.Add(new Cell(_tagManager.GetTag(status).Id) { Align = Align.Left, Color = ConsoleColor.White });
                    grid.Children.Add(new Cell(_tagManager.GetTag(status).Name) { Align = Align.Left, Color = ConsoleColor.White });
                }

                doc.Children.Add(grid);
                ConsoleRenderer.RenderDocument(doc);
                Console.WriteLine();

                statusQueueIds = new List<int>();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter status ids: ");
                Console.ForegroundColor = ConsoleColor.White;

                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    string[] temp = input.Split(' ');
                    foreach (string s in temp)
                    {
                        int tempId;
                        if (!int.TryParse(s, out tempId))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("incorrect input\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }
                        statusQueueIds.Add(tempId);
                    }
                }

                foreach (int statusId in statusQueueIds)
                {
                    if (_tagManager.FindTag(statusId) == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect status id\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }

                // current status id
                currentStatusIndex = 0;
                if (statusQueueIds.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("enter current status index: ");
                    Console.ForegroundColor = ConsoleColor.White;

                    input = Console.ReadLine();
                    if (int.TryParse(input, out currentStatusIndex))
                    {
                        if (currentStatusIndex < 0 || currentStatusIndex >= statusQueueIds.Count)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("current status index must be in range from 0 to status queue length\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect input\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("current status index: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(task.CurrentStatusIndex);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change current status index (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                if (input == "y")
                {
                    // current status id
                    if (statusQueueIds.Count > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("enter current status index: ");
                        Console.ForegroundColor = ConsoleColor.White;

                        input = Console.ReadLine();
                        if (int.TryParse(input, out currentStatusIndex))
                        {
                            if (currentStatusIndex < 0 || currentStatusIndex >= statusQueueIds.Count)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("current status index must be in range from 0 to status queue length\n");
                                Console.ForegroundColor = ConsoleColor.White;
                                return;
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("incorrect input\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }
                    }
                }
            }

            if (updateAll)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("status queue:\n");
                Console.ForegroundColor = ConsoleColor.White;
                doc = new Document();

                grid = new Grid();
                grid.Color = ConsoleColor.DarkGray;

                grid.Columns.Add(GridLength.Auto);
                grid.Columns.Add(GridLength.Auto);

                foreach (var status in task.StatusQueueIds)
                {
                    grid.Children.Add(new Cell(_tagManager.GetTag(status).Id) { Align = Align.Left, Color = ConsoleColor.White });
                    grid.Children.Add(new Cell(_tagManager.GetTag(status).Name) { Align = Align.Left, Color = ConsoleColor.White });
                }

                doc.Children.Add(grid);
                ConsoleRenderer.RenderDocument(doc);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change statuses (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                if (input == "y")
                {
                    statusQueueIds = new List<int>();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("enter status ids: ");
                    Console.ForegroundColor = ConsoleColor.White;

                    input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input))
                    {
                        string[] temp = input.Split(' ');
                        foreach (string s in temp)
                        {
                            int tempId;
                            if (!int.TryParse(s, out tempId))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("incorrect input\n");
                                Console.ForegroundColor = ConsoleColor.White;
                                return;
                            }
                            statusQueueIds.Add(tempId);
                        }
                    }

                    foreach (int statusId in statusQueueIds)
                    {
                        if (_tagManager.FindTag(statusId) == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("incorrect status id\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }
                    }

                    // current status id
                    currentStatusIndex = 0;
                    if (statusQueueIds.Count > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("enter current status index: ");
                        Console.ForegroundColor = ConsoleColor.White;

                        input = Console.ReadLine();
                        if (int.TryParse(input, out currentStatusIndex))
                        {
                            if (currentStatusIndex < 0 || currentStatusIndex >= statusQueueIds.Count)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("current status index must be in range from 0 to status queue length\n");
                                Console.ForegroundColor = ConsoleColor.White;
                                return;
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("incorrect input\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("current status index: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(task.CurrentStatusIndex);

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("change current status index (y/n): ");
                    Console.ForegroundColor = ConsoleColor.White;
                    input = Console.ReadLine();
                    if (input != "y" && input != "n")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("should have entered y or x\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                    
                    if (input == "y")
                    {
                        // current status id
                        if (statusQueueIds.Count > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("enter current status index: ");
                            Console.ForegroundColor = ConsoleColor.White;

                            input = Console.ReadLine();
                            if (int.TryParse(input, out currentStatusIndex))
                            {
                                if (currentStatusIndex < 0 || currentStatusIndex >= statusQueueIds.Count)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("current status index must be in range from 0 to status queue length\n");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    return;
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("incorrect input\n");
                                Console.ForegroundColor = ConsoleColor.White;
                                return;
                            }
                        }
                    }
                }
            }

            // timed
            bool timed = task.Timed;
            if (!updateAll && boolParams["updateTimed"])
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("timed (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input) && input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                if (input == "y")
                    timed = true;
                if (input == "n")
                    timed = false;
            }

            DateTime startTime = task.StartTime;
            if (!updateAll && boolParams["updateStartTime"])
            {
                if (task.StartTime == DateTime.MinValue)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("start time not set");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("start time: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(task.StartTime.Year.ToString() + "-" +
                                task.StartTime.Month.ToString() + "-" +
                                task.StartTime.Day.ToString() + " " +
                                task.StartTime.Hour.ToString() + ":" +
                                task.StartTime.Minute.ToString());
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter task start time as yyyy-mm-dd hh:mm : ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("incorrect input\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            DateTime deadline = task.Deadline;
            if (!updateAll && boolParams["updateDeadline"])
            {
                if (task.Deadline == DateTime.MinValue)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("deadline not set\n");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("deadline: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(task.Deadline.Year.ToString() + "-" +
                                task.Deadline.Month.ToString() + "-" +
                                task.Deadline.Day.ToString() + " " +
                                task.Deadline.Hour.ToString() + ":" +
                                task.Deadline.Minute.ToString());
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter task deadline as yyyy-mm-dd hh:mm : ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out deadline))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("incorrect input\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            bool isRepeated = false;
            TimeSpan repeatPeriod = task.RepeatPeriod;
            if (!updateAll && boolParams["updateRepeatPeriod"])
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("repeated (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input) && input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                if (input == "n")
                {
                    isRepeated = false;
                }
                else if (input == "y")
                {
                    isRepeated = true;
                    // repeat period
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("repeat period: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(task.RepeatPeriod.Days.ToString() + "d "
                        + task.RepeatPeriod.Hours.ToString() + "h "
                        + task.RepeatPeriod.Minutes.ToString() + "m");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("enter task repeat period as d:hh:mm : ");
                    Console.ForegroundColor = ConsoleColor.White;

                    input = Console.ReadLine();
                    if (!TimeSpan.TryParseExact(input, @"d\:hh\:mm", CultureInfo.InvariantCulture, TimeSpanStyles.None, out repeatPeriod))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect input\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
            }

            if (updateAll)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("timed (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                if (input == "n")
                {
                    Task newTask = new Task(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod,
                        tagIds, parentIds, childIds, name, false,
                        task.UserId, description, StatusQueueIds: statusQueueIds);
                    newTask.SetStatusIndex(currentStatusIndex);
                    _taskManager.UpdateTask(newTask);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("succsessfully updated task\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change time parameters (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("should have entered y or x\n\n");
                    return;
                }

                if (input == "n")
                {
                    Task newTask = new Task(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod,
                        tagIds, parentIds, childIds, name, task.Timed,
                        task.UserId, description, StatusQueueIds: statusQueueIds);
                    newTask.SetStatusIndex(currentStatusIndex);
                    _taskManager.UpdateTask(newTask);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("succsessfully updated task\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                // start time
                if (task.StartTime == DateTime.MinValue)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("start time not set");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("start time: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(task.StartTime.Year.ToString() + "-" +
                                task.StartTime.Month.ToString() + "-" +
                                task.StartTime.Day.ToString() + " " +
                                task.StartTime.Hour.ToString() + ":" +
                                task.StartTime.Minute.ToString());
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change start time (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                startTime = task.StartTime;

                if (input == "y")
                {

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("enter task start time as yyyy-mm-dd hh:mm : ");
                    Console.ForegroundColor = ConsoleColor.White;
                    input = Console.ReadLine();
                    if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect input\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }

                // deadline
                if (task.Deadline == DateTime.MinValue)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("deadline not set\n");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("deadline: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(task.Deadline.Year.ToString() + "-" +
                                task.Deadline.Month.ToString() + "-" +
                                task.Deadline.Day.ToString() + " " +
                                task.Deadline.Hour.ToString() + ":" +
                                task.Deadline.Minute.ToString());
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change deadline (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                deadline = task.Deadline;

                if (input == "y")
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("enter task deadline as yyyy-mm-dd hh:mm : ");
                    Console.ForegroundColor = ConsoleColor.White;
                    input = Console.ReadLine();
                    if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out deadline))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect input\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }

                // isRepeated
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("repeated (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                if (input == "n")
                {
                    if (startTime > deadline)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("start time can't be after deadline\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }

                    Task newTask = new Task(task.Id, startTime, deadline, TimeSpan.Zero, tagIds, parentIds, childIds, name, true,
                        task.UserId, description, false, task.Archived, statusQueueIds);
                    newTask.SetStatusIndex(currentStatusIndex);
                    _taskManager.UpdateTask(newTask);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"succsessfully updated task with id={task.Id}\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                // repeat period
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("repeat period: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(task.RepeatPeriod.Days.ToString() + "d "
                    + task.RepeatPeriod.Hours.ToString() + "h "
                    + task.RepeatPeriod.Minutes.ToString() + "m");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("change repeat period (y/n): ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("should have entered y or x\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                repeatPeriod = task.RepeatPeriod;

                if (input == "y")
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("enter task repeat period d:mm:hh : ");
                    Console.ForegroundColor = ConsoleColor.White;

                    input = Console.ReadLine();
                    if (!TimeSpan.TryParseExact(input, @"d\:hh\:mm", CultureInfo.InvariantCulture, TimeSpanStyles.None, out repeatPeriod))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect input\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }

                Task newTask__ = new Task(task.Id, startTime, deadline, repeatPeriod, tagIds, parentIds, childIds, name, true,
                            task.UserId, description, true, task.Archived, statusQueueIds);
                newTask__.SetStatusIndex(currentStatusIndex);
                _taskManager.UpdateTask(newTask__);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("succsessfully updated task\n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (startTime > deadline)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("start time can't be after deadline\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Task newTask_ = new Task(task.Id, startTime, deadline, repeatPeriod, tagIds, parentIds, childIds, name, timed,
                        task.UserId, description, isRepeated, task.Archived, statusQueueIds);
            newTask_.SetStatusIndex(currentStatusIndex);
            _taskManager.UpdateTask(newTask_);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"succsessfully updated task with id={task.Id}\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Execute(string command)
        {
            string[] args = command.Split(' ');
            if (args.Length == 2 && args[1] == "user")
            {
                UpdateUser(_userManager.CurrentUser.Id);
                return;
            }

            if (args[args.Length - 1] == "tag")
            {
                bool parseSuccessfull = true;

                Dictionary<string, bool> boolParams = new Dictionary<string, bool>()
                {
                    { "updateName", false },
                    { "updateCategory", false },
                    { "updateDescription", false }
                };

                List<string> filters = new List<string>();

                List<string> argsList = new List<string>(args);
                argsList.Remove("update");
                argsList.Remove("tag");
                TagCommandParser parser = new TagCommandParser();
                parseSuccessfull = parser.Parse(ref filters, argsList, ref boolParams, "update");

                if (parseSuccessfull)
                {
                    UpdateTag(filters, boolParams);
                    return;
                }
            }

            if (args[args.Length - 1] == "task")
            {
                bool parseSuccessfull = true;

                Dictionary<string, bool> boolParams = new Dictionary<string, bool>()
                {
                    { "updateName", false },
                    { "updateDescription", false },
                    { "updateTags", false },
                    { "updateStatus", false },
                    { "updateParent", false },
                    { "updateChild", false },
                    { "updateStartTime", false },
                    { "updateDeadline", false },
                    { "updateRepeatPeriod", false },
                    { "updateTimed", false },
                };

                List<string> filters = new List<string>();

                List<string> argsList = new List<string>(args);
                argsList.Remove("update");
                argsList.Remove("task");
                TaskCommandParser parser = new TaskCommandParser();
                parseSuccessfull = parser.Parse(ref filters, argsList, ref boolParams, "update");

                if (parseSuccessfull)
                {
                    UpdateTask(filters, boolParams);
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("incorrect command parameters\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string GetDescription()
        {
            return "updates specified object|objects";
        }

        public string GetName()
        {
            return "update -i={value} [-n] [-d] [-t] [-p] [-c] [-s] [-tp] task\n" +
                "   flags:\n" +
                "       -n - update name\n" +
                "       -d - update description\n" +
                "       -t - update tags\n" +
                "       -p - update parents\n" +
                "       -c - update children\n" +
                "       -s - update status queue or/and current status index\n" +
                "       -st - update start time\n" +
                "       -dl - update deadline\n" +
                "       -r - repeat period\n" +
                "       -tm - update timed (has task time limitations, or not)\n" +
                "       note: if you use time options with timed==false, time info for task\n" +
                "       will be changed, but not displayed\n" +
                "update -i={value} [-n] [-c] [-d] tag\n" +
                "   flags:\n" +
                "       -n - update name\n" +
                "       -c - update category\n" +
                "       -d - update description\n" +
                "update user";
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "update")
                return true;
            return false;
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            else
                return false;
        }
    }
}
