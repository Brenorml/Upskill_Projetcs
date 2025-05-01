namespace JobPortal_API.DTOs
{
    public class ChangePasswordEmpresaDTO
    {
        public int IdEmpresa { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
