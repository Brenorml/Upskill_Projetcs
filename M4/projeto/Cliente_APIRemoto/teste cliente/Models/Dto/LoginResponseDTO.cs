using teste_cliente.Models;
using teste_cliente.Models.Dto;

namespace JobPortal_API.DTOs
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
    }
}
