using Microsoft.AspNetCore.DataProtection;

namespace Auth.MinimalApi.Authentication;

// public class AuthService
// {
//     private readonly IDataProtectionProvider _provider;
//     private readonly IHttpContextAccessor _accessor;
//
//     public AuthService(IDataProtectionProvider provider, IHttpContextAccessor accessor)
//     {
//         _provider = provider;
//         _accessor = accessor;
//     }
//
//     public void SignIn()
//     {
//         var protector = _provider.CreateProtector("auth-cookie");
//
//         if (_accessor.HttpContext != null)
//             _accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:aleksei")}";
//     }
// }