using GithubUsers.Shared.Models;

namespace GithubUsers.Web.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<List<UserInfo>>> RetreiveUsers();
    }
}
