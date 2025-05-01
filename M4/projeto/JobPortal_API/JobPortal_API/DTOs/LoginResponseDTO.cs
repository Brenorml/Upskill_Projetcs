using JobPortal_API.Models;

namespace JobPortal_API.DTOs
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
    }
}
