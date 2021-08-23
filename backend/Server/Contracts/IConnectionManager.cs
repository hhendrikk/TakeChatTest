namespace Server.Contracts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Threading.Tasks;

    public interface IConnectionManager
    {
        IEnumerable<string> GetAll();
        
        string Get(WebSocket socket);

        WebSocket Get(string id);
        
        Task<string> ConnectAsync(WebSocket socket);

        Task<IConnectionManager> DisconnectAsync(string id);

        bool IsOpen(string id);
    }
}