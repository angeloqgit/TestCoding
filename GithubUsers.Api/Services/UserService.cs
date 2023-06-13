using GithubUsers.Api.Controllers;
using GithubUsers.Shared.Models;
using Newtonsoft.Json;

namespace GithubUsers.Api.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _client;
        private readonly ILogger<UserService> _logger;
        public UserService(HttpClient client, ILogger<UserService> logger) 
        { 
            _client = client;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<UserInfo>>> RetrieveUsers()
        {
            ServiceResponse<List<UserInfo>> response = new();
            List<UserInfo> users = new List<UserInfo>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.github.com");
            var token = "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";
            client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AppName", "1.0"));
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", token);

            try
            {
                var responseUsers = await client.GetAsync("/users");
                if (responseUsers.IsSuccessStatusCode)
                {
                    string jsonUsers = responseUsers.Content.ReadAsStringAsync().Result;
                    var resultUsers = JsonConvert.DeserializeObject<List<Username>>(jsonUsers);

                    // Prevent duplicate usernames
                    resultUsers = resultUsers?.Distinct().ToList();
                    foreach (var item in resultUsers)
                    {
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
