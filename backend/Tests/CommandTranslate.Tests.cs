namespace Tests
{
    using Server.Contracts;
    using Server.Core;
    using Server.Domain;
    using Server.Domain.Enum;
    using Xunit;

    public class CommandTranslateTests
    {
        [Fact]
        public void should_be_translate_command_create_channel()
        {
            var value = "{\"channel\":\"new\",\"type\":1,\"sender\":{\"id\":\"123456789\",\"nickname\":\"user1\"}}";
            ICommandTranslate command = new CommandTranslate();
            var result = command.Translate(value);

            Assert.Equal(CommandType.CreateChannel, result.Type);
            Assert.IsType<ChannelCommand>(result);
            Assert.Equal("Hello, a new channel (#new) has been created!", ((ChannelCommand)result).Message);
            Assert.Equal("new", ((ChannelCommand)result).Channel);
            Assert.Equal("user1", ((ChannelCommand)result).Sender.Nickname);
            Assert.Equal("123456789", ((ChannelCommand)result).Sender.Id);

            var expectToString = "{\"channel\":\"new\",\"type\":1,\"sender\":{\"id\":\"123456789\",\"nickname\":\"user1\"},\"message\":\"Hello, a new channel (#new) has been created!\"}";

            Assert.Equal(expectToString, ((ChannelCommand)result).ToString());
        }

        [Fact]
        public void should_be_translate_command_channel_message()
        {
            var value = "{\"channel\":\"new\",\"type\":2,\"sender\":{\"id\":\"123456789\",\"nickname\":\"user1\"},\"message\":\"Hello friends!\"}";
            ICommandTranslate command = new CommandTranslate();
            var result = command.Translate(value);

            Assert.Equal(CommandType.MessageChannel, result.Type);
            Assert.IsType<ChannelCommand>(result);
            Assert.Equal("Hello friends!", ((ChannelCommand)result).Message);
            Assert.Equal("new", ((ChannelCommand)result).Channel);
            Assert.Equal("user1", ((ChannelCommand)result).Sender.Nickname);
            Assert.Equal("123456789", ((ChannelCommand)result).Sender.Id);
            Assert.Equal(value, ((ChannelCommand)result).ToString());
        }

        [Fact]
        public void should_be_translate_command_joined_channel()
        {
            var value = "{\"channel\":\"new\",\"type\":4,\"sender\":{\"id\":\"112233\",\"nickname\":\"user3\"},\"message\":\"user3 has joined @new\"}";
            ICommandTranslate command = new CommandTranslate();
            var result = command.Translate(value);

            Assert.Equal(CommandType.JoinChannel, result.Type);
            Assert.IsType<ChannelCommand>(result);
            Assert.Equal("user3 has joined @new", ((ChannelCommand)result).Message);
            Assert.Equal("new", ((ChannelCommand)result).Channel);
            Assert.Equal("user3", ((ChannelCommand)result).Sender.Nickname);
            Assert.Equal("112233", ((ChannelCommand)result).Sender.Id);
            Assert.Equal(value, ((ChannelCommand)result).ToString());
        }

        [Fact]
        public void should_be_translate_command_message_private()
        {
            var value = "{\"receive\":{\"id\":\"987654321\",\"nickname\":\"user2\"},\"users\":null,\"type\":3,\"sender\":{\"id\":\"123456789\",\"nickname\":\"user1\"},\"message\":\"Hi friend!\"}";
            ICommandTranslate command = new CommandTranslate();
            var result = command.Translate(value);

            Assert.Equal(CommandType.MessagePrivate, result.Type);
            Assert.IsType<UserCommand>(result);
            Assert.Equal("Hi friend!", ((UserCommand)result).Message);
            Assert.Equal("user2", ((UserCommand)result).Receive.Nickname);
            Assert.Equal("987654321", ((UserCommand)result).Receive.Id);
            Assert.Equal("user1", ((UserCommand)result).Sender.Nickname);
            Assert.Equal("123456789", ((UserCommand)result).Sender.Id);
            Assert.Equal(value, ((UserCommand)result).ToString());
        }
    }
}