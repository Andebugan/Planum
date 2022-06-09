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
                new LogInCommand(userManager),
                new LogOutCommand(userManager),
                new SignUpCommand(userManager),
                new CreateCommand(tagManager, userManager, taskManager),
                new DeleteCommand(userManager, taskManager, tagManager),
                new ShowCommand(userManager, taskManager, tagManager),
                new UpdateCommand(userManager, taskManager, tagManager)
            };

            Commands = commands;
        }

        protected void WriteGreeting()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.Blue;
            string text = "╭────╮                                      \n" +
                          "│ ╭╮ │ ╭─╮    ╭────╮ ╭────╮ ╭─╮╭─╮ ╭───────╮\n" +
                          "│ ╰╯ │ │ │    │ ╭╮ │ │ ╭╮ │ │ ││ │ │ ╭╮ ╭╮ │\n" +
                          "│ ╭──╯ │ │    │ ╰╯ │ │ ││ │ │ ││ │ │ ││ ││ │\n" +
                          "│ │    │ ╰──╮ │ ╭╮ │ │ ││ │ │ ╰╯ │ │ ││ ││ │\n" +
                          "╰─╯    ╰────╯ ╰─╯╰─╯ ╰─╯╰─╯ ╰────╯ ╰─╯╰─╯╰─╯\n";
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void MainLoop()
        {
            WriteGreeting();
            while (true)
            {
                Console.Write("> ");
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
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("avalible commands:");
                    Console.ForegroundColor = ConsoleColor.White;

                    foreach (ICommand command in avaliableCommands)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("name: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(command.GetName());
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("description:");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(command.GetDescription());
                        Console.WriteLine();
                    }
                    continue;
                }

                bool executed = false;
                foreach (ICommand command in avaliableCommands)
                {
                    if (command.IsCommand(input))
                    {
                        command.Execute(input.Split());
                        executed = true;
                        break;
                    }
                }
                
                if (!executed)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("incorrect command\n");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}
