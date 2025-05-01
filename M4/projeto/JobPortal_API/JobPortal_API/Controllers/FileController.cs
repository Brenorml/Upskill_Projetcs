// JobPortal_API/Controllers/FileCVController.cs
using AutoMapper;
using AutoMapper.QueryableExtensions;
using JobPortal_API.Data;
using JobPortal_API.DTOs;
using JobPortal_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/filecv")]
    public class FileCVController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public FileCVController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET api/filecv/por-candidato/123
        [HttpGet("por-candidato/{idCandidato}")]
        public async Task<ActionResult<FileCVDTO>> GetFileCv(int idCandidato)
        {
            var dto = await _context.FileCV
                .Where(f => f.IdCandidatoFile == idCandidato)
                .ProjectTo<FileCVDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // POST api/filecv
        [HttpPost]
        public async Task<IActionResult> PostFileCv(
            [FromForm] IFormFile file,
            [FromForm] int idCandidatoFile)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Ficheiro inválido.");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var ent = new FileCV
            {
                File = ms.ToArray(),
                IdCandidatoFile = idCandidatoFile
            };
            _context.FileCV.Add(ent);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // PUT api/filecv/123
        [HttpPut("{idCandidato}")]
        public async Task<IActionResult> PutFileCv(
            int idCandidato,
            [FromForm] IFormFile file)
        {
            var ent = await _context.FileCV
                .FirstOrDefaultAsync(f => f.IdCandidatoFile == idCandidato);
            if (ent == null) return NotFound();

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ent.File = ms.ToArray();
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/filecv/123
        [HttpDelete("{idCandidato}")]
        public async Task<IActionResult> DeleteFileCv(int idCandidato)
        {
            var ent = await _context.FileCV
                .FirstOrDefaultAsync(f => f.IdCandidatoFile == idCandidato);
            if (ent == null) return NotFound();

            _context.FileCV.Remove(ent);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
