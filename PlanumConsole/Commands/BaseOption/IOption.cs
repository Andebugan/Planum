namespace Planum.Console.Commands
{
    public interface IOption
    {
        public OptionInfo OptionInfo { get; set; }
        
        public bool CheckMatch(string value);
    }
}
