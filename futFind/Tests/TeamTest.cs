using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using futFind.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests
{
    public class TeamTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly AuthTest _authTest;

        public TeamTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _authTest = new AuthTest(factory);
        }

        // Test: Get teams
        [Fact]
        public async Task GetTeams()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync("/api/Team");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var teams = await response.Content.ReadFromJsonAsync<IEnumerable<Teams>>();
            teams.Should().NotBeNull();
        }

        // Test: Get team by share code
        [Fact]
        public async Task GetTeamByCode()
        {
            string inviteCode = "string";

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync($"/api/Team/code/{inviteCode}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var team = await response.Content.ReadFromJsonAsync<Teams>();
            team.Should().NotBeNull();
            team?.invite_code.Should().Be(inviteCode);
        }

        // Test: Get team by ID
        [Fact]
        public async Task GetTeamById()
        {
            int teamId = 1;

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync($"/api/Team/{teamId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var team = await response.Content.ReadFromJsonAsync<Teams>();
            team.Should().NotBeNull();
            team?.id.Should().Be(teamId);
        }

        // Test: Create team
        [Fact]
        public async Task CreateTeam()
        {
            var newTeam = new Teams
            {
                name = "New Team",
                description = "A test team description",
                capacity = 10,
                invite_code = "newInviteCode123",
                leader = 2
            };

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.PostAsJsonAsync("/api/Team", newTeam);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdTeam = await response.Content.ReadFromJsonAsync<Teams>();
            createdTeam.Should().NotBeNull();
            createdTeam?.name.Should().Be(newTeam.name);
            createdTeam?.invite_code.Should().Be(newTeam.invite_code);
        }

        // Test: Update team
        [Fact]
        public async Task UpdateTeam()
        {
            int teamId = 3;

            var updatedTeam = new Teams
            {
                name = "Updated Team Name",
                description = "Updated team description",
                capacity = 20,
                invite_code = "updatedInviteCode123",
                leader = 2
            };

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.PutAsJsonAsync($"/api/Team/{teamId}", updatedTeam);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var team = await response.Content.ReadFromJsonAsync<Teams>();
            team.Should().NotBeNull();
            team?.name.Should().Be(updatedTeam.name);
            team?.invite_code.Should().Be(updatedTeam.invite_code);
        }
    }
}