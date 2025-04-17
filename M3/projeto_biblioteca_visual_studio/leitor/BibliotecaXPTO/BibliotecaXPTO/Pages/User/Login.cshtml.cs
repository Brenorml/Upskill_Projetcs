using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using EscolaXPTO_EF;

namespace BibliotecaXPTO.Pages.User
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        private readonly EscolaEF _authService;

        public LoginModel(EscolaEF authService)
        {
            _authService = authService;
        }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            string mensagem = "";

            bool autenticaçao = _authService.AutenticarLeitor(Username, Password, out mensagem);

            if (autenticaçao)
            {

                int userId = _authService.ObterUserIdPorUsername(Username);


                HttpContext.Session.SetInt32("userID", userId);


                return RedirectToPage("/User/Inicial");
            }
            else
            {
                ErrorMessage = mensagem;
            }

            return Page();
        }
    }
}
