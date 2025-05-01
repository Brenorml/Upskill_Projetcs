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
    [Route("api/cv")]
    [ApiController]
    public class CVController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CVController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET api/cv
        [HttpGet]
        public async Task<IEnumerable<CVDTO>> GetAll()
        {
            return await _context.CV
                .ProjectTo<CVDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // GET api/cv/idCandidato?idCandidato=123
        [HttpGet("idCandidato")]
        public async Task<ActionResult<CVDTO>> GetByCandidato([FromQuery] int idCandidato)
        {
            var cv = await _context.CV
                .Where(c => c.IdCandidatoCv == idCandidato)
                .ProjectTo<CVDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            if (cv == null) return NotFound();
            return Ok(cv);
        }

        // POST api/cv
        [HttpPost]
        public async Task<ActionResult<CVDTO>> PostCv([FromBody] CVDTO cvDTO)
        {
            var cv = _mapper.Map<CV>(cvDTO);
            _context.CV.Add(cv);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<CVDTO>(cv);
            return CreatedAtAction(nameof(GetByCandidato),
                                   new { idCandidato = cv.IdCandidatoCv },
                                   resultDto);
        }

        // PUT api/cv/5
        [HttpPut("{id}")]
        public async Task<ActionResult<CVDTO>> PutCv(int id, [FromBody] CVDTO cvDTO)
        {
            if (id != cvDTO.IdCV) return BadRequest();

            var cv = await _context.CV.FindAsync(id);
            if (cv == null) return NotFound();

            _mapper.Map(cvDTO, cv);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<CVDTO>(cv));
        }
    }
}
