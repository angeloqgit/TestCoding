using GithubUsers.Shared.Models;
using System.Threading.Tasks;

namespace GithubUsers.Web.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;

        public UserService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<List<UserInfo>>> RetreiveUsers()
        {
            string endPointUrl = "https://localhost:7248/api/Users";
            return await _http.GetFromJsonAsync<ServiceResponse<List<UserInfo>>>(endPointUrl);
        }

    }
}
