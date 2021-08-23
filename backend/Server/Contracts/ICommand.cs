namespace Server.Contracts
{
    using Domain.Enum;

    public interface ICommand
    {
        CommandType Type { get; set; }

        string ToString();
    }
}