namespace Auth.Jwt.RoleBased;

public class JwtTokenSettings
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string SigningKey { get; set; } = null!;
    public int TokenTimeoutMinutes { get; set; }
}