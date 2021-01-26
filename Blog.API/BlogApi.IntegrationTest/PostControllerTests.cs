using Blog.API;
using Blog.API.Contracts;
using Blog.API.Contracts.V1.Requests;
using Blog.API.Contracts.V1.Responses;
using Blog.API.Domain.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Priority;

namespace BlogApi.IntegrationTest
{
    [TestCaseOrderer("FullNameOfOrderStrategyHere", "OrderStrategyAssemblyName")]
    public class PostControllerTests: IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Startup> _factory;
#pragma warning disable 169
        private static bool[] _counter;
#pragma warning restore 169

        public PostControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact, Priority(1)]
        public async Task GetAll_WithoutAnyPosts_ReturnEmptyResponse()
        {
            //Arrange
            await _factory.AuthenticateAsync(_client);

            //Act
            var response = await _client.GetAsync(ApiRoutes.Posts.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<PostDTO>> ()).Should().BeEmpty();
        }

        [Fact, Priority(2)]
        public async Task Create_NewPostSuccess_ReturnCreatedResponse()
        {
            // Arrange
            await _factory.AuthenticateAsync(_client);

            // Act
            var response = await _client.PostAsync(ApiRoutes.Posts.Create, null);
            var createResponse = await response.Content.ReadAsAsync<CreatePostResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            //(await response..ReadAsAsync<CreatePostResponse>()).Should().NotBeNull();
        }
    }
}
