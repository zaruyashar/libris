using LIBRIS.Data;
using LIBRIS.Models;
using LIBRIS.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LIBRIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext dbcontext;

        public AuthorController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        [Route("GetAuthors")]
        public async Task<IEnumerable<AuthorReadDto>> GetAuthors()
        {
            var authors = await dbcontext.Authors.ToListAsync();

            var authorDtos = authors.Select(a => new AuthorReadDto
            {
                AuthorId = a.AuthorId,
                FullName = a.FullName,
                Nationality = a.Nationality,
                BirthYear = a.BirthYear,
                IsActive = a.IsActive
            });

            return authorDtos;
        }

        [HttpGet]
        [Route("GetAuthorsById/{id}")]
        public async Task<ActionResult<AuthorReadDto>> GetAuthorsById(int id)
        {
            var author = await dbcontext.FindAsync<Author>(id);

            if (author == null)
            {
                return NotFound();
            }

            var authorDto = new AuthorReadDto
            {
                AuthorId = author.AuthorId,
                FullName = author.FullName,
                Nationality = author.Nationality,
                BirthYear = author.BirthYear,
                IsActive = author.IsActive
            };

            return authorDto;
        }

        [HttpPost]
        [Route("AddAuthor")]
        public async Task<ActionResult<AuthorReadDto>> AddAuthor(AuthorCreateDto authorDto)
        {
            var author = new Author
            {
                FullName = authorDto.FullName,
                Nationality = authorDto.Nationality,
                BirthYear = authorDto.BirthYear,
                IsActive = authorDto.IsActive
            };

            dbcontext.Add(author);
            await dbcontext.SaveChangesAsync();

            var createdDto = new AuthorReadDto
            {
                AuthorId = author.AuthorId,
                FullName = author.FullName,
                Nationality = author.Nationality,
                BirthYear = author.BirthYear,
                IsActive = author.IsActive
            };

            return CreatedAtAction(nameof(GetAuthorsById), new { id = author.AuthorId }, createdDto);
        }

        [HttpPut]
        [Route("UpdateAuthor/{id}")]
        public async Task<ActionResult<AuthorReadDto>> UpdateAuthor(int id, AuthorCreateDto authorDto)
        {
            var author = await dbcontext.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            author.FullName = authorDto.FullName;
            author.Nationality = authorDto.Nationality;
            author.BirthYear = authorDto.BirthYear;
            author.IsActive = authorDto.IsActive;

            await dbcontext.SaveChangesAsync();

            var updatedDto = new AuthorReadDto
            {
                AuthorId = author.AuthorId,
                FullName = author.FullName,
                Nationality = author.Nationality,
                BirthYear = author.BirthYear,
                IsActive = author.IsActive
            };

            return updatedDto;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await dbcontext.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            dbcontext.Authors.Remove(author);
            await dbcontext.SaveChangesAsync();

            return NoContent();
        }
    }
}
