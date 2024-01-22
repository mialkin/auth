public class User
{
    public User(string username)
    {
        Username = username;
    }

    public string Username { get; set; }
    
    public string? PasswordHash { get; set; }
    public List<UserClaim> Claims { get; set; } = new();
}