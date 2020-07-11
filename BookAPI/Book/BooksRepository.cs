using BookAPI.Database;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPI.Book
{
    public class BooksRepository : IBooksRepository
    {
        private IDbConnectionFactory connectionFactory;

        public BooksRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Get all books
        /// </summary>
        /// <returns>
        /// List of books
        /// </returns>

        public async Task<IEnumerable<Book>> ListAsync()
        {
            IEnumerable<Book> books = new List<Book>();
            string sql = "SELECT * FROM `books`";
            using (var connection = connectionFactory.CreateConnection())
            {
                books = await connection.QueryAsync<Book>(sql);
            }
            return books;
        }

        /// <summary>
        /// Get a book
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns>
        /// A book
        /// </returns>

        public async Task<Book> GetAsync(string isbn)
        {
            Book book;

            string sql = "SELECT * FROM books WHERE isbn = @ISBN;";

            using (var connection = connectionFactory.CreateConnection())
            {
                IEnumerable<Book> books = await connection.QueryAsync<Book>(sql, new { ISBN = isbn });
                book = books.Count() > 0 ? books.First() : null;
            }
            return book;
        }

        /// <summary>
        /// Inserts a book
        /// </summary>
        /// <param name="book"></param>
        /// <returns>
        /// The inserted book or null on failure
        /// </returns>

        public async Task<Book> InsertAsync(Book book)
        {
            int affectedRows;
            string sql = "INSERT INTO books (isbn,title,author) Values (@ISBN, @Title, @Author);";

            using (var connection = connectionFactory.CreateConnection())
            {
                affectedRows = await connection.ExecuteAsync(sql, book);
            }
            return affectedRows > 0 ? book : null;
        }

        /// <summary>
        /// Update a book
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="book"></param>
        /// <returns>
        /// ture or false depending if book was updated
        /// </returns>

        public async Task<bool> UpdateAsync(string isbn, Book book)
        {
            int affectedRows;
            string sql = "UPDATE books SET title = @Title, author = @Author WHERE isbn = @ISBN;";

            using (var connection = connectionFactory.CreateConnection())
            {
                affectedRows = await connection.ExecuteAsync(sql, new { ISBN = isbn, book.Author, book.Title });
            }
            return affectedRows > 0;
        }

        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns>
        /// ture or false depending if book was deleted
        /// </returns>

        public async Task<bool> DeleteAsync(string isbn)
        {
            int affectedRows;
            string sql = "DELETE from books WHERE isbn = @ISBN;";

            using (var connection = connectionFactory.CreateConnection())
            {
                affectedRows = await connection.ExecuteAsync(sql, new { ISBN = isbn });
            }
            return affectedRows > 0;
        }
    }
}