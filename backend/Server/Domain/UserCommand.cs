namespace Server.Domain
{
    using Infrastructure.Extensions;
    using System.Collections.Generic;

    public class UserCommand : BaseCommand
    {
        public User Receive { get; set; }
        
        public IEnumerable<User> Users { get; set; }

        public UserCommand() {}
        
        public UserCommand(string value )
        {
            var command = value.ToJsonDeserialize<UserCommand>();

            Type = command!.Type;
            Sender = command!.Sender;
            Receive = command!.Receive;
            Message = command!.Message;
            Users = command!.Users;
        }
        
        public override string ToString() => this.ToJsonSerialize();
    }
}