using LIBRIS.Data;
using LIBRIS.DTOs;
using LIBRIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LIBRIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext dbcontext;

        public BookController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        [Route("GetBooks")]
        public async Task<IEnumerable<BookReadDto>> GetBooks()
        {
            var books = await dbcontext.Books
                        .Include(b => b.Author)
                        .Include(b => b.Genre)
                        .ToListAsync();

            var bookDtos = books.Select(b => new BookReadDto
            {
                BookId = b.BookId,
                GenreId = b.GenreId,
                GenreName = b.Genre.Name,
                AuthorId = b.AuthorId,
                AuthorName = b.Author.FullName,
                Title = b.Title,
                Isbn = b.Isbn
            });

            return bookDtos;
        }

        [HttpGet]
        [Route("GetBooksById/{id}")]
        public async Task<ActionResult<BookReadDto>> GetBooksById(int id)
        {
            var book = await dbcontext.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            var bookDto = new BookReadDto
            {
                BookId = book.BookId,
                AuthorId = book.AuthorId,
                AuthorName = book.Author.FullName,
                GenreId = book.GenreId,
                GenreName = book.Genre.Name,
                Title = book.Title,
                Isbn = book.Isbn
            };

            return bookDto;
        }

        [HttpPost]
        [Route("AddBook")]
        public async Task<ActionResult<BookReadDto>> AddBook(BookCreateDto bookDto)
        {
            var book = new Book
            {
                AuthorId = bookDto.AuthorId,
                GenreId = bookDto.GenreId,
                Title = bookDto.Title,
                Isbn = bookDto.Isbn
            };

            dbcontext.Books.Add(book);
            await dbcontext.SaveChangesAsync();

            await dbcontext.Entry(book).Reference(b => b.Author).LoadAsync();
            await dbcontext.Entry(book).Reference(b => b.Genre).LoadAsync();

            var createdDto = new BookReadDto
            {
                BookId = book.BookId,
                AuthorId = book.AuthorId,
                AuthorName = book.Author.FullName,
                GenreId = book.GenreId,
                GenreName = book.Genre.Name,
                Title = book.Title,
                Isbn = book.Isbn
            };

            return CreatedAtAction(nameof(GetBooksById), new { id = book.BookId }, createdDto);
        }

        [HttpPut]
        [Route("UpdateBook/{id}")]
        public async Task<ActionResult<BookReadDto>> UpdateBook(int id, BookCreateDto bookDto)
        {
            var book = await dbcontext.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            book.AuthorId = bookDto.AuthorId;
            book.GenreId = bookDto.GenreId;
            book.Title = bookDto.Title;
            book.Isbn = bookDto.Isbn;

            await dbcontext.SaveChangesAsync();

            await dbcontext.Entry(book).Reference(b => b.Author).LoadAsync();
            await dbcontext.Entry(book).Reference(b => b.Genre).LoadAsync();

            var updatedDto = new BookReadDto
            {
                BookId = book.BookId,
                AuthorId = book.AuthorId,
                AuthorName = book.Author.FullName,
                GenreId = book.GenreId,
                GenreName = book.Genre.Name,
                Title = book.Title,
                Isbn = book.Isbn
            };

            return updatedDto;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await dbcontext.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            dbcontext.Books.Remove(book);
            await dbcontext.SaveChangesAsync();

            return NoContent();
        }
    }
}
