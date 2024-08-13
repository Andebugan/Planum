using System;

namespace Planum.Commands
{
    public class ConsoleManager 
    {
        protected CommandManager CommandManager { get; set; }

        public ConsoleManager(CommandManager commandManager)
        {
            CommandManager = commandManager;
        }

        protected string GetGreeting()
        {
            return "" +
                "╭────╮                                      \n" +
                "│ ╭╮ │ ╭─╮    ╭────╮ ╭────╮ ╭─╮╭─╮ ╭───────╮\n" +
                "│ ╰╯ │ │ │    │ ╭╮ │ │ ╭╮ │ │ ││ │ │ ╭╮ ╭╮ │\n" +
                "│ ╭──╯ │ │    │ ╰╯ │ │ ││ │ │ ││ │ │ ││ ││ │\n" +
                "│ │    │ ╰──╮ │ ╭╮ │ │ ││ │ │ ╰╯ │ │ ││ ││ │\n" +
                "╰─╯    ╰────╯ ╰─╯╰─╯ ╰─╯╰─╯ ╰────╯ ╰─╯╰─╯╰─╯ " + "2.0" +
                "";
        }

        public void RunConsoleMode()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            // print greeting
        }

        public void RunCommandMode(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }
    }
}
