namespace Server.Contracts
{
    using Domain;
    using System.Collections.Generic;

    public interface IRepository
    {
        IReadOnlyCollection<User> GetAllUsers();
        
        IReadOnlyCollection<User> GetUsersByChannel(string channel);
        
        bool SaveChannelUser(string channel, User user);

        void RemoveUserOnChannel(string id);

        User GetUserById(string id);

        bool Exists(string nickname);
    }
}