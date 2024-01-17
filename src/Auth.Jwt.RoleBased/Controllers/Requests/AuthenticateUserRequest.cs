namespace Auth.Jwt.RoleBased.Controllers.Requests;

public record AuthenticateUserRequest(string Username, string Password);