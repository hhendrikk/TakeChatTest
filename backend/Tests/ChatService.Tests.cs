namespace Tests
{
    using Moq;
    using Server.Contracts;
    using Server.Core;
    using Server.Domain;
    using Server.Domain.Enum;
    using Server.Services;
    using System.Net.WebSockets;
    using System.Threading.Tasks;
    using Xunit;

    public class ChatServiceTests
    {
        [Fact]
        public async Task should_be_connect()
        {
            var connectionManagerMock = new Mock<IConnectionManager>();
            var repositoryMock = new Mock<IRepository>();
            var commandTranslateMock = new Mock<ICommandTranslate>();
            var commandActionsMock = new Mock<ICommandActions>();
            var socketMock = new Mock<WebSocket>();

            repositoryMock.Setup(x => x.SaveChannelUser("general", It.Is<User>(y => y.Id == "123456789" && y.Nickname == "user1"))).Returns(true);
            connectionManagerMock.Setup(x => x.ConnectAsync(socketMock.Object)).ReturnsAsync("123456789");
            commandTranslateMock.Setup(x => x.Translate("{\"channel\":\"general\",\"type\":5,\"sender\":{\"id\":\"123456789\",\"nickname\":\"user1\"},\"message\":\"user1 has joined channel #general\"}")).Returns(new ChannelCommand
            {
                Channel = "general",
                Message = "user1 has joined channel #general",
                Sender = new User("user1", "123456789"),
                Type = CommandType.MessageChannel
            });
            
            commandTranslateMock.Setup(x => x.Translate("{\"receive\":null,\"users\":null,\"type\":7,\"sender\":{\"id\":\"123456789\",\"nickname\":\"user1\"},\"message\":\"All users\"}")).Returns(new UserCommand
            {
                Message = "All users",
                Sender = new User("user1", "123456789"),
                Type = CommandType.Users
            });

            var chatService = new ChatService(connectionManagerMock.Object, repositoryMock.Object, commandTranslateMock.Object, commandActionsMock.Object);

            var socketId = await chatService.OnConnectedAsync("user1", socketMock.Object);
            
            Assert.Equal("123456789", socketId);
            
            repositoryMock.VerifyAll();
            connectionManagerMock.VerifyAll();
            commandTranslateMock.VerifyAll();
        }

        [Fact]
        public async Task should_be_disconnect()
        {
            var connectionManagerMock = new Mock<IConnectionManager>();
            var repositoryMock = new Mock<IRepository>();
            var commandTranslateMock = new Mock<ICommandTranslate>();
            var commandActionsMock = new Mock<ICommandActions>();
            
            connectionManagerMock.Setup(x => x.DisconnectAsync("123456789")).ReturnsAsync(connectionManagerMock.Object);
            repositoryMock.Setup(x => x.RemoveUserOnChannel("123456789"));

            var chatService = new ChatService(connectionManagerMock.Object, repositoryMock.Object, commandTranslateMock.Object, commandActionsMock.Object);

            await chatService.OnDisconnectedAsync("123456789");
            
            connectionManagerMock.VerifyAll();
            repositoryMock.VerifyAll();
        }
    }
}