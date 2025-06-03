namespace MHServerEmu.Games.Commands
{
    public interface ICommandParser<T>
    {
        public bool TryParse(string message, T invoker);
    }
}
