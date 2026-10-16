using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Race_Day.Controllers;
using Race_Day.Data;
using Race_Day.Models;
using Race_Day.Models.DTOs;
using Race_Day.Test.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Race_Day.Test
{
    public class AuthTests
    {
        [Fact]
        public async Task Register_NewUser_ShouldReturnOk()
        {
            
            var (controller, db) = TestFactory.CreateAuthController();
            var dto = new RegisterDTO
            {
                Email = "a@b.com",
                Password = "P@ssw0rd!",
                Role = "User"
            };
           
            var result = await controller.Register(dto);
            // Assert — HTTP response
            Assert.IsType<OkObjectResult>(result);


           
            var created = await db.accounts
                .FirstOrDefaultAsync(u => u.Email == "a@b.com");

            Assert.NotNull(created);
            Assert.Equal("User", created!.Role);
            Assert.NotNull(created.PasswordHash);
            Assert.NotEqual("P@ssw0rd!", created.PasswordHash);
        }
        [Fact]
        public async Task Register_ExistingEmail_ShouldReturnBadRequest()
        {
            var (controller, _) = TestFactory.CreateAuthController();
            var dto = new RegisterDTO
            {
                Email = "dupe@b.com",
                Password = "P@ssw0rd!",
                Role = "User"
            };

            await controller.Register(dto);

            // Act
            var result = await controller.Register(dto);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Email already exists", bad.Value);
        }
        [Fact]
        public void GetAccounts_ShouldReturnAccounts()
        {
            //First A
            using var context = TestDbFactory.Create();
            var (controller, db) = TestFactory.CreateAuthController();

            //Second A
            var result = controller.Register;
            //Third A
            Assert.NotNull(result);


        }
    }
    
}
