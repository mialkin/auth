using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Auth.Jwt.RoleBased;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenSettings _jwtTokenSettings;

    public AuthController(IOptions<JwtTokenSettings> jwtTokenOptions)
    {
        _jwtTokenSettings = jwtTokenOptions.Value;
    }
    
    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate(LoginUserRequest request)
    {
        var claim = new Claim(type: ClaimTypes.Role, value: "admin");
        var claimIdentity = new ClaimsIdentity(claims: new[] { claim }, authenticationType: "login");
        var claimsPrincipal = new ClaimsPrincipal(identities: new[] { claimIdentity });

        var claims = new List<Claim>();
        
        var token = JwtHelper.GetJwtToken(
            request.Username,
            _jwtTokenSettings.SigningKey,
            _jwtTokenSettings.Issuer,
            _jwtTokenSettings.Audience,
            TimeSpan.FromMinutes(_jwtTokenSettings.TokenTimeoutMinutes),
            claims.ToArray());

        var result = new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expires = token.ValidTo
        };

        return Ok(result);
    }
}