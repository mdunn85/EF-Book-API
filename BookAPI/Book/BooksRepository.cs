using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPI.Book
{
    public class BooksRepository : IBooksRepository
    {
        private readonly string connectionString;

        public BooksRepository(IConfiguration config)
        {
            connectionString = config["ConnectionString"];
        }
        
        public async Task<IEnumerable<Book>> ListAsync()
        {
            IEnumerable<Book> books = new List<Book>();
            string sql = "SELECT * FROM `books`";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                books = await connection.QueryAsync<Book>(sql);
            }
            return books;
        }

        public async Task<Book> GetAsync(string isbn)
        {
            Book book;

            string sql = "SELECT * FROM books WHERE isbn = @ISBN;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                IEnumerable<Book> books = await connection.QueryAsync<Book>(sql, new { ISBN = isbn });
                book = books.Count() > 0 ? books.First() : null;
            }
            return book;
        }

        public async Task<Book> InsertAsync(Book book)
        {
            int affectedRows;
            string sql = "INSERT INTO books (isbn,title,author) Values (@ISBN, @Title, @Author);";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                affectedRows = await connection.ExecuteAsync(sql, book);
            }
            return affectedRows > 0 ? book : null;
        }

        public async Task<bool> UpdateAsync(string isbn, Book book)
        {
            int affectedRows;
            string sql = "UPDATE books SET title = @Title, author = @Author WHERE isbn = @ISBN;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                affectedRows = await connection.ExecuteAsync(sql, new { ISBN = isbn, book.Author, book.Title });
            }
            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(string isbn)
        {
            int affectedRows;
            string sql = "DELETE from books WHERE isbn = @ISBN;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                affectedRows = await connection.ExecuteAsync(sql, new { ISBN = isbn });
            }
            return affectedRows > 0;
        }
    }
}