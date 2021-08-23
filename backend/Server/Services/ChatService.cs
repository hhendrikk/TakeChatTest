namespace Server.Services
{
    using Contracts;
    using Domain;
    using Domain.Enum;
    using System;
    using System.Net.WebSockets;
    using System.Threading.Tasks;

    public class ChatService : IChatService
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IRepository _repository;
        private readonly ICommandTranslate _commandTranslate;
        private readonly ICommandActions _commandActions;

        public ChatService(IConnectionManager connectionManager, IRepository repository, ICommandTranslate commandTranslate, ICommandActions commandActions)
        {
            _connectionManager = connectionManager;
            _repository = repository;
            _commandTranslate = commandTranslate;
            _commandActions = commandActions;
        }

        const string GeneralChannel = "general";

        public async Task<string> OnConnectedAsync(string nickname, WebSocket socket)
        {
            var socketId = await _connectionManager.ConnectAsync(socket);
            var user = new User(nickname, socketId);

            _repository.SaveChannelUser(GeneralChannel, user);

            var command = new ChannelCommand
            {
                Channel = GeneralChannel, Type = CommandType.Connected, Message = $"{nickname} has joined channel #{GeneralChannel}", Sender = user,
            };

            await SendCommandAsync(command.ToString());

            var commandUsers = new UserCommand { Message = "All users", Sender = user, Type = CommandType.Users };

            await SendCommandAsync(commandUsers.ToString());

            return socketId;
        }

        public async Task OnDisconnectedAsync(string socketId)
        {
            _repository.RemoveUserOnChannel(socketId);
            await _connectionManager.DisconnectAsync(socketId);
        }

        public async Task SendCommandAsync(string value)
        {
            var command = _commandTranslate.Translate(value);

            switch (command.Type)
            {
                case CommandType.MessageChannel:
                case CommandType.Connected:
                case CommandType.Disconnected:
                    await _commandActions.SendMessageChannelAsync(command as ChannelCommand);
                    break;
                case CommandType.CreateChannel:
                    await _commandActions.CreateChannelAsync(command as ChannelCommand);
                    break;
                case CommandType.JoinChannel:
                    if (command is ChannelCommand cmd)
                    {
                        cmd.Message = $"{cmd.Sender.Nickname} has joined channel #{cmd.Channel}";
                        await _commandActions.JoinChannelAsync(cmd);
                    }
                    break;
                case CommandType.MessagePrivate:
                    await _commandActions.SendMessagePrivateAsync(command as UserCommand);
                    break;
                case CommandType.Users:
                    await _commandActions.SendUsersAsync(command as UserCommand);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}