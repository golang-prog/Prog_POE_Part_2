using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Race_Day.Controllers;
using Race_Day.Data;
using Race_Day.Models;

namespace Race_Day.Test.TestHelpers
{
    public static class TestFactory
    {
        public static (AuthController controller, ApplicationDbcontext db) CreateAuthController()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbcontext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var db = new ApplicationDbcontext(options);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "THIS_IS_A_TEST_KEY_THAT_IS_LONG_ENOUGH_32CHARS!",
                    ["Jwt:Issuer"] = "TestIssuer",
                    ["Jwt:Audience"] = "TestAudience"
                })
                .Build();

            var hasher = new PasswordHasher<UserAccount>();
            var controller = new AuthController(db, config, hasher);

            return (controller, db);
        }
    }
}