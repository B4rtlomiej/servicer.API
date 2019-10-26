namespace servicer.API.Dtos
{
    public class PasswordsForUserActivationDto
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
}