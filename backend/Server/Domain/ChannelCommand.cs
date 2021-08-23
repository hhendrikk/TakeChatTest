
namespace Server.Domain
{
    using Enum;
    using Infrastructure.Extensions;
    using System.Linq;

    public class ChannelCommand : BaseCommand
    {
        public string Channel { get; set; }

        public ChannelCommand() {}
        
        public ChannelCommand(string value)
        {
            var command = value.ToJsonDeserialize<ChannelCommand>();
            
            Type = command!.Type;
            Channel = command!.Channel;
            Message = command!.Message;
            Sender = command!.Sender;

            if (Type == CommandType.CreateChannel)
                Message = $"Hello, a new channel (#{Channel}) has been created!";
        }

        public override string ToString() => this.ToJsonSerialize();
    }
}