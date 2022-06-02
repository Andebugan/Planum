using Planum.ConsoleUI.ConsoleCommands;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;

namespace Planum.ConsoleUI
{
    public class ConsoleShell
    {
        protected List<ICommand> Commands;
        public ConsoleShell(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            List<ICommand> commands = new List<ICommand>() {
                new ExitCommand(),
                new DeleteUserCommand(userManager, taskManager, tagManager),
                new LogInCommand(userManager),
                new LogOutCommand(userManager),
                new ShowCurrentUserCommand(userManager),
                new ShowAllUsersCommand(userManager),
                new ShowUserCommand(userManager),
                new SignUpCommand(userManager),
                new UpdateUserCommand(userManager),

                new CreateTagCommand(tagManager, userManager),
                new DeleteTagCommand(tagManager, userManager),
                new DeleteAllTagsCommand(tagManager, userManager),
                new ShowAllTagsCommand(tagManager, userManager),
                new ShowTagCommand(tagManager, userManager),
                new UpdateTagCommand(tagManager, userManager),

                new AddChildCommand(taskManager, tagManager, userManager),
                new AddParentCommand(taskManager, userManager),
                new ArchiveTaskCommand(taskManager, userManager),
                new ClearChildrenCommand(taskManager, userManager),
                new ClearParentsCommand(taskManager, userManager),
                new ClearTagsCommand(taskManager, userManager),
                new CreateTaskCommand(taskManager, userManager, tagManager),
                new DeleteAllTasksCommand(taskManager, userManager),
                new DeleteTaskCommand(taskManager, userManager),
                new RemoveChildCommand(taskManager, userManager),
                new RemoveParentCommand(taskManager, userManager),
                new RemoveTagCommand(taskManager, userManager),
                new ShowAllArchivedTasksCommand(taskManager, userManager),
                new ShowAllTasksCommand(taskManager, userManager),
                new ShowArchivedTaskCommand(taskManager, userManager),
                new ShowTaskCommand(taskManager, userManager),
                new UnarchiveTaskCommand(taskManager, userManager),
                new UpdateTaskCommand(taskManager, userManager),
            };

            Commands = commands;
        }

        public void MainLoop()
        {
            Console.WriteLine("Welcome to Planum!");

            while (true)
            {
                Console.Write(">");
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine();
                    continue;
                }

                List<ICommand> avaliableCommands = new List<ICommand>();
                foreach (ICommand command in Commands)
                {
                    if (command.IsAvaliable())
                        avaliableCommands.Add(command);
                }

                if (input == "help")
                {
                    Console.WriteLine("Avalible commands:");
                    foreach (ICommand command in avaliableCommands)
                    {
                        Console.WriteLine("Name: " + command.GetName());
                        Console.WriteLine("Description: " + command.GetDescription());
                        Console.WriteLine();
                    }
                    continue;
                }

                bool executed = false;
                foreach (ICommand command in avaliableCommands)
                {
                    if (command.GetName() == input)
                    {
                        command.Execute();
                        executed = true;
                        break;
                    }
                }
                if (!executed)
                    Console.WriteLine("Error: command unavaliable or incorrect");
            }
        }
    }
}
