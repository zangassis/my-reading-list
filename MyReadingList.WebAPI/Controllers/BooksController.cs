using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyReadingList.WebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MyReadingList.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        private readonly BookContext _context;

        public BooksController(BookContext context)
        {
            _context = context;
        }

        //Get all books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        //Get all books read
        [HttpGet("read")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksRead()
        {
            bool read = true;
            var books = await _context.Books.ToListAsync();

            var booksRead = (from book in books where book.Read == read select book).ToList();

            return booksRead;
        }

        //Get a Book by id  
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(string id)
        {
            Guid guidId = Guid.Parse(id);

            var book = await _context.Books.FindAsync(guidId);

            if (book == null)
                return NotFound();

            return book;
        }

        //Add a new book
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Book>> Create(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        //Update an existing book
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Book book)
        {
            if (id != Convert.ToString(book.Id).ToUpper())
                return BadRequest();

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                    return NotFound();
            }

            return NoContent();
        }

        //Delete an existing book
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            Guid guidId = Guid.Parse(id);

            var book = await _context.Books.FindAsync(guidId);

            if (book == null)
                return NotFound();

            _context.Books.Remove(book);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Check if the book exists in the database
        private bool BookExists(string id)
        {
            return _context.Books.Any(e => Convert.ToString(e.Id) == id);
        }
    }
}
