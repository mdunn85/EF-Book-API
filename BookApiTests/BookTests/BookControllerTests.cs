using System.Collections.Generic;
using System.Threading.Tasks;
using BookAPI.Book;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace BookApiTests.BookTests
{
    public class BookControllerTests
    {
        [Fact]
        public async Task List_ReturnsAListOfBooks()
        {
            Mock<IBooksRepository> mockRepo = new Mock<IBooksRepository>();
            List<Book> books = new List<Book>
            {
                new Book()
                {
                    Author = "a1",
                    Title = "t1",
                    ISBN = "i1",
                },
                new Book()
                {
                    Author = "a2",
                    Title = "t2",
                    ISBN = "i2",
                }
            };
            mockRepo
                .Setup(repo => repo.ListAsync())
                .ReturnsAsync(books);

            BooksController controller = new BooksController(mockRepo.Object);

            IActionResult response = await controller.List();

            Assert.IsType<OkObjectResult>(response);

            ObjectResult objectResponse = response as ObjectResult;

            Assert.Equal(200, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsSuccessfullResponse()
        {
            Book book = new Book()
            {
                Author = "a1",
                Title = "t1",
                ISBN = "i1",
            };
            Mock<IBooksRepository> mockRepo = new Mock<IBooksRepository>();
            mockRepo.Setup(repo => repo.InsertAsync(book)).ReturnsAsync(book);
            BooksController controller = new BooksController(mockRepo.Object);

            IActionResult response = await controller.Create(book);
            Assert.IsType<CreatedResult>(response);

            ObjectResult objectResponse = response as ObjectResult;

            Assert.Equal(201, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest()
        {
            Book book = new Book();
            Mock<IBooksRepository> mockRepo = new Mock<IBooksRepository>();
            mockRepo.Setup(repo => repo.InsertAsync(book)).ReturnsAsync(() => null);
            BooksController controller = new BooksController(mockRepo.Object);
            IActionResult response = await controller.Create(book);
            Assert.IsType<BadRequestObjectResult>(response);
            ObjectResult objectResponse = response as ObjectResult;
            Assert.Equal(400, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Get_ReturnsSuccessfullResponse()
        {
            Book book = new Book()
            {
                Author = "a1",
                Title = "t1",
                ISBN = "i1",
            };
            Mock<IBooksRepository> mockRepo = new Mock<IBooksRepository>();
            mockRepo.Setup(repo => repo.GetAsync("i1")).ReturnsAsync(book);
            BooksController controller = new BooksController(mockRepo.Object);
            IActionResult response = await controller.Get("i1");
            Assert.IsType<OkObjectResult>(response);
            ObjectResult objectResponse = response as ObjectResult;
            Assert.Equal(200, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Get_ReturnsNotFound()
        {
            Book book = new Book();
            Mock<IBooksRepository> mockRepo = new Mock<IBooksRepository>();
            mockRepo.Setup(repo => repo.GetAsync("")).ReturnsAsync(() => null);
            BooksController controller = new BooksController(mockRepo.Object);
            IActionResult response = await controller.Get("i1");
            Assert.IsType<NotFoundObjectResult>(response);
            ObjectResult objectResponse = response as ObjectResult;
            Assert.Equal(404, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsSuccessfullResponse()
        {
            Book book = new Book()
            {
                Author = "a1",
                Title = "t1",
                ISBN = "i1",
            };
            Mock<IBooksRepository> mockRepo = new Mock<IBooksRepository>();
            mockRepo.Setup(repo => repo.UpdateAsync("i1", book)).ReturnsAsync(true);
            BooksController controller = new BooksController(mockRepo.Object);
            IActionResult response = await controller.Update("i1", book);
            Assert.IsType<OkObjectResult>(response);
            ObjectResult objectResponse = response as ObjectResult;
            Assert.Equal(200, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest()
        {
            Book book = new Book();
            Mock<IBooksRepository> mockRepo = new Mock<IBooksRepository>();
            mockRepo.Setup(repo => repo.UpdateAsync("i1", book)).ReturnsAsync(false);
            BooksController controller = new BooksController(mockRepo.Object);
            IActionResult response = await controller.Update("i1", book);
            Assert.IsType<BadRequestObjectResult>(response);
            ObjectResult objectResponse = response as ObjectResult;
            Assert.Equal(400, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsSuccessfullResponse()
        {
            Mock<IBooksRepository> mockRepo = new Mock<IBooksRepository>();
            mockRepo.Setup(repo => repo.DeleteAsync("i1")).ReturnsAsync(true);
            BooksController controller = new BooksController(mockRepo.Object);
            IActionResult response = await controller.Delete("i1");
            Assert.IsType<OkObjectResult>(response);
            ObjectResult objectResponse = response as ObjectResult;
            Assert.Equal(200, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            Mock<IBooksRepository> mockRepo = new Mock<IBooksRepository>();
            mockRepo.Setup(repo => repo.DeleteAsync("i1")).ReturnsAsync(false);
            BooksController controller = new BooksController(mockRepo.Object);
            IActionResult response = await controller.Delete("i1");
            Assert.IsType<NotFoundObjectResult>(response);
            ObjectResult objectResponse = response as ObjectResult;
            Assert.Equal(404, objectResponse.StatusCode);
        }
    }
}
