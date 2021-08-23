namespace Server.Core
{
    using Contracts;
    using Domain;
    using System.Collections.Generic;
    using System.Linq;

    public class RepositoryMemory : IRepository
    {
        private readonly ISet<Channel> _channels;

        public RepositoryMemory()
        {
            _channels = new HashSet<Channel> { new("general", new HashSet<User>()) };
        }

        public IReadOnlyCollection<User> GetAllUsers() =>
            _channels
                .SelectMany(c => c.Users)
                .ToArray();

        public IReadOnlyCollection<User> GetUsersByChannel(string channel) =>
            _channels
                .Where(c => c.Name == channel)
                .SelectMany(c => c.Users)
                .ToArray();

        public bool SaveChannelUser(string channel, User user)
        {
            var existsChannel = _channels.Any(c => c.Name == channel);

            if (existsChannel)
            {
                return
                    _channels
                        .Where(c => c.Name == channel)
                        .Select(c => c.Users.Add(user))
                        .Any();
            }

            return _channels.Add(new Channel(channel, new HashSet<User> { user }));
        }

        public void RemoveUserOnChannel(string id)
        {
            foreach (var channel in _channels)
            {
                channel.Users = channel.Users.Where(u => u.Id != id).ToHashSet();
            }
        }

        public User GetUserById(string id) =>
            _channels
                .Where(c => c.Users.Any(u => u.Id == id))
                .SelectMany(c => c.Users)
                .First(u => u.Id == id);

        public bool Exists(string nickname) => _channels.Any(c => c.Users.Any(u => u.Nickname == nickname));
    }
}