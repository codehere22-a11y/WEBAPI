public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public User() { }  // needed for EF Core, which constructs objects then sets properties

    public User(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }
}