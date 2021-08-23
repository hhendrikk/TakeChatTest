namespace Server.Domain.Enum
{
    public enum CommandType
    {
        CreateChannel = 1,
        MessageChannel = 2,
        MessagePrivate = 3,
        JoinChannel = 4,
        Connected = 5,
        Disconnected = 6,
        Users = 7,
    }
}