using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using EscolaXPTO_EF;
using static EscolaXPTO_EF.Dto.Dtos;

namespace BibliotecaXPTO.Pages.User
{
    [IgnoreAntiforgeryToken] // Ignora validação antifalsificação
    public class PerfilModel : PageModel
    {
        private readonly EscolaEF _escolaService;

        [BindProperty]
        public PerfilDTO Perfil { get; set; }
        public bool ModoEdicao { get; set; }

        public PerfilModel(EscolaEF escolaService)
        {
            _escolaService = escolaService;
        }

        public void OnGet()
        {
            CarregarDadosPerfil();
        }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetInt32("userID");
            if (!userId.HasValue) return RedirectToPage("/User/Login");

            if (!ModelState.IsValid)
            {
                // Log de erros de validação
                foreach (var entry in ModelState)
                {
                    if (entry.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"Erro no campo '{entry.Key}': {entry.Value.Errors[0].ErrorMessage}");
                    }
                }
                CarregarDadosPerfil();
                TempData["Erro"] = "Dados inválidos.";
                return Page();
            }

            bool atualizado = _escolaService.AtualizarPerfilUsuario(userId.Value, Perfil);

            if (atualizado)
                TempData["Sucesso"] = "Dados salvos com sucesso!";
            else
                TempData["Erro"] = "Falha ao salvar. Tente novamente.";

            return RedirectToPage();
        }

        private void CarregarDadosPerfil()
        {
            var userId = HttpContext.Session.GetInt32("userID");
            if (userId.HasValue)
            {
                Perfil = _escolaService.ObterPerfilUsuario(userId.Value);
            }
        }

        public IActionResult OnPostCancelarInscricao(int userId)
        {
            Console.WriteLine($"Recebido userId: {userId}");

            if (userId == 0)
            {
                return BadRequest("ID de usuário inválido.");
            }

            var resultado = _escolaService.CancelarInscricao(userId);

            if (resultado != null && resultado.Any())
            {
                return new JsonResult(new { success = true });
            }
            else
            {
                return new JsonResult(new { success = false, message = "Falha ao cancelar inscrição." });
            }
        }

    }
}
