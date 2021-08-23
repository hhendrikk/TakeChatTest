namespace Tests
{
    using Moq;
    using Server.Contracts;
    using Server.Core;
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class ConnectionManagerTests
    {
        private readonly ISocketStore _store;

        public ConnectionManagerTests()
        {
            _store = new SocketStore();
        }

        [Fact]
        public async Task should_be_add_get_socket_connection()
        {
            var webSocketMock = new Mock<WebSocket>();
            var idGeneratorMock = new Mock<IIdGenerator>();

            idGeneratorMock.Setup(x => x.New()).Returns("123456789");

            IConnectionManager connectionManager = new ConnectionManager(idGeneratorMock.Object, _store);
            await connectionManager.ConnectAsync(webSocketMock.Object);

            var socketId = connectionManager.Get(webSocketMock.Object);

            Assert.Equal("123456789", socketId);
        }

        [Fact]
        public async Task should_be_disconnect_socket()
        {
            var webSocketMock = new Mock<WebSocket>();
            var idGeneratorMock = new Mock<IIdGenerator>();

            webSocketMock.Setup(x => x.CloseAsync(
                It.Is<WebSocketCloseStatus>(y => y == WebSocketCloseStatus.NormalClosure),
                It.Is<string>(y => y == "Disconnected!"),
                It.Is<CancellationToken>(y => y == CancellationToken.None)));

            idGeneratorMock.Setup(x => x.New()).Returns("123456789");

            IConnectionManager connectionManager = new ConnectionManager(idGeneratorMock.Object, _store);
            await connectionManager.ConnectAsync(webSocketMock.Object);
            await connectionManager.DisconnectAsync("123456789");

            void ActionGet() => connectionManager.Get("123456789");
            var exception = Assert.Throws<Exception>(ActionGet);

            Assert.Equal("This socket is not exists!", exception.Message);
            webSocketMock.VerifyAll();
        }

        [Fact]
        public async Task should_be_verify_socket_has_open()
        {
            var webSocketMock = new Mock<WebSocket>();
            var idGeneratorMock = new Mock<IIdGenerator>();

            idGeneratorMock.Setup(x => x.New()).Returns("123456789");

            IConnectionManager connectionManager = new ConnectionManager(idGeneratorMock.Object, _store);
            await connectionManager.ConnectAsync(webSocketMock.Object);
            var closed =  connectionManager.IsOpen("123456789");

            Assert.False(closed);
        }
    }
}