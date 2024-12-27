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
    public class MemberTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly AuthTest _authTest;

        public MemberTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _authTest = new AuthTest(factory);
        }

        // Test: Get clan member
        [Fact]
        public async Task GetClanMember()
        {
            int userId = 1;
            int clanId = 1;

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync($"/api/Member/{userId}/{clanId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var member = await response.Content.ReadFromJsonAsync<Members>();
            member.Should().NotBeNull();
            member?.user_id.Should().Be(userId);
            member?.clan_id.Should().Be(clanId);
        }

        // Test: Get all members of a clan
        [Fact]
        public async Task GetMembersOfClan()
        {
            int clanId = 1;

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync($"/api/Member/clan/{clanId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var members = await response.Content.ReadFromJsonAsync<IEnumerable<Users>>();
            members.Should().NotBeNull();
            members.Should().HaveCountGreaterThan(0);
        }

        // Test: Get user clan
        [Fact]
        public async Task GetUserClan()
        {
            int userId = 1;

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync($"/api/Member/user/{userId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var clans = await response.Content.ReadFromJsonAsync<IEnumerable<Teams>>();
            clans.Should().NotBeNull();
            clans.Should().HaveCountGreaterThan(0);
        }

        // Test: Add member to clan
        [Fact]
        public async Task AddMemberToClan()
        {
            var newMember = new
            {
                user_id = 1,
                clan_id = 3,
                user = new
                {
                    id = 0,
                    name = "string",
                    email = "user@example.com",
                    password = "string",
                    phone = "string"
                },
                team = new
                {
                    id = 0,
                    leader = 0,
                    name = "string",
                    description = "string",
                    capacity = 0,
                    invite_code = "string"
                }
            };

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.PostAsJsonAsync("/api/Member", newMember);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var addedMember = await response.Content.ReadFromJsonAsync<Members>();
            addedMember.Should().NotBeNull();
            addedMember?.user_id.Should().Be(newMember.user_id);
            addedMember?.clan_id.Should().Be(newMember.clan_id);
        }

        // Test: Remove member from clan
        [Fact]
        public async Task RemoveUserFromClan()
        {
            int userId = 1;
            int clanId = 3;

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.DeleteAsync($"/api/Member/{clanId}/{userId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var message = await response.Content.ReadAsStringAsync();
            message.Should().Contain("User successfully removed from the clan.");
        }
    }
}