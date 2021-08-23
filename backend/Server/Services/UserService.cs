namespace Server.Services
{
    using Contracts;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly IRepository _repository;

        public UserService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> ExistsAsync(string nickname) => await Task.FromResult(_repository.Exists(nickname));
    }
}