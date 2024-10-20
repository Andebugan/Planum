using System.Collections;
using Planum.Console.Commands;
using Planum.Logger;

namespace PlanumConsole_Test.CommandManager_Test
{
    public class CommandManager_Test
    {
        ILoggerWrapper logger;

        class DummyCommand : ICommand
        {
            public CommandInfo CommandInfo { get; set; }

            public DummyCommand(CommandInfo commandInfo)
            {
                CommandInfo = commandInfo;
            }

            public IEnumerable<IOption> CommandOptions => new List<IOption>();
            public bool WasExecuted { get; set; } = false;
            public bool CheckMatch(string value) => value.Trim() == CommandInfo.Name;
            public List<string> Execute(ref IEnumerator<string> args) => new List<string> { CommandInfo.Name };
        }

        public CommandManager_Test()
        {
            logger = new PlanumLogger(LogLevel.INFO, clearFile: true);
        }

        [Theory]
        [ClassData(typeof(TryExecuteCommand_TestData))]
        public void TryExecuteCommand_Test(IEnumerable<ICommand> commands, ICommand exitCommand, IEnumerable<string> lines, IEnumerable<string> expected)
        {
            // Arrange
            CommandManager manager = new CommandManager(commands, exitCommand, logger);

            // Act
            var actual = manager.TryExecuteCommand(lines);

            // Assert
            Assert.Equal(expected, actual);
        }

        class TryExecuteCommand_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var exitCommand = new DummyCommand(new CommandInfo("exit", "exit command", ""));
                var commands = new List<ICommand> {
                    new DummyCommand(new CommandInfo("test", "test command", "")),
                    exitCommand
                };
                yield return new object[] {
                    commands,
                    exitCommand,
                    new List<string> {
                        "test"
                    },
                    new List<string> {
                        "test",
                    }
                };
                yield return new object[] {
                    commands,
                    exitCommand,
                    new List<string> {
                        "exit"
                    },
                    new List<string> {
                        "exit",
                    }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
