using Planum.Console;
using Planum.Console.Commands;
using Planum.Logger;

namespace PlanumConsole_Test
{
    public class ConsoleManager_Test
    {
        ILoggerWrapper logger;
        CommandManager commandManager;

        class DummyCommand : ICommand
        {
            public CommandInfo CommandInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public IEnumerable<IOption> CommandOptions => new List<IOption>();
            public bool WasExecuted { get; set; } = false;
            public bool CheckMatch(string value) => false;
            public List<string> Execute(ref IEnumerator<string> args) => new List<string>();
        }

        public ConsoleManager_Test()
        {
            logger = new PlanumLogger(LogLevel.INFO, clearFile: true);
            commandManager = new CommandManager(new List<ICommand>(), new DummyCommand(), logger);
        }

        class ConsoleManager_TestClass : ConsoleManager
        {
            public ConsoleManager_TestClass(CommandManager commandManager, ILoggerWrapper logger) : base(commandManager, logger) { }

            public IEnumerable<string> ParseArgs_Test(string input) => ParseArgs(input);
        }

        [Theory]
        [InlineData("", new string[] {})]
        [InlineData("test", new string[] { "test" })]
        [InlineData("test1 \"test2\" test3", new string[] { "test1", "test2", "test3" })]
        [InlineData("\"test1\" test2 \"test3\"", new string[] { "test1", "test2", "test3" })]
        [InlineData("\"test1\" \"test2\" \"test3\"", new string[] { "test1", "test2", "test3" })]
        public void ParseArgs_Test(string input, string[] expected)
        {
            // Arrange
            var logger = new PlanumLogger(clearFile: true);
            var consoleManager = new ConsoleManager_TestClass(commandManager, logger);

            // Act
            var actual = consoleManager.ParseArgs_Test(input);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
