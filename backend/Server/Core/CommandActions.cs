namespace Server.Core
{
    using Contracts;
    using Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class CommandActions : ICommandActions
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IRepository _repository;

        public CommandActions(IConnectionManager connectionManager, IRepository repository)
        {
            _connectionManager = connectionManager;
            _repository = repository;
        }
        
        public async Task SendMessageChannelAsync(ChannelCommand command)
        {
            var usersChannel = _repository.GetUsersByChannel(command.Channel);
            await SendAsync(usersChannel, command.ToString());
        }

        public async Task CreateChannelAsync(ChannelCommand command)
        {
            _repository.SaveChannelUser(command.Channel, command.Sender);

            var allUsers = _repository.GetAllUsers();
            await SendAsync(allUsers, command.ToString());
        }

        public async Task JoinChannelAsync(ChannelCommand command)
        {
            _repository.SaveChannelUser(command.Channel, command.Sender);
            var usersChannel = _repository.GetUsersByChannel(command.Channel);
            await SendAsync(usersChannel, command.ToString());
        }

        public async Task SendMessagePrivateAsync(UserCommand command)
        {
            await SendAsync(new HashSet<User> { command.Sender, command.Receive }, command.ToString());
        }

        public async Task SendUsersAsync(UserCommand command)
        {
            command.Users = _repository
                .GetAllUsers();

            await SendAsync(command.Users, command.ToString());
        }
        
        private async Task SendAsync(IEnumerable<User> users, string message) =>
            await Task.WhenAll(users
                .Select(async user => await SendAsync(user.Id, message))
                .AsEnumerable());
        
        private async Task SendAsync(string id, string message)
        {
            var socket = _connectionManager.Get(id);

            if (!_connectionManager.IsOpen(id))
                return;

            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, message.Length);
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}