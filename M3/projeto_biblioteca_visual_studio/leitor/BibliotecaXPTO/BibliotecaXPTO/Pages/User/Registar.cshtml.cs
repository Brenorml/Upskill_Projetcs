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
        //verificação de números e caracteres especiais no nome
        [RegularExpression(@"^(?!.*\s{2,})[A-Za-zÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "O nome deve conter apenas letras e um espaço entre palavras.")]
        public string Nome { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Date)]
        //Limitação de intervalo de ano para a data de aniversário
        [Range(typeof(DateOnly), "1900-01-01", "2099-12-31", ErrorMessage = "Ano inválido. Deve ser maior que 1900.")]
        public DateOnly DataNascimento { get; set; }

        [BindProperty]
        [Required]
        //[EmailAddress]
        //validação do email
        [EmailAddress(ErrorMessage = "O email inserido não é válido.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
    ErrorMessage = "O email deve conter '@' e um domínio válido, ex: exemplo@site.com.")]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        //Não aceitar números menores ou iguais a zero
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "O telefone deve conter apenas números maiores que zero.")]
        public string Telefone { get; set; }

        [BindProperty]
        [Required]
        //Não aceitar espaços
        [RegularExpression(@"^\S+$", ErrorMessage = "O nome de usuário não pode conter espaços.")]
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

            MensagemRetorno = "Registo efetuado. Faça login"; ;
        }


    }
}
