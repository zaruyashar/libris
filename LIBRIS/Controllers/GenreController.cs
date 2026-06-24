using LIBRIS.Data;
using LIBRIS.DTOs;
using LIBRIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LIBRIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly ApplicationDbContext dbcontext;

        public GenreController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        [Route("GetGenres")]
        public async Task<IEnumerable<GenreReadDto>> GetGenres()
        {
            var genres = await dbcontext.Genres.ToListAsync();

            var genreDtos = genres.Select(a => new GenreReadDto
            {
                GenreId = a.GenreId,
                Name = a.Name,
                Description = a.Description,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt
            });

            return genreDtos;
        }

        [HttpGet]
        [Route("GetGenresById/{id}")]
        public async Task<ActionResult<GenreReadDto>> GetGenresById(int id)
        {
            var genre = await dbcontext.FindAsync<Genre>(id);

            if (genre == null)
            {
                return NotFound();
            }

            var genreDto = new GenreReadDto
            {
                GenreId = genre.GenreId,
                Name = genre.Name,
                Description = genre.Description,
                IsActive = genre.IsActive,
                CreatedAt = genre.CreatedAt
            };

            return genreDto;
        }

        [HttpPost]
        [Route("AddGenre")]
        public async Task<ActionResult<GenreReadDto>> AddGenre(GenreCreateDto genreDto)
        {
            var genre = new Genre
            {
                Name = genreDto.Name,
                Description = genreDto.Description,
                IsActive = genreDto.IsActive,
                CreatedAt = genreDto.CreatedAt
            };

            dbcontext.Add(genre);
            await dbcontext.SaveChangesAsync();

            var createdDto = new GenreReadDto
            {
                GenreId = genre.GenreId,
                Name = genre.Name,
                Description = genre.Description,
                IsActive = genre.IsActive,
                CreatedAt = genre.CreatedAt
            };

            return CreatedAtAction(nameof(GetGenresById), new { id = genre.GenreId }, createdDto);
        }

        [HttpPut]
        [Route("UpdateGenre/{id}")]
        public async Task<ActionResult<GenreReadDto>> UpdateGenre(int id, GenreCreateDto genreDto)
        {
            var genre = await dbcontext.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            genre.Name = genreDto.Name;
            genre.Description = genreDto.Description;
            genre.IsActive = genreDto.IsActive;
            genre.CreatedAt = genreDto.CreatedAt;

            await dbcontext.SaveChangesAsync();

            var updatedDto = new GenreReadDto
            {
                GenreId = genre.GenreId,
                Name = genre.Name,
                Description = genre.Description,
                IsActive = genre.IsActive,
                CreatedAt = genre.CreatedAt
            };

            return updatedDto;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await dbcontext.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            dbcontext.Genres.Remove(genre);
            await dbcontext.SaveChangesAsync();

            return NoContent();
        }
    }
}
