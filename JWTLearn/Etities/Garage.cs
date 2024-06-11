namespace Etities;

public class Garage : EntityBase
{
    public string Name { get; set; }
    public string Address { get; set; }
    public List<Voiture> Voitures { get; set; } = [];
}
