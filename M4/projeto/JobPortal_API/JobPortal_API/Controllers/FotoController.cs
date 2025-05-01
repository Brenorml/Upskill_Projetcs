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

    [ApiController]
    [Route("api/foto")]
    public class FotoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public FotoController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //todos os registros
        [HttpGet("TodasFotos")]
        public async Task<IEnumerable<FotoDTO>> GetFoto()
        {
            return await _context.Foto.ProjectTo<FotoDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }
        //busca por ID do candidato
        [AllowAnonymous]
        [HttpGet("FotoPorId{id}")]
        public async Task<ActionResult<FotoDTO>> GetFoto(int idCandidato)
        {
            if (_context.Foto == null)
            {
                return NotFound();
            }
            var foto = await _context.Foto.ProjectTo<FotoDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(m => m.IdCandidatoFoto == idCandidato);
            if (foto == null)
            {
                return NotFound();
            }

            return Ok(foto);
            //return await foto;
        }

        [AllowAnonymous]
        [HttpGet("BuscarFotoPorIdCandidato/{idCandidato}")]
        public async Task<IActionResult> GetFotoPorCandidato(int idCandidato)
        {
            var foto = await _context.Foto
                .FirstOrDefaultAsync(m => m.IdCandidatoFoto == idCandidato);

            if (foto == null || foto.FotoPerfil == null)
            {
                return NotFound();
            }

            return File(foto.FotoPerfil, "image/jpeg");
        }

        
        //Criar candidato
        [HttpPost("CriarFoto")]
        public async Task<ActionResult> PostFoto(FotoDTO fotoDTO)
        {
            var foto = _mapper.Map<Foto>(fotoDTO);
            _context.Add(foto);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //editar candidato
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutFoto(int id, FotoDTO fotoDTO)
        {
            var foto = await _context.Foto.FirstOrDefaultAsync(c => c.Id == id);
            if (foto == null)
            {
                return NotFound();
            }

            // Verificar se o IdCandidatoFoto corresponde
            if (foto.IdCandidatoFoto != fotoDTO.IdCandidatoFoto)
            {
                return BadRequest("O IdCandidatoFoto não pode ser alterado.");
            }

            foto = _mapper.Map(fotoDTO, foto);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //delete
        [HttpDelete("DeletarFoto/{id:int}")]
        public async Task<ActionResult> DeleteFoto(int id)
        {
            var foto = await _context.Foto.FirstOrDefaultAsync(c => c.IdCandidatoFoto == id);
            if (foto == null)
            {
                return NotFound();
            }
            _context.Foto.Remove(foto);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("ByCandidato/{idCandidato}")]
        public async Task<ActionResult<FotoDTO>> GetFotoJson(int idCandidato)
        {
            if (_context.Foto == null)
            {
                return NotFound();
            }

            var foto = await _context.Foto
                .ProjectTo<FotoDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(m => m.IdCandidatoFoto == idCandidato);

            if (foto == null)
            {
                return NotFound();
            }

            return Ok(foto);
        }

    }
}
