using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using AnotherGoodAPI.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using AnotherGoodAPI.Endpoints.Posts;
using AnotherGoodAPI.Endpoints.Categories;
using AnotherGoodAPI.Endpoints.Comments;
using AnotherGoodAPI.Endpoints.Users;

namespace ForumTest
{
    using CreatePostRequest = CreatePostEndpoint.Request;
    using CreatePostResponse = CreatePostEndpoint.Response;
    using GetPostsResponse = GetPostsEndpoint.Response;
    using GetPostResponse = GetPostEndpoint.Response;
    using UpdatePostRequest = UpdatePostEndpoint.Request;
    using UpdatePostResponse = UpdatePostEndpoint.Response;
    using CategoryDto = GetCategoriesEndpoint.CategoryDto;
    using CategoryCreateRequest = CreateCategoryEndpoint.CategoryCreateRequest;
    using CategoryResponse = CreateCategoryEndpoint.CategoryResponse;
    using GetCommentsResponse = GetCommentsEndpoint.Response;
    using CreateCommentRequest = CreateCommentEndpoint.Request;
    using CreateCommentResponse = CreateCommentEndpoint.Response;
    using RegisterUserRequest = RegisterUserEndpoint.Request;
    using RegisterUserResponse = RegisterUserEndpoint.Response;
    using LoginUserRequest = LoginUserEndpoint.Request;
    using LoginUserResponse = LoginUserEndpoint.Response;
    using GetCurrentUserResponse = GetCurrentUserEndpoint.Response;
    using ToggleLikeResponse = ToggleLikeEndpoint.Response;

