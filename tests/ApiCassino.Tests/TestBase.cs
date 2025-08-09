using ApiCassino.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCassino.Tests
{
    public abstract class TestBase : IDisposable
    {
        protected readonly ApplicationDbContext Context;

        protected TestBase()
        {
            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

            var serviceProvider = services.BuildServiceProvider();
            Context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}