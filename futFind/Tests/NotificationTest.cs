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
    public class NotificationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly AuthTest _authTest;

        public NotificationTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _authTest = new AuthTest(factory);
        }

        // Test: Get notifications
        [Fact]
        public async Task GetNotifications()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync("/api/Notification");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var notifications = await response.Content.ReadFromJsonAsync<IEnumerable<Notifications>>();
            notifications.Should().NotBeNull();
        }

        // Test: Get specific notification
        [Fact]
        public async Task GetNotification()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync("/api/Notification/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var notification = await response.Content.ReadFromJsonAsync<Notifications>();
            notification.Should().NotBeNull();
        }

        // Test: Get notifications by game
        [Fact]
        public async Task GetNotificationsByGame()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.GetAsync("/api/Notification/Game/3");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var notifications = await response.Content.ReadFromJsonAsync<IEnumerable<Notifications>>();
            notifications.Should().NotBeNull();
        }

        // Test: Create notification
        [Fact]
        public async Task CreateNotification()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var newNotification = new Notifications
            {
                match_id = 9,
                message = "Test message",
                seen = false,
                timestamp = DateTime.UtcNow
            };

            var response = await _client.PostAsJsonAsync("/api/Notification", newNotification);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdNotification = await response.Content.ReadFromJsonAsync<Notifications>();
            createdNotification.Should().NotBeNull();
            createdNotification?.message.Should().Be(newNotification.message);
        }

        // Test: Update notification
        [Fact]
        public async Task UpdateNotification()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var updatedNotification = new Notifications
            {
                match_id = 9,
                message = "Updated message",
                seen = true,
                timestamp = DateTime.UtcNow
            };

            var response = await _client.PutAsJsonAsync("/api/Notification/3", updatedNotification);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var notification = await response.Content.ReadFromJsonAsync<Notifications>();
            notification.Should().NotBeNull();
            notification?.message.Should().Be(updatedNotification.message);
        }

        // Test: Delete notification
        [Fact]
        public async Task DeleteNotification()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _authTest.GetToken("test@example.com", "test")}");

            var response = await _client.DeleteAsync("/api/Notifications/3");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<object>();
            result.Should().NotBeNull();
        }
    }
}