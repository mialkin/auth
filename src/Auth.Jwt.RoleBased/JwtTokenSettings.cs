namespace Auth.Jwt.RoleBased;

public class JwtTokenSettings
{
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string? SigningKey { get; set; }
}