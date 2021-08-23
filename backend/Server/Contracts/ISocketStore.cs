namespace Server.Contracts
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.WebSockets;

    public interface ISocketStore
    {
        bool Add(string id, WebSocket webSocket);

        WebSocket Remove(string id);

        IEnumerable<string> GetAll();

        WebSocket Get(string id);
        
        string Get(WebSocket socket);

        bool Has(string id);

        bool Has(WebSocket socket);
    }
}