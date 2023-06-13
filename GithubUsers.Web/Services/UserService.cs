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
            ServiceResponse<List<UserInfo>> serviceResponse = new();
            string endPointUrl = "https://localhost:7248/api/Users";
            try
            {
                serviceResponse = await _http.GetFromJsonAsync<ServiceResponse<List<UserInfo>>>(endPointUrl);
            }
            catch (HttpRequestException ex)
            {
                serviceResponse.isSuccess = false;
                serviceResponse.Message = ex.Message;
                serviceResponse.Data = new();
            }
            return serviceResponse;
        }

    }
}
