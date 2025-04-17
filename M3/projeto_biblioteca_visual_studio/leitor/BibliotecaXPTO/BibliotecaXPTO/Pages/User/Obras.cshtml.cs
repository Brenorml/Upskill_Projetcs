using EscolaXPTO_EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using static EscolaXPTO_EF.Dto.Dtos;

namespace BibliotecaXPTO.Pages.User
{
    public class ObrasModel : PageModel
    {
        private readonly ILogger<ObrasModel> _logger;
        private readonly EscolaEF _escolaEF;

        public ObrasModel(ILogger<ObrasModel> logger, EscolaEF escolaEF)
        {
            _logger = logger;
            _escolaEF = escolaEF;
        }

        [BindProperty(SupportsGet = true)]
        public string? Pesquisa { get; set; }  // Recebe a pesquisa de t�tulo ou autor

        public List<ObraDto> Obras { get; set; } = new List<ObraDto>();

        public void OnGet()
        {
            try
            {
                // Busca obras com base na pesquisa (t�tulo ou autor)
                Obras = _escolaEF.MostrarTodasObras(pesquisa: Pesquisa);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar obras: {ex.Message}");
            }
        }

        public IActionResult OnGetNucleosDisponiveis(int obraId)
        {
            try
            {
                var nucleos = _escolaEF.ObterNucleosDisponiveisPorObra(obraId);
                return new JsonResult(nucleos);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
        }


        public IActionResult OnPostRequisitar([FromBody] RequisicaoDto requisicao)
        {
            try
            {
                // Obt�m o userID da sess�o
                int? userId = HttpContext.Session.GetInt32("userID");

                if (!userId.HasValue || userId <= 0)
                {
                    return new JsonResult(new { error = "Usu�rio n�o autenticado." });
                }

                _logger.LogInformation($"Usu�rio {userId.Value} requisitando obra {requisicao.RequisitarObraId} para n�cleo {requisicao.RequisitarNucleoId}");

                // Chama o m�todo diretamente com os valores inteiros
                var dataDevolucao = _escolaEF.FazerRequisicaoLivro(userId.Value, requisicao.RequisitarObraId, requisicao.RequisitarNucleoId);

                if (dataDevolucao.HasValue)
                {
                    return new JsonResult(new { dataDevolucao = dataDevolucao.Value.ToString("dd/MM/yyyy") });
                }
                else
                {
                    return new JsonResult(new { error = "Voc� tem 4 requisi��es ativas." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao requisitar obra: {ex.Message}");
                return new JsonResult(new { error = "Erro interno ao processar a requisi��o." });
            }
        }


    }

    public class RequisicaoDto
    {
        public int RequisitarObraId { get; set; }
        public int RequisitarNucleoId { get; set; }
    }

}
