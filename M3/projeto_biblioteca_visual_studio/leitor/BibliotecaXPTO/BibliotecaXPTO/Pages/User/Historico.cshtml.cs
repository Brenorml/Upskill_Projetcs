using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using static EscolaXPTO_EF.Dto.Dtos;
using EscolaXPTO_EF;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaXPTO.Pages.User
{
    public class HistoricoModel : PageModel
    {
        private readonly EscolaEF _bibliotecaService;

        public List<HistoricoRequisicaoDTO> Historico { get; set; }

        [BindProperty(SupportsGet = true)]
        public string NucleoFiltro { get; set; }

        [BindProperty(SupportsGet = true)]
        public string DataInicio { get; set; }

        [BindProperty(SupportsGet = true)]
        public string DataFim { get; set; }

        public HistoricoModel(EscolaEF bibliotecaService)
        {
            _bibliotecaService = bibliotecaService;
        }

        public void OnGet()
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("userID");
                if (userId.HasValue && userId > 0)
                {


                    // Chamar o método com os filtros
                    Historico = _bibliotecaService.ObterHistoricoRequisicoes(
                        userId.Value.ToString(),
                        NucleoFiltro,
                        DataInicio,
                        DataFim
                    );
                }
                else
                {
                    TempData["Erro"] = "Usuário não autenticado.";
                }
            }
            catch (Exception ex)
            {
                TempData["Erro"] = $"Erro ao carregar histórico: {ex.Message}";
                Historico = new List<HistoricoRequisicaoDTO>();
            }
        }
    }
}