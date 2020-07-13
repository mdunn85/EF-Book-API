using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using BookAPI;
using BookAPI.Book;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Xunit;

namespace BookApiTests.BookTests
{
    public class BookIntegrationTests
    {
        [Fact]
        public async void EndpointTests()
        {
            var configuration = new ConfigurationBuilder()
               .AddEnvironmentVariables()
               .AddJsonFile("appsettings.Testing.json")
               .Build();

            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseEnvironment("Development");

                    webHost.UseTestServer();

                    webHost.UseStartup(typeof(Startup)).UseConfiguration(configuration);
                });

            var host = await hostBuilder.StartAsync();

            var client = host.GetTestClient();

            // Try to add an emtpy book

            var mockBook = new Book();

            var content = new StringContent(JsonConvert.SerializeObject(mockBook), UnicodeEncoding.UTF8, "application/json");

            using (var response = await client.PostAsync("/books", content))
            {
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }

            // Add a book

            mockBook = new Book
            {
                ISBN = "Test ISBN",
                Title = "Test Title",
                Author = "Test Author"
            };

            content = new StringContent(JsonConvert.SerializeObject(mockBook), UnicodeEncoding.UTF8, "application/json");

            using (var response = await client.PostAsync("/books", content))
            {
                Assert.True(response.IsSuccessStatusCode);
            }

            // Try to add same book

            content = new StringContent(JsonConvert.SerializeObject(mockBook), UnicodeEncoding.UTF8, "application/json");

            using (var response = await client.PostAsync("/books", content))
            {
                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            }

            // Should be in all books list

            using (var response = await client.GetAsync("/books"))
            {

                Assert.True(response.IsSuccessStatusCode);

                var json = await response.Content.ReadAsStringAsync();

                var book = JsonConvert.DeserializeObject<List<Book>>(json).First();

                Assert.Equal(mockBook.ISBN, book.ISBN);
                Assert.Equal(mockBook.Author, book.Author);
                Assert.Equal(mockBook.Title, book.Title);
            }

            // Get the book

            using (var response = await client.GetAsync($"/books/{mockBook.ISBN}"))
            {
                Assert.True(response.IsSuccessStatusCode);

                var json = await response.Content.ReadAsStringAsync();

                var book = JsonConvert.DeserializeObject<Book>(json);

                Assert.Equal(mockBook.ISBN, book.ISBN);
                Assert.Equal(mockBook.Author, book.Author);
                Assert.Equal(mockBook.Title, book.Title);
            }

            // Try to update a book with an empty book

            content = new StringContent(JsonConvert.SerializeObject(new Book()), UnicodeEncoding.UTF8, "application/json");

            using (var response = await client.PutAsync($"/books/{mockBook.ISBN}", content))
            {
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }

            // Update a book

            mockBook.Title = "Updated title";

            content = new StringContent(JsonConvert.SerializeObject(mockBook), UnicodeEncoding.UTF8, "application/json");

            using (var response = await client.PutAsync($"/books/{mockBook.ISBN}", content))
            {
                Assert.True(response.IsSuccessStatusCode);
            }

            // Check book is updated

            using (var response = await client.GetAsync($"/books/{mockBook.ISBN}"))
            {
                Assert.True(response.IsSuccessStatusCode);

                var json = await response.Content.ReadAsStringAsync();

                var book = JsonConvert.DeserializeObject<Book>(json);

                Assert.Equal(mockBook.ISBN, book.ISBN);
                Assert.Equal(mockBook.Author, book.Author);
                Assert.Equal(mockBook.Title, book.Title);
            }

            // Delete the book

            using (var response = await client.DeleteAsync($"/books/{mockBook.ISBN}"))
            {
                Assert.True(response.IsSuccessStatusCode);
            }

            // Get non existent book

            using (var response = await client.GetAsync($"/books/{mockBook.ISBN}"))
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            // Try to delete the deleted book

            using (var response = await client.DeleteAsync($"/books/{mockBook.ISBN}"))
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            // Try to update the deleted book

            content = new StringContent(JsonConvert.SerializeObject(mockBook), UnicodeEncoding.UTF8, "application/json");

            using (var response = await client.PutAsync($"/books/{mockBook.ISBN}", content))
            {
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }

            // Check no books in database

            using (var response = await client.GetAsync("/books"))
            {
                Assert.True(response.IsSuccessStatusCode);

                var json = await response.Content.ReadAsStringAsync();

                var books = JsonConvert.DeserializeObject<List<Book>>(json);

                Assert.Empty(books);
            }
        }
    }
}
