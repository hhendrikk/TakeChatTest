namespace Server.Core
{
    using Contracts;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;

    public class SocketStore : ISocketStore
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets;

        public SocketStore()
        {
            _sockets = new ConcurrentDictionary<string, WebSocket>();
        }

        public IEnumerable<string> GetAll() => _sockets.Keys;

        public bool Add(string id, WebSocket socket) => _sockets.TryAdd(id, socket);

        public WebSocket Remove(string id)
        {
            _sockets.TryRemove(id, out var socket);
            return socket;
        }

        public WebSocket Get(string id)
        {
            if (!Has(id))
                throw new Exception("This socket is not exists!");
            
            return _sockets.First(s => s.Key == id).Value;
        }

        public string Get(WebSocket socket)
        {
            if (!Has(socket))
                throw new Exception("This socket is not exists!");
            
            return _sockets.First(s => s.Value == socket).Key;
        }

        public bool Has(string id) => _sockets.Any(s => s.Key == id);

        public bool Has(WebSocket socket) => _sockets.Any(s => s.Value == socket);
    }
}