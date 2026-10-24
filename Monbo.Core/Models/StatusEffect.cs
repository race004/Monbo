namespace Monbo.Core.Models;

public class StatusEffect
{
    public string Name { get; set; } = string.Empty;
    public MonsterType Element { get; set; }
    public int Duration { get; set; } // Koliko poteza ostaje
    public bool IsStackable { get; set; } // Da li može da se sabira (npr. Bane)

    // Za štete po potezu (Burn, Poison, Bane)
    public double DamagePercentPerTurn { get; set; } = 0;

    // Za smanjenje statistika (Rust, Blind, Soak)
    public double AtkModifier { get; set; } = 0;
    public double SpeedModifier { get; set; } = 0;
    public double AccuracyModifier { get; set; } = 0;

    // Za preskakanje poteza (Stun, Freeze)
    public bool SkipTurn { get; set; } = false;

    // Za Confusion
    public bool IsConfusion { get; set; } = false;

    public StatusEffect(string name, MonsterType element, int duration)
    {
        Name = name;
        Element = element;
        Duration = duration;
    }
}