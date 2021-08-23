namespace Server.Domain
{
    using System.Collections.Generic;

    public record Channel
    {
        public string Name { get; set; }

        public ISet<User> Users { get; set; }

        public Channel(string name, ISet<User> users)
        {
            Name = name;
            Users = users;
        }
    };
}