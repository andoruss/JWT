namespace Etities;

public class User : EntityBase
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; }
    public List<Garage> Garages { get; set; } = [];
    public List<Voiture> Voitures { get; set; } = [];
}
