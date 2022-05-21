using System;
using System.Collections.Generic;

namespace Planum.ConsoleUI
{
    public class ConsoleShell
    {
        protected List<ICommand> Commands;
        public ConsoleShell(List<ICommand> commands)
        {
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
                        Console.WriteLine();
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
