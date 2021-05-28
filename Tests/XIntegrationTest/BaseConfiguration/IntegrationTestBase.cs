using ChatyChaty;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Linq;
using ChatyChaty.Infrastructure.Database;

namespace XIntegrationTest
{
    public abstract class IntegrationTestBase
    {
        protected readonly HttpClient client;
        public IntegrationTestBase()
        {
            var Factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    RemoveRealDBContext(services);

                    var sqliteMemory = new SqliteInMemory();
                    services.AddDbContextFactory<ChatyChatyContext>(optionsBuilder =>
                    {
                        optionsBuilder.UseSqlite(sqliteMemory.Connection);
                    });
                });
            });
            //create a special client that work the In-Memory app
            client = Factory.CreateClient();
        }

        public static async Task<T> CustomReadResponse<T>(HttpResponseMessage response)
        {
            T result;
            try
            {
                result = await response.Content.ReadAsAsync<T>();
            }
            catch (UnsupportedMediaTypeException)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                throw new Exception($"Unexpected type response, The response as string : {responseAsString}");
            }
            return result;
        }

        private static void RemoveRealDBContext(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ChatyChatyContext>));
            services.Remove(descriptor);
        }
    }
}
