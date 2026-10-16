using Microsoft.EntityFrameworkCore;
using Race_Day.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Race_Day.Test.TestHelpers
{
    internal static class TestDbFactory
    {
        public static ApplicationDbcontext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbcontext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbcontext(options);
        }
    }
}
