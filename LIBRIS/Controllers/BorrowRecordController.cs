using LIBRIS.Data;
using LIBRIS.DTOs;
using LIBRIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LIBRIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowRecordController : ControllerBase
    {
        private readonly ApplicationDbContext dbcontext;

        public BorrowRecordController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        [Route("GetBorrowRecords")]
        public async Task<IEnumerable<BorrowRecordReadDto>> GetBorrowRecords()
        {
            var borrowRecords = await dbcontext.BorrowRecords
                        .Include(b => b.Book)
                        .Include(b => b.Member)
                        .ToListAsync();

            var borrowRecordDtos = borrowRecords.Select(b => new BorrowRecordReadDto
            {
                BorrowId = b.BorrowId,
                BookId = b.BookId,
                BookTitle = b.Book.Title,
                MemberId = b.MemberId,
                MemberFullName = b.Member.FullName,
                BorrowedAt = b.BorrowedAt,
                ReturnedAt = b.ReturnedAt
            });

            return borrowRecordDtos;
        }

        [HttpGet]
        [Route("GetBorrowRecordsById/{id}")]
        public async Task<ActionResult<BorrowRecordReadDto>> GetBorrowRecordsById(int id)
        {
            var borrowRecord = await dbcontext.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.Member)
                .FirstOrDefaultAsync(b => b.BorrowId == id);

            if (borrowRecord == null)
            {
                return NotFound();
            }

            var borrowRecordDto = new BorrowRecordReadDto
            {
                BorrowId = borrowRecord.BorrowId,
                BookId = borrowRecord.BookId,
                BookTitle = borrowRecord.Book.Title,
                MemberId = borrowRecord.MemberId,
                MemberFullName = borrowRecord.Member.FullName,
                BorrowedAt = borrowRecord.BorrowedAt,
                ReturnedAt = borrowRecord.ReturnedAt
            };

            return borrowRecordDto;
        }

        [HttpPost]
        [Route("AddBorrowRecord")]
        public async Task<ActionResult<BorrowRecordReadDto>> AddBorrowRecord(BorrowRecordCreateDto borrowRecordDto)
        {
            var borrowRecord = new BorrowRecord
            {
                BookId = borrowRecordDto.BookId,
                MemberId = borrowRecordDto.MemberId,
                BorrowedAt = borrowRecordDto.BorrowedAt,
                ReturnedAt = borrowRecordDto.ReturnedAt
            };

            dbcontext.BorrowRecords.Add(borrowRecord);
            await dbcontext.SaveChangesAsync();

            await dbcontext.Entry(borrowRecord).Reference(b => b.Book).LoadAsync();
            await dbcontext.Entry(borrowRecord).Reference(b => b.Member).LoadAsync();

            var createdDto = new BorrowRecordReadDto
            {
                BorrowId = borrowRecord.BorrowId,
                BookId = borrowRecord.BookId,
                BookTitle = borrowRecord.Book.Title,
                MemberId = borrowRecord.MemberId,
                MemberFullName = borrowRecord.Member.FullName,
                BorrowedAt = borrowRecord.BorrowedAt,
                ReturnedAt = borrowRecord.ReturnedAt
            };

            return CreatedAtAction(nameof(GetBorrowRecordsById), new { id = borrowRecord.BorrowId }, createdDto);
        }

        [HttpPut]
        [Route("UpdateBorrowRecord/{id}")]
        public async Task<ActionResult<BorrowRecordReadDto>> UpdateBorrowRecord(int id, BorrowRecordCreateDto borrowRecordDto)
        {
            var borrowRecord = await dbcontext.BorrowRecords.FindAsync(id);

            if (borrowRecord == null)
            {
                return NotFound();
            }

            borrowRecord.BookId = borrowRecordDto.BookId;
            borrowRecord.MemberId = borrowRecordDto.MemberId;
            borrowRecord.BorrowedAt = borrowRecordDto.BorrowedAt;
            borrowRecord.ReturnedAt = borrowRecordDto.ReturnedAt;

            await dbcontext.SaveChangesAsync();

            await dbcontext.Entry(borrowRecord).Reference(b => b.Book).LoadAsync();
            await dbcontext.Entry(borrowRecord).Reference(b => b.Member).LoadAsync();

            var updatedDto = new BorrowRecordReadDto
            {
                BorrowId = borrowRecord.BorrowId,
                BookId = borrowRecord.BookId,
                BookTitle = borrowRecord.Book.Title,
                MemberId = borrowRecord.MemberId,
                MemberFullName = borrowRecord.Member.FullName,
                BorrowedAt = borrowRecord.BorrowedAt,
                ReturnedAt = borrowRecord.ReturnedAt
            };

            return updatedDto;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrowRecord(int id)
        {
            var borrowRecord = await dbcontext.BorrowRecords.FindAsync(id);

            if (borrowRecord == null)
            {
                return NotFound();
            }

            dbcontext.BorrowRecords.Remove(borrowRecord);
            await dbcontext.SaveChangesAsync();

            return NoContent();
        }
    }
}
