namespace Tests
{
    using Moq;
    using Server.Contracts;
    using Server.Core;
    using System;
    using System.Net.WebSockets;
    using Xunit;

    public class SocketStoreTests
    {
        [Fact]
        public void should_be_throw_exception_when_retrieving_invalid_socket()
        {
            var webSocketMock = new Mock<WebSocket>();

            ISocketStore store = new SocketStore();

            void ActionBySocket() => store.Get(webSocketMock.Object);
            void ActionById() => store.Get("123456788");

            var exceptionBySocket = Assert.Throws<Exception>(ActionBySocket);
            var exceptionById = Assert.Throws<Exception>(ActionById);
            
            Assert.Equal("This socket is not exists!", exceptionBySocket.Message);
            Assert.Equal("This socket is not exists!", exceptionById.Message);
        }
        
        [Fact]
        public void should_be_add_socket_on_store()
        {
            var webSocketMock = new Mock<WebSocket>();

            ISocketStore store = new SocketStore();
            store.Add("123456789", webSocketMock.Object);

            var socket = store.Get("123456789");

            Assert.Equal(webSocketMock.Object, socket);
        }
        
        [Fact]
        public void should_be_remove_socket_on_store()
        {
            var webSocketMock = new Mock<WebSocket>();

            ISocketStore store = new SocketStore();
            store.Add("123456789", webSocketMock.Object);
            var socket = store.Remove("123456789");
            var allSockets = store.GetAll();

            Assert.Empty(allSockets);
            Assert.Equal(webSocketMock.Object, socket);
        }
    }
}