namespace Server.Core
{
    using Contracts;
    using Domain;
    using Infrastructure.Extensions;
    using System;
    using System.Data;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using CommandType = Domain.Enum.CommandType;

    public class CommandTranslate : ICommandTranslate
    {
        public ICommand Translate(string value)
        {
            var command = value.ToJsonDeserialize<BaseCommand>();
            return command!.Type switch
            {
                CommandType.CreateChannel => new ChannelCommand(value),
                CommandType.MessageChannel => new ChannelCommand(value),
                CommandType.JoinChannel => new ChannelCommand(value),
                CommandType.MessagePrivate => new UserCommand(value),
                CommandType.Connected => new ChannelCommand(value),
                CommandType.Disconnected => new ChannelCommand(value),
                CommandType.Users => new UserCommand(value),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}