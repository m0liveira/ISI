using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using futFind.Models;
using futFind.Swagger.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests
{
    public class AuthTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public AuthTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        public async Task<string?> GetToken(string email, string password)
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var authResponse = await _client.PostAsJsonAsync("/api/Auth", loginRequest);

            if (authResponse.IsSuccessStatusCode)
            {
                var authResponseBody = await authResponse.Content.ReadFromJsonAsync<AuthResponse>();

                return authResponseBody?.Token;
            }

            return null;
        }

        [Fact]
        public async Task Authenticate()
        {
            var client = _factory.CreateClient();
            var validLoginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "test"
            };

            var response = await client.PostAsJsonAsync("/api/Auth", validLoginRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}