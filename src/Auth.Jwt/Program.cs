using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var claims = new List<Claim>
{
    new(type: ClaimTypes.Name, value: "Bob"),
    new(type: ClaimTypes.Role, value: "admin")
};

const string symmetricKeyText = "ksdfiusb4kjbiub8zdgf8sudfi7h4387r5ghi9JJhdkl9JKm,dpPl,349&Hnjkl:JOIHoisdofisdf";

var symmetricKey = new SymmetricSecurityKey(key: Encoding.UTF8.GetBytes(symmetricKeyText));

var signingCredentials = new SigningCredentials(
    key: symmetricKey,
    algorithm: SecurityAlgorithms.HmacSha512Signature);

var tokenHandler = new JwtSecurityTokenHandler();

// var token = new JwtSecurityToken();
// This results in following JWT token: eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.e30.
// A header: { "alg": "none", "typ": "JWT" }; a payload: {}; no signature

var token = new JwtSecurityToken(
    claims: claims,
    expires: DateTime.UtcNow.AddDays(1),
    signingCredentials: signingCredentials);

string jwt = tokenHandler.WriteToken(token);

Console.WriteLine(jwt);

// This results in the following JWT token: eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQm9iIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiYWRtaW4iLCJleHAiOjE3MDU1MDg0Njl9.7zX5BkhaNIk8UtcvOsya6gBZdUXOXOlNP4OZF4nYPSG2flKfHlJ2IHSrz4WyC6rFQGJgUfS_m3w3xDoetuSzWA

// https://www.base64decode.org/

// Header: eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9
/*
 * {
     "alg": "HS512",
     "typ": "JWT"
   }
*/

// Payload: eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQm9iIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiYWRtaW4iLCJleHAiOjE3MDU1MDg0Njl9
/*
 * {
     "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "Bob",
     "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "admin",
     "exp": 1705508469
   }
 */
 
 // Signature: 7zX5BkhaNIk8UtcvOsya6gBZdUXOXOlNP4OZF4nYPSG2flKfHlJ2IHSrz4WyC6rFQGJgUfS_m3w3xDoetuSzWA