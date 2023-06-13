using GithubUsers.Api.Services;
using GithubUsers.Shared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace GithubUsers.Test
{
    public class UserServiceFake : IUserService
    {
        public async Task<ServiceResponse<List<UserInfo>>> RetrieveUsers()
        {
            ServiceResponse<List<UserInfo>> response = new();
            List<UserInfo> users = new List<UserInfo>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.github.com");
            client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AppName", "1.0"));
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", "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");

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
                            //_logger.LogWarning($"Username {item?.Login} not found");
                        }
                    }
                    users = users.OrderBy(a => a.Name).ToList();
                    response.Data = users;
                    response.isSuccess = true;
                    response.Message = $"Total Github Users {users.Count}";
                }
                else
                {
                    //_logger.LogError("Unauthorized");
                    response.isSuccess = false;
                    response.Message = "Unauthorized";
                    response.Data = new();
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.Message);
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
