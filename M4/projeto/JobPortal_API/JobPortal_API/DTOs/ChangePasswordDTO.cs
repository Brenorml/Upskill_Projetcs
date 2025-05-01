namespace JobPortal_API.DTOs
{
    public class ChangePasswordDTO
    {
        public int IdCandidato { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
