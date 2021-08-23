namespace Api.Controllers
{
    using System.Net;
    using System.Net.WebSockets;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Server.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly IDictionary<WebSocketMessageType, Action<string, WebSocketReceiveResult, byte[]>> _socketActions;

        public ChatController(IChatService chatService, IUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
            _socketActions = new Dictionary<WebSocketMessageType, Action<string, WebSocketReceiveResult, byte[]>>
            {
                {
                    WebSocketMessageType.Text, ((_, result, buffer) =>
                    {
                        var command = $"{Encoding.UTF8.GetString(buffer, 0, result.Count)}";
                        _chatService.SendCommandAsync(command);
                    })
                },
                { WebSocketMessageType.Close, (id, _, _) => _chatService.OnDisconnectedAsync(id) }
            };
        }

        [HttpGet("/ws/{nickname}")]
        public async Task WebSocket(string nickname)
        {
            if (this.HttpContext.WebSockets.IsWebSocketRequest)
            {
                using WebSocket socket = await this.HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketId = await _chatService.OnConnectedAsync(nickname, socket);
                await ReceiveAsync(socket, (result, buffer) => _socketActions[result.MessageType](socketId, result, buffer));
                return;
            }
            
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        
        [HttpGet("/api/users/{nickname}")]
        public async Task<bool> Get(string nickname) => await _userService.ExistsAsync(nickname);

        private static async Task ReceiveAsync(WebSocket socket, Action<WebSocketReceiveResult, byte[]> action)
        {
            var buffer = new byte[1024 * 4];
            while(socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                action(result, buffer);
            }
        }
    }
}