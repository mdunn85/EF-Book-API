using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookAPI.Book
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : Controller
    {
        private readonly IBooksRepository _booksRepository;

        public BooksController(IBooksRepository bookRepository)
        {
            _booksRepository = bookRepository;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {
                return Ok(await _booksRepository.ListAsync());
            }
            catch (Exception e)
            {
                return Problem(e.Message, statusCode: 500);
            }
        }

        [HttpGet("{isbn}")]
        public async Task<IActionResult> Get([FromRoute] string isbn)
        {
            try
            {
                Book book = await _booksRepository.GetAsync(isbn);
                if (book == null)
                {
                    return NotFound("Book not found");
                }
                else
                {
                    return Ok(book);
                }
            }
            catch (Exception e)
            {
                return Problem(e.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            try
            {
                if (await _booksRepository.InsertAsync(book) != null)
                {
                    return Created($"/books/{book.ISBN}", book);
                }
                return BadRequest("Error saving book");
            }
            catch (Exception e)
            {
                return Problem(e.Message, statusCode: 500);
            }
        }

        [HttpPut("{isbn}")]
        public async Task<IActionResult> Update([FromRoute] string isbn, [FromBody] Book book)
        {
            try
            {
                if (await _booksRepository.UpdateAsync(isbn, book))
                {
                    return Ok("Book updated");
                }
                return BadRequest("Error updating book");
            }
            catch (Exception e)
            {
                return Problem(e.Message, statusCode: 500);
            }
        }

        [HttpDelete("{isbn}")]
        public async Task<IActionResult> Delete([FromRoute] string isbn)
        {
            try
            {
                if (await _booksRepository.DeleteAsync(isbn))
                {
                    return Ok("Book deleted");
                }
                return NotFound("Error deleting book");
            }
            catch (Exception e)
            {
                return Problem(e.Message, statusCode: 500);
            }
        }
    }
}
