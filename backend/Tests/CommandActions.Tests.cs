// unset

namespace Tests
{
    using Moq;
    using Server.Contracts;
    using Server.Core;
    using Server.Domain;
    using Server.Domain.Enum;
    using System;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class CommandActionsTests
    {
        [Fact]
        public async Task should_be_send_channel_message()
        {
            var connectionManagerMock = new Mock<IConnectionManager>();
            var repository = new Mock<IRepository>();
            var socket1 = new Mock<WebSocket>();
            var socket2 = new Mock<WebSocket>();
            repository.Setup(x => x.GetUsersByChannel("general")).Returns(new HashSet<User> { new ("user1", "123456789"), new ("user2", "987654321") });
            
            connectionManagerMock.Setup(x => x.Get("123456789")).Returns(socket1.Object);
            connectionManagerMock.Setup(x => x.Get("987654321")).Returns(socket2.Object);
            connectionManagerMock.Setup(x => x.IsOpen("123456789")).Returns(true);
            connectionManagerMock.Setup(x => x.IsOpen("987654321")).Returns(true);

            socket1.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, CancellationToken.None));
            socket2.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, CancellationToken.None));

            var commandActions = new CommandActions(connectionManagerMock.Object, repository.Object);
            var command = new ChannelCommand
            {
                Channel = "general",
                Message = "Message",
                Sender = new User("user1", "123456789"),
                Type = CommandType.MessageChannel
            };

            await commandActions.SendMessageChannelAsync(command);
            
            connectionManagerMock.VerifyAll();
            repository.VerifyAll();
            socket1.VerifyAll();
            socket2.VerifyAll();
        }
        
        [Fact]
        public async Task should_be_send_create_channel()
        {
            var connectionManagerMock = new Mock<IConnectionManager>();
            var repository = new Mock<IRepository>();
            var socket1 = new Mock<WebSocket>();
            var socket2 = new Mock<WebSocket>();
            repository.Setup(x => x.SaveChannelUser("general", It.Is<User>(y => y.Id == "123456789" && y.Nickname == "user1"))).Returns(true);
            repository.Setup(x => x.GetAllUsers()).Returns(new HashSet<User> { new("user1", "123456789"), new("user2", "987654321") });
            
            connectionManagerMock.Setup(x => x.Get("123456789")).Returns(socket1.Object);
            connectionManagerMock.Setup(x => x.Get("987654321")).Returns(socket2.Object);
            connectionManagerMock.Setup(x => x.IsOpen("123456789")).Returns(true);
            connectionManagerMock.Setup(x => x.IsOpen("987654321")).Returns(true);

            socket1.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, CancellationToken.None));
            socket2.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, CancellationToken.None));

            var commandActions = new CommandActions(connectionManagerMock.Object, repository.Object);
            var command = new ChannelCommand
            {
                Channel = "general",
                Sender = new User("user1", "123456789"),
                Type = CommandType.CreateChannel
            };

            await commandActions.CreateChannelAsync(command);
            
            connectionManagerMock.VerifyAll();
            repository.VerifyAll();
            socket1.VerifyAll();
            socket2.VerifyAll();
        }
        
        [Fact]
        public async Task should_be_send_join_channel()
        {
            var connectionManagerMock = new Mock<IConnectionManager>();
            var repository = new Mock<IRepository>();
            var socket1 = new Mock<WebSocket>();
            var socket2 = new Mock<WebSocket>();
            repository.Setup(x => x.SaveChannelUser("general", It.Is<User>(y => y.Id == "123456789" && y.Nickname == "user1"))).Returns(true);
            repository.Setup(x => x.GetUsersByChannel("general")).Returns(new HashSet<User> { new ("user1", "123456789"), new ("user2", "987654321") });

            connectionManagerMock.Setup(x => x.Get("123456789")).Returns(socket1.Object);
            connectionManagerMock.Setup(x => x.Get("987654321")).Returns(socket2.Object);
            connectionManagerMock.Setup(x => x.IsOpen("123456789")).Returns(true);
            connectionManagerMock.Setup(x => x.IsOpen("987654321")).Returns(true);

            socket1.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, CancellationToken.None));
            socket2.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, CancellationToken.None));

            var commandActions = new CommandActions(connectionManagerMock.Object, repository.Object);
            var command = new ChannelCommand
            {
                Channel = "general",
                Sender = new User("user1", "123456789"),
                Type = CommandType.JoinChannel
            };

            await commandActions.JoinChannelAsync(command);
            
            connectionManagerMock.VerifyAll();
            repository.VerifyAll();
            socket1.VerifyAll();
            socket2.VerifyAll();
        }
        
        [Fact]
        public async Task should_be_send_message_private()
        {
            var connectionManagerMock = new Mock<IConnectionManager>();
            var repository = new Mock<IRepository>();
            var socket1 = new Mock<WebSocket>();
            var socket2 = new Mock<WebSocket>();

            connectionManagerMock.Setup(x => x.Get("123456789")).Returns(socket1.Object);
            connectionManagerMock.Setup(x => x.Get("987654321")).Returns(socket2.Object);
            connectionManagerMock.Setup(x => x.IsOpen("123456789")).Returns(true);
            connectionManagerMock.Setup(x => x.IsOpen("987654321")).Returns(true);

            socket1.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, CancellationToken.None));
            socket2.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, CancellationToken.None));

            var commandActions = new CommandActions(connectionManagerMock.Object, repository.Object);
            var command = new UserCommand
            {
                Sender = new User("user1", "123456789"),
                Receive = new User("user2", "987654321"),
                Type = CommandType.MessagePrivate
            };

            await commandActions.SendMessagePrivateAsync(command);
            
            connectionManagerMock.VerifyAll();
            repository.VerifyAll();
            socket1.VerifyAll();
            socket2.VerifyAll();
        }
        
        [Fact]
        public async Task should_be_send_all_users()
        {
            var connectionManagerMock = new Mock<IConnectionManager>();
            var repository = new Mock<IRepository>();
            var socket1 = new Mock<WebSocket>();
            var socket2 = new Mock<WebSocket>();
            repository.Setup(x => x.GetAllUsers()).Returns(new HashSet<User> { new("user1", "123456789"), new("user2", "987654321") });

            connectionManagerMock.Setup(x => x.Get("123456789")).Returns(socket1.Object);
            connectionManagerMock.Setup(x => x.Get("987654321")).Returns(socket2.Object);
            connectionManagerMock.Setup(x => x.IsOpen("123456789")).Returns(true);
            connectionManagerMock.Setup(x => x.IsOpen("987654321")).Returns(true);

            socket1.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, CancellationToken.None));
            socket2.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, CancellationToken.None));

            var commandActions = new CommandActions(connectionManagerMock.Object, repository.Object);
            var command = new UserCommand
            {
                Users = new HashSet<User> { new("user1", "123456789"), new("user2", "987654321") },
                Type = CommandType.Users
            };

            await commandActions.SendUsersAsync(command);
            
            connectionManagerMock.VerifyAll();
            repository.VerifyAll();
            socket1.VerifyAll();
            socket2.VerifyAll();
        }
    }
}