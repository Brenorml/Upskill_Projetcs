using System.ComponentModel.DataAnnotations;
using EscolaXPTO_EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BibliotecaXPTO.Pages.User
{
    public class RegistarModel : PageModel
    {
        [BindProperty]
        [Required]
        //verifica��o de n�meros e caracteres especiais no nome
        [RegularExpression(@"^(?!.*\s{2,})[A-Za-z�-��-��-�\s]+$", ErrorMessage = "O nome deve conter apenas letras e um espa�o entre palavras.")]
        public string Nome { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Date)]
        //Limita��o de intervalo de ano para a data de anivers�rio
        [Range(typeof(DateOnly), "1900-01-01", "2099-12-31", ErrorMessage = "Ano inv�lido. Deve ser maior que 1900.")]
        public DateOnly DataNascimento { get; set; }

        [BindProperty]
        [Required]
        //[EmailAddress]
        //valida��o do email
        [EmailAddress(ErrorMessage = "O email inserido n�o � v�lido.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
    ErrorMessage = "O email deve conter '@' e um dom�nio v�lido, ex: exemplo@site.com.")]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        //N�o aceitar n�meros menores ou iguais a zero
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "O telefone deve conter apenas n�meros maiores que zero.")]
        public string Telefone { get; set; }

        [BindProperty]
        [Required]
        //N�o aceitar espa�os
        [RegularExpression(@"^\S+$", ErrorMessage = "O nome de usu�rio n�o pode conter espa�os.")]
        public string Username { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string PalavraPasse { get; set; }

        public string MensagemRetorno { get; set; }

        private readonly EscolaEF _bibliotecaService;

        public RegistarModel(EscolaEF bibliotecaService)
        {
            _bibliotecaService = bibliotecaService;
        }        

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                MensagemRetorno = "Preencha todos os campos corretamente.";
                return;
            }

            bool sucesso = _bibliotecaService.User_RegistrarNovoLeitor(Nome, DataNascimento, Email, Telefone, Username, PalavraPasse, out string mensagem);

            MensagemRetorno = "Registo efetuado. Fa�a login"; ;
        }


    }
}
