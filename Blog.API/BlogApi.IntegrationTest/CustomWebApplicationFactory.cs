using Blog.API.Contracts;
using Blog.API.Contracts.V1.Requests;
using Blog.API.Contracts.V1.Responses;
using Blog.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.IntegrationTest
{
    public class CustomWebApplicationFactory<TStartUp> : WebApplicationFactory<TStartUp> where TStartUp: class
    {
        private IServiceProvider _serviceProvider;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //base.ConfigureWebHost(builder);
            builder.ConfigureServices(services => 
            {
                var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<DataContext>));
                services.Remove(descriptor);

                services.AddDbContext<DataContext>(options => 
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                _serviceProvider = services.BuildServiceProvider();

                using (var scope = _serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<DataContext>();

                    db.Database.EnsureCreated();
                }
            });
        }

        public async Task AuthenticateAsync(HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "bearer", parameter: await GetJwtAsync(client));
        }

        private async Task<string> GetJwtAsync(HttpClient client)
        {
            var response = await client.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegistrationRequest
            {
                Email = "testss@integration.com",
                Password = "Password123*"
            });
            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();

            if (registrationResponse.Token == null)
            {
                var loginResponse = await client.PostAsJsonAsync(ApiRoutes.Identity.Login, new UserLoginRequest 
                {
                    Email = "testss@integration.com",
                    Password = "Password123*"
                });
                var loginResponseResult = await response.Content.ReadAsAsync<AuthSuccessResponse>();
                return loginResponseResult.Token;
            }

            return registrationResponse.Token;
        }
    }
}
