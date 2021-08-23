namespace Server.Contracts
{
    public interface ICommandTranslate
    {
        ICommand Translate(string command);
    }
}