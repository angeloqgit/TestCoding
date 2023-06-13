using GithubUsers.Shared.Models;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace GithubUsers.Web.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;
        private readonly AppSettingsModel _settings;

        public UserService(HttpClient http, IOptions<AppSettingsModel> settings)
        {
            _http = http;
            _settings = settings.Value;
        }

        public async Task<ServiceResponse<List<UserInfo>>> RetreiveUsers()
        {
            ServiceResponse<List<UserInfo>> serviceResponse = new();
            try
            {
                serviceResponse = await _http.GetFromJsonAsync<ServiceResponse<List<UserInfo>>>(_settings.EndPoint);
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
