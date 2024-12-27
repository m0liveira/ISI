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
    public class PlayerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly AuthTest _authTest;

        public PlayerTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _authTest = new AuthTest(factory);
        }

        // Test: Get all players
        [Fact]
        public async Task GetPlayers()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync("/api/Player");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var players = await response.Content.ReadFromJsonAsync<IEnumerable<Players>>();
            players.Should().NotBeNull();
        }

        // Test: Get a specific player of a game
        [Fact]
        public async Task GetPlayer()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync("/api/Player/1/3");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var player = await response.Content.ReadFromJsonAsync<Players>();
            player.Should().NotBeNull();
        }

        // Test: Get all players of a game
        [Fact]
        public async Task GetGamePlayers()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync("/api/Player/Game/3");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var players = await response.Content.ReadFromJsonAsync<IEnumerable<Users>>();
            players.Should().NotBeNull();
        }

        // Test: Get all games of a specific player
        [Fact]
        public async Task GetPlayerGames()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync("/api/Player/1/Games");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var games = await response.Content.ReadFromJsonAsync<IEnumerable<Games>>();
            games.Should().NotBeNull();
        }

        // Test: Add player to a game
        [Fact]
        public async Task AddPlayer()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var newPlayer = new
            {
                user_id = 3,
                match_id = 3,
                user = new
                {
                    id = 0,
                    name = "string",
                    email = "user@example.com",
                    password = "string",
                    phone = "string"
                },
                game = new
                {
                    id = 0,
                    host_id = 0,
                    date = "2023-08-21T12:00:00",
                    address = "string",
                    capacity = 0,
                    price = 150.0m,
                    is_private = true,
                    share_code = "stringdasd",
                    status = "Scheduled"
                }
            };

            var response = await _client.PostAsJsonAsync("/api/Player", newPlayer);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdPlayer = await response.Content.ReadFromJsonAsync<Players>();
            createdPlayer.Should().NotBeNull();
            createdPlayer?.user_id.Should().Be(newPlayer.user_id);
        }

        // Test: Remove player from a game
        [Fact]
        public async Task RemovePlayer()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.DeleteAsync("/api/Player/1/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<object>();
            result.Should().NotBeNull();
        }
    }
}