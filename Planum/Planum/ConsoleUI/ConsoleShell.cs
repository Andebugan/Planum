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
            };

            Commands = commands;
        }

        protected void WriteGreeting()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.Cyan;
            string text = "╭────╮                                      \n" +
                          "│ ╭╮ │ ╭─╮    ╭────╮ ╭────╮ ╭─╮╭─╮ ╭───────╮\n" +
                          "│ ╰╯ │ │ │    │ ╭╮ │ │ ╭╮ │ │ ││ │ │ ╭╮ ╭╮ │\n" +
                          "│ ╭──╯ │ │    │ ╰╯ │ │ ││ │ │ ││ │ │ ││ ││ │\n" +
                          "│ │    │ ╰──╮ │ ╭╮ │ │ ││ │ │ ╰╯ │ │ ││ ││ │\n" +
                          "╰─╯    ╰────╯ ╰─╯╰─╯ ╰─╯╰─╯ ╰────╯ ╰─╯╰─╯╰─╯\n";
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        public void MainLoop()
        {
            WriteGreeting();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.White;
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
                        Console.WriteLine("Name:\n" + command.GetName());
                        Console.WriteLine("Description:\n" + command.GetDescription());
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
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Incorrect command!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}
