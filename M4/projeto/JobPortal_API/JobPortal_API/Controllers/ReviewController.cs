using AutoMapper;
using JobPortal_API.DTOs;
using JobPortal_API.Models;
using JobPortal_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobPortal_API.Controllers
{
    [Route("api/review")]
    [ApiController]
    public class ReviewAPIController : ControllerBase
    {
        private readonly IReviewRepository _repository;
        private readonly IMapper _mapper;

        public ReviewAPIController(IReviewRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            var reviews = await _repository.GetAllAsync();
            return Ok(reviews);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            var review = await _repository.GetAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(review);
        }

        [HttpGet("empresa/{empresaId:int}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByEmpresa(int empresaId)
        {
            var reviews = await _repository.GetReviewsByEmpresaIdAsync(empresaId);
            return Ok(reviews);
        }

        [Authorize(Roles = "Admin, Candidato")]
        [HttpPost]
        public async Task<ActionResult<Review>> CreateReview([FromBody] ReviewDTO reviewDto)
        {
            if (reviewDto == null)
            {
                return BadRequest("Dados da review não podem ser nulos.");
            }

            if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
            {
                return BadRequest("O rating deve ser um valor entre 1 e 5.");
            }

            // Validar se IdEmpresa existe
            var empresaExists = await _repository.EmpresaExistsAsync(reviewDto.IdEmpresa);
            if (!empresaExists)
            {
                return BadRequest("A empresa especificada não existe.");
            }

            try
            {
                var review = _mapper.Map<Review>(reviewDto);
                review.DataCriacao = DateTime.Now;

                var createdReview = await _repository.CreateAsync(review);
                return CreatedAtAction(nameof(GetReview), new { id = createdReview.IdReview }, createdReview);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno ao criar a review: " + ex.Message);
            }
        }

        //[HttpPost]
        //public async Task<ActionResult<Review>> CreateReview([FromBody] ReviewDTO reviewDto)
        //{
        //    if (reviewDto == null)
        //    {
        //        return BadRequest("Dados da review não podem ser nulos.");
        //    }

        //    if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
        //    {
        //        return BadRequest("O rating deve ser um valor entre 1 e 5.");
        //    }

        //    try
        //    {
        //        // Mapear o DTO para a entidade Review
        //        var review = _mapper.Map<Review>(reviewDto);
        //        review.DataCriacao = DateTime.Now; // Definir manualmente

        //        var createdReview = await _repository.CreateAsync(review);
        //        return CreatedAtAction(nameof(GetReview), new { id = createdReview.IdReview }, createdReview);
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Logar o erro, se necessário
        //        return StatusCode(500, "Erro interno ao criar a review.");
        //    }
        //}

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewDTO reviewDto)
        {
            if (reviewDto == null)
            {
                return BadRequest("Dados da review não podem ser nulos.");
            }

            if (id <= 0)
            {
                return BadRequest("ID da review inválido.");
            }

            try
            {
                // Buscar a review existente
                var reviewToUpdate = await _repository.GetAsync(id);
                if (reviewToUpdate == null)
                {
                    return NotFound("Review não encontrada.");
                }

                // Mapear os novos dados do DTO para a review existente
                var updatedReview = _mapper.Map(reviewDto, reviewToUpdate); // Note o uso de Map com destino

                // Garantir que o IdReview não seja alterado
                updatedReview.IdReview = id;

                // Atualizar no repositório
                await _repository.UpdateAsync(updatedReview);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Logar o erro, se necessário
                return StatusCode(500, "Erro interno ao atualizar a review.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var result = await _repository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