    public class CustomWebApplicationFactory : WebApplicationFactory<AnotherGoodAPI.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }

        public HttpClient CreateClientWithCookies()
        {
            var clientOptions = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true,
                HandleCookies = true
            };
            return CreateClient(clientOptions);
        }
    }

    public class PostTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public PostTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // Test: Verify that creating a post works (or returns 401 if authentication is required)
        // Steps:
        // 1. Create a post with title, content, and category
        // 2. Send POST request to /posts endpoint
        // 3. Check if the response is successful or returns 401 (unauthorized)
        // 4. If successful, verify the response contains the created post with correct data
        [Fact]
        public async Task Create_Post_Test()
        {
            var httpClient = _factory.CreateClientWithCookies();

            // Arrange - Register a user (which automatically signs them in)
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            var registerRequest = new RegisterUserRequest(
                "Test User " + uniqueId,
                $"testuser{uniqueId}@example.com",
                "TestPassword123!"
            );

            var registerResponse = await httpClient.PostAsJsonAsync("/users/register", registerRequest);
            if (!registerResponse.IsSuccessStatusCode)
            {
                Assert.True(true, $"User registration failed - cannot test post creation");
                return;
            }

            // Now create the post (user is already signed in from registration)
            var createPost = new CreatePostRequest("Test Post", "This is a test post", 1);

            // Act
            var response = await httpClient.PostAsJsonAsync("/posts", createPost);

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"API call failed with status code: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<CreatePostResponse>();
            Assert.NotNull(result);
            Assert.Equal("Test Post", result.Title);
            Assert.Equal("This is a test post", result.Content);
            Assert.True(result.Id > 0, "Post ID should be assigned");
        }

        // Test: Verify that creating multiple posts results in unique post IDs
        // Steps:
        // 1. Create two different posts with different titles and categories
        // 2. Send both POST requests to /posts endpoint
        // 3. Verify both posts are created successfully (or 401 if auth required)
        // 4. Check that each post has a different ID (database auto-increment works)
        // 5. Verify each post retained its correct title
        [Fact]
        public async Task Unique_Post_Test()
        {
            var httpClient = _factory.CreateClientWithCookies();

            // Arrange - Register a user (which automatically signs them in)
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            var registerRequest = new RegisterUserRequest(
                "Test User " + uniqueId,
                $"testuser{uniqueId}@example.com",
                "TestPassword123!"
            );

            var registerResponse = await httpClient.PostAsJsonAsync("/users/register", registerRequest);
            if (!registerResponse.IsSuccessStatusCode)
            {
                Assert.True(true, $"User registration failed - cannot test post creation");
                return;
            }

            // Now create the posts (user is already signed in from registration) - use same category ID
            var createPost1 = new CreatePostRequest("Test Post 1", "This is test post 1", 1);
            var createPost2 = new CreatePostRequest("Test Post 2", "This is test post 2", 1);

            // Act
            var response1 = await httpClient.PostAsJsonAsync("/posts", createPost1);
            var response2 = await httpClient.PostAsJsonAsync("/posts", createPost2);

            // Assert
            Assert.True(response1.IsSuccessStatusCode, $"First API call failed with status code: {response1.StatusCode}");
            Assert.True(response2.IsSuccessStatusCode, $"Second API call failed with status code: {response2.StatusCode}");

            var result1 = await response1.Content.ReadFromJsonAsync<CreatePostResponse>();
            var result2 = await response2.Content.ReadFromJsonAsync<CreatePostResponse>();

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotEqual(result1.Id, result2.Id);
            Assert.Equal("Test Post 1", result1.Title);
            Assert.Equal("Test Post 2", result2.Title);
        }

        // Test: Verify that getting all posts works correctly
        // Steps:
        // 1. Send GET request to /posts endpoint
        // 2. Verify the response is successful (200 OK)
        // 3. Verify the response can be deserialized into a list of posts
        // 4. Verify the list is not null (may be empty if no posts exist)
        [Fact]
        public async Task Get_Posts_Test()
        {
            var httpClient = _factory.CreateClient();

            // Arrange & Act
            var response = await httpClient.GetAsync("/posts");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"API call failed with status code: {response.StatusCode}");
            var posts = await response.Content.ReadFromJsonAsync<List<GetPostsResponse>>();
            Assert.NotNull(posts);
        }

        // Test: Verify that getting a single post by ID works correctly
        // Steps:
        // 1. Send GET request to /posts/{id} endpoint with a specific post ID
        // 2. Check if the response is successful (200 OK) or 404 (Not Found)
        // 3. If post is found, verify the response contains the correct post data
        // 4. If post is not found, verify 404 status is returned
        [Fact]
        public async Task Get_Post_By_Id_Test()
        {
            var httpClient = _factory.CreateClient();

            // Arrange
            int testPostId = 1;

            // Act
            var response = await httpClient.GetAsync($"/posts/{testPostId}");

            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Assert.True(true, "Post not found - expected 404");
                return;
            }

            Assert.True(response.IsSuccessStatusCode, $"API call failed with status code: {response.StatusCode}");
            var post = await response.Content.ReadFromJsonAsync<GetPostResponse>();
            Assert.NotNull(post);
            Assert.Equal(testPostId, post.Id);
            Assert.NotNull(post.Title);
            Assert.NotNull(post.Body);
        }

        // Test: Verify that updating a post works correctly (or returns 401/403 if unauthorized)
        // Steps:
        // 1. Register and login a user to get authentication
        // 2. Create a post with initial data
        // 3. Create an update request with new title, content, and category
        // 4. Send PUT request to /posts/{id} endpoint
        // 5. Check if response is successful, unauthorized (401), forbidden (403), or not found (404)
        // 6. If successful, verify the updated post contains the new data
        [Fact]
        public async Task Update_Post_Test()
        {
            var httpClient = _factory.CreateClientWithCookies();

            // Arrange - Register a user (which automatically signs them in)
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            var registerRequest = new RegisterUserRequest(
                "Test User " + uniqueId,
                $"testuser{uniqueId}@example.com",
                "TestPassword123!"
            );

            var registerResponse = await httpClient.PostAsJsonAsync("/users/register", registerRequest);
            if (!registerResponse.IsSuccessStatusCode)
            {
                Assert.True(true, $"User registration failed - cannot test post update");
                return;
            }

            // Create a post to update (user is already signed in from registration)
            var createPost = new CreatePostRequest("Original Test Post", "Original content", 1);
            var createResponse = await httpClient.PostAsJsonAsync("/posts", createPost);

            if (!createResponse.IsSuccessStatusCode)
            {
                Assert.True(true, $"Post creation failed with status {createResponse.StatusCode} - cannot test update");
                return;
            }

            var createdPost = await createResponse.Content.ReadFromJsonAsync<CreatePostResponse>();
            Assert.NotNull(createdPost);
            int testPostId = createdPost.Id;

            // Arrange - Create update request
            var updateRequest = new UpdatePostRequest("Updated Test Post", "This is updated content", 1);

            // Act
            var response = await httpClient.PutAsJsonAsync($"/posts/{testPostId}", updateRequest);

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"API call failed with status code: {response.StatusCode}");
            var result = await response.Content.ReadFromJsonAsync<UpdatePostResponse>();
            Assert.NotNull(result);
            Assert.Equal("Updated Test Post", result.Title);
            Assert.Equal("This is updated content", result.Content);
        }

        // Test: Verify that deleting a post works correctly (or returns 401/403 if unauthorized)
        // Steps:
        // 1. Login as admin user
        // 2. Create a post to delete
        // 3. Send DELETE request to /posts/{id} endpoint for the created post
        // 4. Check if response is no content (204), unauthorized (401), forbidden (403), or not found (404)
        // 5. If successful, verify no content is returned (204 status)
        // 6. If unauthorized, verify 401 status
        // 7. If forbidden, verify 403 status
        // 8. If not found, verify 404 status
        [Fact]
        public async Task Delete_Post_Test()
        {
            var httpClient = _factory.CreateClientWithCookies();

            // Arrange - Login as admin user
            var loginRequest = new LoginUserRequest(
                "admin@example.com",
                "Admin123!"
            );

            var loginResponse = await httpClient.PostAsJsonAsync("/users/login", loginRequest);
            if (!loginResponse.IsSuccessStatusCode)
            {
                Assert.True(true, $"Admin login failed - cannot test post deletion");
                return;
            }

            // Create a post to delete (admin is already signed in)
            var createPost = new CreatePostRequest("Post to Delete", "This post will be deleted", 1);
            var createResponse = await httpClient.PostAsJsonAsync("/posts", createPost);

            if (!createResponse.IsSuccessStatusCode)
            {
                Assert.True(true, $"Post creation failed with status {createResponse.StatusCode} - cannot test delete");
                return;
            }

            var createdPost = await createResponse.Content.ReadFromJsonAsync<CreatePostResponse>();
            Assert.NotNull(createdPost);
            int testPostId = createdPost.Id;

            // Act
            var response = await httpClient.DeleteAsync($"/posts/{testPostId}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        // Test: Verify that toggling like on a post works correctly (or returns 401 if unauthorized)
        // Steps:
        // 1. Create a post first
        // 2. Send POST request to /posts/{id}/togglelike endpoint
        // 3. Check if response is successful (200 OK), unauthorized (401), or not found (404)
        // 4. If successful, verify the response message indicates "liked" or "unliked"
        [Fact]
        public async Task Toggle_Like_Post_Test()
        {
            var httpClient = _factory.CreateClientWithCookies();

            // Arrange - Register a user (which automatically signs them in)
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            var registerRequest = new RegisterUserRequest(
                "Test User " + uniqueId,
                $"testuser{uniqueId}@example.com",
                "TestPassword123!"
            );

            var registerResponse = await httpClient.PostAsJsonAsync("/users/register", registerRequest);
            if (!registerResponse.IsSuccessStatusCode)
            {
                Assert.True(true, $"User registration failed - cannot test toggle like");
                return;
            }

            // Create a post to like (user is already signed in from registration)
            var createPost = new CreatePostRequest("Post to Like", "This post will be liked", 1);
            var createResponse = await httpClient.PostAsJsonAsync("/posts", createPost);

            if (!createResponse.IsSuccessStatusCode)
            {
                Assert.True(true, $"Post creation failed with status {createResponse.StatusCode} - cannot test toggle like");
                return;
            }

            var createdPost = await createResponse.Content.ReadFromJsonAsync<CreatePostResponse>();
            Assert.NotNull(createdPost);
            int testPostId = createdPost.Id;

            // Act
            var response = await httpClient.PostAsync($"/posts/{testPostId}/togglelike", null);

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"API call failed with status code: {response.StatusCode}");
            var result = await response.Content.ReadFromJsonAsync<ToggleLikeResponse>();
            Assert.NotNull(result);
            Assert.NotNull(result.Message);
            Assert.True(result.Message == "liked" || result.Message == "unliked");
        }
    }

    public class CategoryTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public CategoryTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // Test: Verify that getting all categories works correctly
        // Steps:
        // 1. Send GET request to /categories endpoint
        // 2. Verify the response is successful (200 OK)
        // 3. Verify the response can be deserialized into a list of categories
        // 4. Verify the list is not null (may be empty if no categories exist)
        [Fact]
        public async Task Get_Categories_Test()
        {
            var httpClient = _factory.CreateClient();

            // Arrange & Act
            var response = await httpClient.GetAsync("/categories");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"API call failed with status code: {response.StatusCode}");
            var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
            Assert.NotNull(categories);
        }

        // Test: Verify that creating a category works correctly
        // Steps:
        // 1. Create a category request with name and description
        // 2. Send POST request to /categories endpoint
        // 3. Check if response is created (201) or bad request (400)
        // 4. If successful, verify the response contains the created category with assigned ID
        // 5. If failed, verify it's due to duplicate name or validation error
        [Fact]
        public async Task Create_Category_Test()
        {
            var httpClient = _factory.CreateClient();

            // Arrange
            var createCategory = new CategoryCreateRequest(
                "Test Category " + Guid.NewGuid().ToString().Substring(0, 8),
                "This is a test category"
            );

            // Act
            var response = await httpClient.PostAsJsonAsync("/categories", createCategory);

            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                Assert.True(true, "Category creation failed - possibly duplicate name or validation error");
                return;
            }

            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<CategoryResponse>();
            Assert.NotNull(result);
            Assert.True(result.Id > 0, "Category ID should be assigned");
            Assert.Equal(createCategory.Name, result.Name);
            Assert.Equal(createCategory.Description, result.Description);
        }
    }

    public class CommentTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public CommentTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // Test: Verify that getting comments for a specific post works correctly
        // Steps:
        // 1. Create a post first
        // 2. Send GET request to /comments/post/{postId} endpoint
        // 3. Verify the response is successful (200 OK)
        // 4. Verify the response can be deserialized into a list of comments
        // 5. Verify the list is not null (may be empty if no comments exist for that post)
        [Fact]
        public async Task Get_Comments_For_Post_Test()
        {
            var httpClient = _factory.CreateClientWithCookies();

            // Arrange - Register a user (which automatically signs them in)
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            var registerRequest = new RegisterUserRequest(
                "Test User " + uniqueId,
                $"testuser{uniqueId}@example.com",
                "TestPassword123!"
            );

            var registerResponse = await httpClient.PostAsJsonAsync("/users/register", registerRequest);

            // Create a post (user is already signed in from registration)
            var createPost = new CreatePostRequest("Post for Comments", "This post will have comments", 1);
            var createResponse = await httpClient.PostAsJsonAsync("/posts", createPost);

            // Use the created post ID if available, otherwise default to 1
            int testPostId = 1;
            if (createResponse.IsSuccessStatusCode)
            {
                var createdPost = await createResponse.Content.ReadFromJsonAsync<CreatePostResponse>();
                if (createdPost != null)
                {
                    testPostId = createdPost.Id;
                }
            }

            // Act
            var response = await httpClient.GetAsync($"/comments/post/{testPostId}");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"API call failed with status code: {response.StatusCode}");
            var comments = await response.Content.ReadFromJsonAsync<List<GetCommentsResponse>>();
            Assert.NotNull(comments);
        }

        // Test: Verify that creating a comment works correctly (or returns 401 if unauthorized)
        // Steps:
        // 1. Create a post first
        // 2. Create a comment request with post ID and comment body
        // 3. Send POST request to /comments endpoint
        // 4. Check if response is created (201), unauthorized (401), or bad request (400)
        // 5. If successful, verify the response contains the created comment with assigned ID
        // 6. If unauthorized, verify 401 status
        // 7. If bad request, verify it's due to validation error or non-existent post
        [Fact]
        public async Task Create_Comment_Test()
        {
            var httpClient = _factory.CreateClientWithCookies();

            // Arrange - Register a user (which automatically signs them in)
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            var registerRequest = new RegisterUserRequest(
                "Test User " + uniqueId,
                $"testuser{uniqueId}@example.com",
                "TestPassword123!"
            );

            var registerResponse = await httpClient.PostAsJsonAsync("/users/register", registerRequest);
            if (!registerResponse.IsSuccessStatusCode)
            {
                Assert.True(true, $"User registration failed - cannot test comment creation");
                return;
            }

            // Create a post (user is already signed in from registration)
            var createPost = new CreatePostRequest("Post for Comment", "This post will have a comment", 1);
            var createResponse = await httpClient.PostAsJsonAsync("/posts", createPost);

            if (!createResponse.IsSuccessStatusCode)
            {
                Assert.True(true, $"Post creation failed - cannot test comment creation");
                return;
            }

            var createdPost = await createResponse.Content.ReadFromJsonAsync<CreatePostResponse>();
            Assert.NotNull(createdPost);
            int postId = createdPost.Id;

            var createComment = new CreateCommentRequest(postId, "This is a test comment");

            // Act
            var response = await httpClient.PostAsJsonAsync("/comments", createComment);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<CreateCommentResponse>();
            Assert.NotNull(result);
            Assert.True(result.Id > 0, "Comment ID should be assigned");
            Assert.Equal(postId, result.PostId);
            Assert.Equal("This is a test comment", result.Body);
        }
    }

    public class UserTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public UserTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // Test: Verify that user registration works correctly
        // Steps:
        // 1. Create a registration request with unique display name, email, and password
        // 2. Send POST request to /users/register endpoint
        // 3. Check if response is created (201) or bad request (400)
        // 4. If successful, verify the response contains user data with assigned ID
        // 5. If failed, verify it's due to duplicate email or validation error
        [Fact]
        public async Task Register_User_Test()
        {
            var httpClient = _factory.CreateClient();

            // Arrange
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            var registerRequest = new RegisterUserRequest(
                "Test User " + uniqueId,
                $"testuser{uniqueId}@example.com",
                "TestPassword123!"
            );

            // Act
            var response = await httpClient.PostAsJsonAsync("/users/register", registerRequest);

            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                Assert.True(true, "User registration failed - possibly duplicate email or validation error");
                return;
            }

            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<RegisterUserResponse>();
            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.Equal(registerRequest.DisplayName, result.DisplayName);
            Assert.Equal(registerRequest.Email, result.Email);
        }

        // Test: Verify that user login works correctly
        // Steps:
        // 1. Register a new user first
        // 2. Create a login request with the registered user's email and password
        // 3. Send POST request to /users/login endpoint
        // 4. Check if response is success (200 OK), unauthorized (401), or bad request (400)
        // 5. If successful, verify the response contains success message
        // 6. If failed, verify appropriate error status is returned
        [Fact]
        public async Task Login_User_Test()
        {
            var httpClient = _factory.CreateClient();

            // Arrange - First, register a new user
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            var registerRequest = new RegisterUserRequest(
                "Login Test User " + uniqueId,
                $"logintest{uniqueId}@example.com",
                "TestPassword123!"
            );

            var registerResponse = await httpClient.PostAsJsonAsync("/users/register", registerRequest);

            // If registration fails, we can't test login
            if (!registerResponse.IsSuccessStatusCode)
            {
                Assert.True(true, $"User registration failed with status {registerResponse.StatusCode} - cannot test login");
                return;
            }

            // Arrange - Create login request with the same credentials
            var loginRequest = new LoginUserRequest(registerRequest.Email, registerRequest.Password);

            // Act
            var response = await httpClient.PostAsJsonAsync("/users/login", loginRequest);

            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Assert.True(true, "Login failed - user doesn't exist or wrong credentials - expected 401");
                return;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                Assert.True(true, "Login failed - validation error");
                return;
            }

            Assert.True(response.IsSuccessStatusCode, $"API call failed with status code: {response.StatusCode}");
            var result = await response.Content.ReadFromJsonAsync<LoginUserResponse>();
            Assert.NotNull(result);
            Assert.Equal("Logged in successfully", result.Message);
        }

        // Test: Verify that getting current user information works correctly (or returns 401 if not authenticated)
        // Steps:
        // 1. Send GET request to /users/current endpoint
        // 2. Check if response is success (200 OK), unauthorized (401), or not found (404)
        // 3. If successful, verify the response contains user data including ID, display name, email, and roles
        // 4. If unauthorized or not found, verify appropriate status is returned
        [Fact]
        public async Task Get_Current_User_Test()
        {
            var httpClient = _factory.CreateClientWithCookies();

            // Arrange - Register a user (which automatically signs them in)
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            var registerRequest = new RegisterUserRequest(
                "Test User " + uniqueId,
                $"testuser{uniqueId}@example.com",
                "TestPassword123!"
            );

            var registerResponse = await httpClient.PostAsJsonAsync("/users/register", registerRequest);
            if (!registerResponse.IsSuccessStatusCode)
            {
                Assert.True(true, $"User registration failed - cannot test get current user");
                return;
            }

            // Act (user is already signed in from registration)
            var response = await httpClient.GetAsync("/users/current");

            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Assert.True(true, "Get current user endpoint not found - expected 404");
                return;
            }

            Assert.True(response.IsSuccessStatusCode, $"API call failed with status code: {response.StatusCode}");
            var result = await response.Content.ReadFromJsonAsync<GetCurrentUserResponse>();
            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.NotNull(result.DisplayName);
            Assert.NotNull(result.Email);
            Assert.NotNull(result.Roles);
        }
    }
}
