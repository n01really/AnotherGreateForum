using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ForumTest
{
    // todo: Adjust the Forum class according to your actual Forum model
    public class ForumTests
    {
        [Fact]
        public async Task Create_Forum_Test()
        {
            // Arrange
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5044/") // Ändra till API-port
            };

            var createForum = new Forum
            { 
                Id = 1,
                Title = "Test Forum",
                Description = "This is a test forum",
                CreatedAt = DateTime.Now,
                CreatedBy = "TestUser",
            };

            // Act
            var response = await httpClient.PostAsJsonAsync("/api/forums", createForum);
            
            // Assert
            Assert.True(response.IsSuccessStatusCode, $"API call failed with status code: {response.StatusCode}");
            
            var result = await response.Content.ReadFromJsonAsync<Forum>();
            Assert.NotNull(result);
            Assert.Equal(createForum.Title, result.Title);
            Assert.Equal(createForum.Description, result.Description);
            Assert.Equal(createForum.CreatedBy, result.CreatedBy);
        }

        [Fact]
        public async Task Unique_Forum_Test() 
        { 
            // Arrange
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5044/") // Ändra till API-port
            };

            var createForum1 = new Forum
            {
                Title = "Test Forum 1",
                Description = "This is a test forum",
                CreatedAt = DateTime.Now,
                CreatedBy = "TestUser",
            };
            var createForum2 = new Forum
            {
                Title = "Test Forum 2",
                Description = "This is a test forum",
                CreatedAt = DateTime.Now,
                CreatedBy = "TestUser",
            };

            // Act
            var response1 = await httpClient.PostAsJsonAsync("/api/forums", createForum1);
            var response2 = await httpClient.PostAsJsonAsync("/api/forums", createForum2);

            // Assert
            Assert.True(response1.IsSuccessStatusCode, $"First API call failed with status code: {response1.StatusCode}");
            Assert.True(response2.IsSuccessStatusCode, $"Second API call failed with status code: {response2.StatusCode}");

            var result1 = await response1.Content.ReadFromJsonAsync<Forum>();
            var result2 = await response2.Content.ReadFromJsonAsync<Forum>();

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotEqual(result1.Id, result2.Id);
            Assert.Equal("Test Forum 1", result1.Title);
            Assert.Equal("Test Forum 2", result2.Title);
        }

        [Fact]
        public async Task Get_Forums_Test()
        {
            // Arrange
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5044/") // Ändra till API-port
            };
            // Act
            var response = await httpClient.GetAsync("/api/forums");
            // Assert
            Assert.True(response.IsSuccessStatusCode, $"API call failed with status code: {response.StatusCode}");
            var forums = await response.Content.ReadFromJsonAsync<List<Forum>>();
            Assert.NotNull(forums);
            Assert.True(forums.Count > 0, "No forums were returned from the API.");
        }
    }
}
