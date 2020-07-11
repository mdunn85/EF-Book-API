using System.Data;
using BookAPI.Book;
using BookAPI.Database;
using Moq;
using Xunit;
using Moq.Dapper;
using Dapper;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace BookApiTests.BookTests
{
    public class BookRepositoryTests
    {
        [Fact]
        public async void GetBooksTest_GetsBooksFromTheDatabase()
        {
            var mockBooks = new List<Book>
            {
                new Book
                {
                    ISBN = "Test ISBN 1",
                    Author = "Test Author 1",
                    Title = "Test Title 1"
                },
                new Book
                {
                    ISBN = "Test ISBN 2",
                    Author = "Test Author 2",
                    Title = "Test Title 2"
                }
            };

            var connectionMock = new Mock<DbConnection>();

            connectionMock
                .SetupDapperAsync(m => m.QueryAsync<Book>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()))
                .ReturnsAsync(mockBooks);


            var connectionFactoryMock = new Mock<IDbConnectionFactory>();

            connectionFactoryMock
                .Setup(m => m.CreateConnection())
                .Returns(connectionMock.Object);


            var booksRepository = new BooksRepository(connectionFactoryMock.Object);

            var books = await booksRepository.ListAsync();

            Assert.Equal(mockBooks.Count, books.AsList().Count);
        }

        [Fact]
        public async void GetBookTest_GetBookFromTheDatabase()
        {
            var mockBooks = new List<Book>
            {
                new Book
                {
                    ISBN = "Test ISBN 1",
                    Author = "Test Author 1",
                    Title = "Test Title 1"
                },
                new Book
                {
                    ISBN = "Test ISBN 2",
                    Author = "Test Author 2",
                    Title = "Test Title 2"
                }
            };

            var connectionMock = new Mock<DbConnection>();

            connectionMock
                .SetupDapperAsync(m => m.QueryAsync<Book>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()))
                .ReturnsAsync(mockBooks);


            var connectionFactoryMock = new Mock<IDbConnectionFactory>();

            connectionFactoryMock
                .Setup(m => m.CreateConnection())
                .Returns(connectionMock.Object);


            var booksRepository = new BooksRepository(connectionFactoryMock.Object);

            var book = await booksRepository.GetAsync(mockBooks.First().ISBN);

            Assert.Equal(mockBooks.First().ISBN, book.ISBN);
        }

        [Fact]
        public async void AddBookTest_AddsBookToTheDatabaseAndReturnIt()
        {
            Book mockBook = new Book
            {
                ISBN = "Test ISBN",
                Author = "Test Author",
                Title = "Test Title"
            };

            var connectionMock = new Mock<DbConnection>();

            connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            var connectionFactoryMock = new Mock<IDbConnectionFactory>();

            connectionFactoryMock
                .Setup(m => m.CreateConnection())
                .Returns(connectionMock.Object);

            var booksRepository = new BooksRepository(connectionFactoryMock.Object);

            var insertedBook = await booksRepository.InsertAsync(mockBook);

            Assert.Equal(insertedBook, mockBook);
        }

        [Fact]
        public async void UpdateBookTest_UpdatesBookToTheDatabaseAndReturnsIfUpdated()
        {
            Book mockBook = new Book
            {
                ISBN = "Test ISBN",
                Author = "Test Author",
                Title = "Test Title"
            };

            var connectionMock = new Mock<DbConnection>();

            connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            var connectionFactoryMock = new Mock<IDbConnectionFactory>();

            connectionFactoryMock
                .Setup(m => m.CreateConnection())
                .Returns(connectionMock.Object);

            var booksRepository = new BooksRepository(connectionFactoryMock.Object);

            var updatedBook = await booksRepository.UpdateAsync(mockBook.ISBN, mockBook);

            Assert.True(updatedBook);
        }

        [Fact]
        public async void DeleteBookTest_DeletesBookFromTheDatabaseAndReturnsIfDeleted()
        {
            var connectionMock = new Mock<DbConnection>();

            connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()))
                .ReturnsAsync(1);

            var connectionFactoryMock = new Mock<IDbConnectionFactory>();

            connectionFactoryMock
                .Setup(m => m.CreateConnection())
                .Returns(connectionMock.Object);

            var booksRepository = new BooksRepository(connectionFactoryMock.Object);

            var deletedBook = await booksRepository.DeleteAsync("Test ISBN");

            Assert.True(deletedBook);
        }
    }
}
