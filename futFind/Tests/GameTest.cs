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
    public class GameTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly AuthTest _authTest;

        public GameTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _authTest = new AuthTest(factory);
        }

        // Test: Get all games
        [Fact]
        public async Task GetGames()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync("/api/Game");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var games = await response.Content.ReadFromJsonAsync<IEnumerable<Games>>();
            games.Should().NotBeNull();
            games.Should().HaveCountGreaterThan(0);
        }

        // Test: Get a game by ID
        [Fact]
        public async Task GetGame()
        {
            int gameId = 3;

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync($"/api/Game/{gameId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var game = await response.Content.ReadFromJsonAsync<Games>();
            game.Should().NotBeNull();
            game?.id.Should().Be(gameId);
        }

        // Test: Get a game by share code
        [Fact]
        public async Task GetGameByShareCode()
        {
            string shareCode = "string";

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync($"/api/Game/Code/{shareCode}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var game = await response.Content.ReadFromJsonAsync<Games>();
            game.Should().NotBeNull();
            game?.share_code.Should().Be(shareCode);
        }

        // Test: Create a game
        [Fact]
        public async Task CreateGame()
        {
            var newGame = new Games
            {
                host_id = 3,
                date = DateTime.UtcNow.AddDays(1),
                address = "TestAddress",
                capacity = 10,
                price = 100,
                is_private = false,
                share_code = "unique_code",
                status = "Scheduled",
            };

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.PostAsJsonAsync("/api/Game", newGame);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdGame = await response.Content.ReadFromJsonAsync<Games>();
            createdGame.Should().NotBeNull();
            createdGame?.host_id.Should().Be(newGame.host_id);
            createdGame?.address.Should().Be(newGame.address);
        }

        // Test: Update a game by ID
        [Fact]
        public async Task UpdateGame()
        {
            int gameId = 1;  // Assumed existing game ID
            var updatedGame = new Games
            {
                host_id = 2,
                date = DateTime.UtcNow.AddDays(2),
                address = "Updated Address",
                capacity = 20,
                price = 150.0m,
                is_private = true,
                share_code = "updated_code",
                status = "active"
            };

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.PutAsJsonAsync($"/api/Game/{gameId}", updatedGame);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var game = await response.Content.ReadFromJsonAsync<Games>();
            game.Should().NotBeNull();
            game?.address.Should().Be(updatedGame.address);
            game?.capacity.Should().Be(updatedGame.capacity);
        }

        // Test: Delete a game by ID
        [Fact]
        public async Task DeleteGame()
        {
            int gameId = 1;  // Assumed existing game ID

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.DeleteAsync($"/api/Games/{gameId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            result.Should().ContainKey("message");
            result?["message"].Should().Be("Game deleted successfully.");
        }
    }
}