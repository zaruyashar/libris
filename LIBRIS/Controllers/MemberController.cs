using LIBRIS.Data;
using LIBRIS.DTOs;
using LIBRIS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LIBRIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly ApplicationDbContext dbcontext;

        public MemberController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        [Route("GetMembers")]
        public async Task<IEnumerable<MemberReadDto>> GetMembers()
        {
            var members = await dbcontext.Members.ToListAsync();

            var memberDtos = members.Select(a => new MemberReadDto
            {
                MemberId = a.MemberId,
                FullName = a.FullName,
                Email = a.Email,
                MemberSince = a.MemberSince,
                IsActive = a.IsActive
            });

            return memberDtos;
        }

        [HttpGet]
        [Route("GetMembersById/{id}")]
        public async Task<ActionResult<MemberReadDto>> GetMembersById(int id)
        {
            var member = await dbcontext.FindAsync<Member>(id);

            if (member == null)
            {
                return NotFound();
            }

            var memberDto = new MemberReadDto
            {
                MemberId = member.MemberId,
                FullName = member.FullName,
                Email = member.Email,
                MemberSince = member.MemberSince,
                IsActive = member.IsActive
            };

            return memberDto;
        }

        [HttpPost]
        [Route("AddMember")]
        public async Task<ActionResult<MemberReadDto>> AddMember(MemberCreateDto memberDto)
        {
            var member = new Member
            {
                FullName = memberDto.FullName,
                Email = memberDto.Email,
                MemberSince = memberDto.MemberSince,
                IsActive = memberDto.IsActive
            };

            dbcontext.Add(member);
            await dbcontext.SaveChangesAsync();

            var createdDto = new MemberReadDto
            {
                MemberId = member.MemberId,
                FullName = member.FullName,
                Email = member.Email,
                MemberSince = member.MemberSince,
                IsActive = member.IsActive
            };

            return CreatedAtAction(nameof(GetMembersById), new { id = member.MemberId }, createdDto);
        }

        [HttpPut]
        [Route("UpdateMember/{id}")]
        public async Task<ActionResult<MemberReadDto>> UpdateMember(int id, MemberCreateDto memberDto)
        {
            var member = await dbcontext.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            member.FullName = memberDto.FullName;
            member.Email = memberDto.Email;
            member.MemberSince = memberDto.MemberSince;
            member.IsActive = memberDto.IsActive;

            await dbcontext.SaveChangesAsync();

            var updatedDto = new MemberReadDto
            {
                MemberId = member.MemberId,
                FullName = member.FullName,
                Email = member.Email,
                MemberSince = member.MemberSince,
                IsActive = member.IsActive
            };

            return updatedDto;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await dbcontext.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            dbcontext.Members.Remove(member);
            await dbcontext.SaveChangesAsync();

            return NoContent();
        }
    }
}

