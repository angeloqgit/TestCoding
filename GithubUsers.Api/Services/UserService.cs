using GithubUsers.Api.Controllers;
using GithubUsers.Shared.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GithubUsers.Api.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _client;
        private readonly ILogger<UserService> _logger;
        private readonly AppSettingsModel _settings;

        public UserService(HttpClient client, ILogger<UserService> logger, IOptions<AppSettingsModel> settings) 
        { 
            _client = client;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<ServiceResponse<List<UserInfo>>> RetrieveUsers()
        {
            ServiceResponse<List<UserInfo>> response = new();
            List<UserInfo> users = new List<UserInfo>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_settings.EndPoint);
            var token = "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";
            client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AppName", "1.0"));
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", token);

            try
            {
                // Get List of Username
                var responseUsers = await client.GetAsync("/users");
                if (responseUsers.IsSuccessStatusCode)
                {
                    string jsonUsers = responseUsers.Content.ReadAsStringAsync().Result;
                    var resultUsers = JsonConvert.DeserializeObject<List<Username>>(jsonUsers);

                    // Prevent duplicate usernames
                    resultUsers = resultUsers?.Distinct().ToList();
                    foreach (var item in resultUsers)
                    {
                        // Get User basic information
                        var responseUser = await client.GetAsync("/users/" + item?.Login);
                        if (responseUser.IsSuccessStatusCode)
                        {
                            string jsonUser = responseUser.Content.ReadAsStringAsync().Result;
                            var resultUser = JsonConvert.DeserializeObject<UserInfo>(jsonUser);

                            if (resultUser?.Public_Repos != 0)
                                resultUser.Average_Public_Repos = resultUser.Followers / resultUser.Public_Repos;
                            users.Add(resultUser);
                        }
                        else
                        {
                            _logger.LogWarning($"Username {item?.Login} not found");
                        }
                    }
                    users = users.OrderBy(a => a.Name).ToList();
                    response.Data = users;
                    response.isSuccess = true;
                    response.Message = $"Total Github Users { users.Count }";
                }
                else
                {
                    _logger.LogError("Unauthorized");
                    response.isSuccess = false;
                    response.Message = "Unauthorized";
                    response.Data = new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
