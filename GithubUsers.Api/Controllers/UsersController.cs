using GithubUsers.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GithubUsers.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private HttpClient _client;
        private readonly ILogger<UsersController> _logger;
        public UsersController(HttpClient client, ILogger<UsersController> logger)
        { 
            _client = client;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetUsersAsync()
        {
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
                            _logger.LogInformation($"Username {item?.Login} not found");
                        }
                    }
                    users = users.OrderBy(a => a.Name).ToList();
                }
                else
                {
                    _logger.LogError("Unauthorized");
                    return StatusCode(401, "Unauthorized");
                }
                
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
