using GithubUsers.Api.Controllers;
using GithubUsers.Api.Services;
using GithubUsers.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubUsers.Test
{
    public class UsersControllerTest
    {
        private readonly UsersController _controller;
        private readonly IUserService _service;

        public UsersControllerTest()
        {
            _service = new UserServiceFake();
            _controller = new UsersController(_service);
        }

        [Fact]
        public void Get_WhenCalled_RetreiveUsers()
        {
            // Act
            var okResult = _controller.RetrieveUsers().Result as OkObjectResult;

            // Assert
            var items = Assert.IsType<ServiceResponse<List<UserInfo>>>(okResult.Value);
            Assert.Equal(30, items.Data.Count) ;
        }
    }
}
