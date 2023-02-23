namespace OutOfTimePrototype.Dto
{
    public class LoginDto
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class AuthenticateSuccessDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class RefreshDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
