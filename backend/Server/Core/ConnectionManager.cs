namespace Server.Core
{
    using Contracts;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    
    public class ConnectionManager : IConnectionManager
    {
        private readonly IIdGenerator _idGenerator;
        private readonly ISocketStore _store;

        public ConnectionManager(IIdGenerator idGenerator, ISocketStore storeStore)
        {
            _idGenerator = idGenerator;
            _store = storeStore;
        }
        
        public IEnumerable<string> GetAll() => _store.GetAll();

        public async Task<string> ConnectAsync(WebSocket socket)
        {
            var id = _idGenerator.New();
            _store.Add(id, socket);
            return await Task.FromResult(id);
        }

        public string Get(WebSocket socket) => _store.Get(socket);
        
        public WebSocket Get(string id) => _store.Get(id);

        public async Task<IConnectionManager> DisconnectAsync(string id)
        {
            var socket = _store.Remove(id);

            if (socket == default)
                return this;

            await socket.CloseAsync(
                closeStatus: WebSocketCloseStatus.NormalClosure,
                statusDescription: "Disconnected!",
                cancellationToken: CancellationToken.None);

            return this;
        }

        public bool IsOpen(string id)
        {
            var socket = _store.Get(id);
            return socket.State == WebSocketState.Open;
        }
    }
}