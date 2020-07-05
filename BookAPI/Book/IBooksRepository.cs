using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookAPI.Book
{
    public interface IBooksRepository
    {
        Task<IEnumerable<Book>> ListAsync();
        Task<Book> GetAsync(string isbn);
        Task<Book> InsertAsync(Book book);
        Task<bool> UpdateAsync(string isbn, Book book);
        Task<bool> DeleteAsync(string isbn);
    }
}
