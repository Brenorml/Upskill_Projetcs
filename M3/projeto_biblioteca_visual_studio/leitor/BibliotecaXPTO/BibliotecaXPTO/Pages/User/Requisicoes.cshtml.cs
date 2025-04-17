using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using static EscolaXPTO_EF.Dto.Dtos;
using EscolaXPTO_EF;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaXPTO.Pages.User
{
    public class RequisicoesModel : PageModel
    {
        private readonly EscolaEF _bibliotecaService;

        // Lista de requisi��es ativas do usu�rio
        public List<SituacaoLeitorDTO> Requisicoes { get; set; } = new List<SituacaoLeitorDTO>();

        // Filtro por n�cleo (opcional)
        [BindProperty(SupportsGet = true)]
        public string? NucleoFiltro { get; set; }

        public RequisicoesModel(EscolaEF bibliotecaService)
        {
            _bibliotecaService = bibliotecaService;
        }

        public void OnGet()
        {
            try
            {
                // Obt�m o ID do usu�rio da sess�o
                int? userId = HttpContext.Session.GetInt32("userID");

                if (userId.HasValue && userId > 0)
                {
                    // Busca as requisi��es ativas do usu�rio
                    Requisicoes = _bibliotecaService.User_VerificarSituacaoAtual(userId.Value);

                    // Aplica o filtro por n�cleo, se fornecido
                    if (!string.IsNullOrEmpty(NucleoFiltro))
                    {
                        Requisicoes = Requisicoes.Where(r => r.Nucleo == NucleoFiltro).ToList();
                    }
                }
                else
                {
                    // Se o usu�rio n�o estiver autenticado, define uma mensagem de erro
                    TempData["Erro"] = "Usu�rio n�o autenticado.";
                }
            }
            catch (Exception ex)
            {
                // Em caso de erro, define uma mensagem de erro
                TempData["Erro"] = $"Erro ao carregar requisi��es: {ex.Message}";
            }
        }

        public IActionResult OnPostDevolver(string requisicao)
        {
            try
            {
                // Obter o LeitorID da sess�o
                int? leitorID = HttpContext.Session.GetInt32("userID");

                if (leitorID == null || leitorID <= 0)
                {
                    TempData["Erro"] = "Usu�rio n�o autenticado.";
                    return RedirectToPage();
                }

                // Chamar o m�todo DevolverObra com o LeitorID da sess�o e o RequisicaoID da obra
                var resultadoDevolucao = _bibliotecaService.DevolverObra(leitorID.Value.ToString(), requisicao);

                if (resultadoDevolucao.Any())
                {
                    TempData["Sucesso_Devolve"] = "Obra devolvida com sucesso!";
                }
                else
                {
                    TempData["Erro_Devolve"] = "Erro ao devolver a obra.";
                }
            }
            catch (Exception ex)
            {
                TempData["Erro"] = $"Erro ao devolver a obra: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}