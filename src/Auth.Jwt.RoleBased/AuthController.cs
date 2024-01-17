using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Auth.Jwt.RoleBased;

[ApiController]
[Authorize]
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
        // Fetch user from database by its username and password
        var claims = new List<Claim>();
        // Feel claims specific to user fetched from database 

        var token = JwtHelper.GetJwtToken(
            request.Username,
            _jwtTokenSettings.SigningKey,
            _jwtTokenSettings.Issuer,
            _jwtTokenSettings.Audience,
            expiration: TimeSpan.FromMinutes(_jwtTokenSettings.TokenTimeoutMinutes),
            claims.ToArray());

        var result = new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expires = token.ValidTo
        };

        return Ok(result);
    }
}