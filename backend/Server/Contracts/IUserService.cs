namespace Server.Contracts
{
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<bool> ExistsAsync(string nickname);
    }
}