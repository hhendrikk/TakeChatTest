// unset

namespace Server.Contracts
{
    using Domain;
    using System.Threading.Tasks;

    public interface ICommandActions
    {
        Task SendMessageChannelAsync(ChannelCommand command);
        Task CreateChannelAsync(ChannelCommand command);
        Task JoinChannelAsync(ChannelCommand command);
        Task SendMessagePrivateAsync(UserCommand command);
        Task SendUsersAsync(UserCommand command);
    }
}