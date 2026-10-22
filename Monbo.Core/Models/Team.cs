namespace Monbo.Core.Models;

public class Team
{
    public string Name { get; set; } = "Igrač";
    public List<Monster> Members { get; set; } = new();

    public Team(string name)
    {
        Name = name;
    }

    public Team() { }

    public bool HasAlive => Members.Any(m => m.IsAlive);
}