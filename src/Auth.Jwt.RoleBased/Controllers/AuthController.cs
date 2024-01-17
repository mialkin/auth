using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Jwt.RoleBased.Controllers.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Auth.Jwt.RoleBased.Controllers;

[ApiController]
[Authorize] // Just require ANY authentication
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
    public IActionResult Authenticate(AuthenticateUserRequest userRequest)
    {
        // Fetch user with roles from database
        var claims = new List<Claim>();
        // Feel claims specific to user roles fetched from database
        claims.Add(new Claim(ClaimTypes.Role, "User"));
        claims.Add(new Claim(ClaimTypes.Role, "Administrator"));

        var token = JwtHelper.GetJwtToken(
            userRequest.Username,
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