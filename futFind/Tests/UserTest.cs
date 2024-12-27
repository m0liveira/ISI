using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using futFind.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests
{
    public class UserTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly AuthTest _authTest;

        public UserTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _authTest = new AuthTest(factory);
        }

        // Test: Get users
        [Fact]
        public async Task GetUsers()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync("/api/Users");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var users = await response.Content.ReadFromJsonAsync<IEnumerable<Users>>();
            users.Should().NotBeNull();
        }

        // Test: Get users by id
        [Fact]
        public async Task GetUser()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var userId = 1;

            var response = await _client.GetAsync($"/api/Users/{userId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var user = await response.Content.ReadFromJsonAsync<Users>();
            user.Should().NotBeNull();
        }

        // Test: Create users
        [Fact]
        public async Task CreateUser()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var newUser = new Users
            {
                name = "John Doe",
                email = "johndoe@example.com",
                password = "Password123",
                phone = "1234567890"
            };

            var response = await _client.PostAsJsonAsync("/api/Users", newUser);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdUser = await response.Content.ReadFromJsonAsync<Users>();
            createdUser.Should().NotBeNull();
            createdUser?.email.Should().Be(newUser.email);
        }

        // Test: Update users
        [Fact]
        public async Task UpdateUser()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var updatedUser = new Users
            {
                name = "Updated Name",
                email = "updatedemail@example.com",
                password = "UpdatedPassword123",
                phone = "0987654321"
            };

            var response = await _client.PutAsJsonAsync($"/api/Users/{1}", updatedUser);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var user = await response.Content.ReadFromJsonAsync<Users>();
            user.Should().NotBeNull();
            user?.name.Should().Be(updatedUser.name);
        }

        // Test: Delete users
        [Fact]
        public async Task DeleteUser()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.DeleteAsync($"/api/Users/{1}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var message = await response.Content.ReadAsStringAsync();
            message.Should().Contain("User deleted successfully.");
        }
    }
}