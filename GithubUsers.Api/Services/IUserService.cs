using GithubUsers.Shared.Models;

namespace GithubUsers.Api.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<List<UserInfo>>> RetrieveUsers();
    }
}
