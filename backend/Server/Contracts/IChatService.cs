namespace Server.Contracts
{
    using System.Net.WebSockets;
    using System.Threading.Tasks;

    public interface IChatService
    {
        Task<string> OnConnectedAsync(string nickname, WebSocket socket);

        Task OnDisconnectedAsync(string id);

        Task SendCommandAsync(string command);
    }
}