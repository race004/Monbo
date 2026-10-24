using Monbo.Core.Models;

namespace Monbo.Core.Data;

public static class TypeChart
{
    public static double GetMultiplier(MonsterType attackType, List<MonsterType> defenderTypes)
    {
        double totalMultiplier = 1.0;
        foreach (var defType in defenderTypes)
        {
            totalMultiplier *= GetSingleMultiplier(attackType, defType);
        }
        return totalMultiplier;
    }

    private static double GetSingleMultiplier(MonsterType attackType, MonsterType defenderType)
    {
        return (attackType, defenderType) switch
        {
            // NORMAL
            (MonsterType.Normal, MonsterType.Rock) => 0.5,
            (MonsterType.Normal, MonsterType.Steel) => 0.5,

            // FIRE
            (MonsterType.Fire, MonsterType.Grass) => 2.0,
            (MonsterType.Fire, MonsterType.Ice) => 2.0,
            (MonsterType.Fire, MonsterType.Steel) => 2.0,
            (MonsterType.Fire, MonsterType.Fire) => 0.5,
            (MonsterType.Fire, MonsterType.Water) => 0.5,
            (MonsterType.Fire, MonsterType.Rock) => 0.5,

            // WATER
            (MonsterType.Water, MonsterType.Fire) => 2.0,
            (MonsterType.Water, MonsterType.Rock) => 2.0,
            (MonsterType.Water, MonsterType.Water) => 0.5,
            (MonsterType.Water, MonsterType.Grass) => 0.5,
            (MonsterType.Water, MonsterType.Steel) => 0.5,

            // GRASS
            (MonsterType.Grass, MonsterType.Water) => 2.0,
            (MonsterType.Grass, MonsterType.Rock) => 2.0,
            (MonsterType.Grass, MonsterType.Fire) => 0.5,
            (MonsterType.Grass, MonsterType.Grass) => 0.5,
            (MonsterType.Grass, MonsterType.Flying) => 0.5,
            (MonsterType.Grass, MonsterType.Steel) => 0.5,

            // ELECTRIC
            (MonsterType.Electric, MonsterType.Water) => 2.0,
            (MonsterType.Electric, MonsterType.Flying) => 2.0,
            (MonsterType.Electric, MonsterType.Grass) => 0.5,
            (MonsterType.Electric, MonsterType.Electric) => 0.5,

            // ROCK
            (MonsterType.Rock, MonsterType.Fire) => 2.0,
            (MonsterType.Rock, MonsterType.Flying) => 2.0,
            (MonsterType.Rock, MonsterType.Ice) => 2.0,
            (MonsterType.Rock, MonsterType.Rock) => 0.5,
            (MonsterType.Rock, MonsterType.Steel) => 0.5,

            // FLYING
            (MonsterType.Flying, MonsterType.Grass) => 2.0,
            (MonsterType.Flying, MonsterType.Flying) => 1.0, // Neutralno
            (MonsterType.Flying, MonsterType.Electric) => 0.5,
            (MonsterType.Flying, MonsterType.Rock) => 0.5,
            (MonsterType.Flying, MonsterType.Steel) => 0.5,

            // PSYCHIC
            (MonsterType.Psychic, MonsterType.Psychic) => 0.5,
            (MonsterType.Psychic, MonsterType.Dark) => 0.0, // Potpuni imun (nema efekta)
            (MonsterType.Psychic, MonsterType.Steel) => 0.5,

            // ICE
            (MonsterType.Ice, MonsterType.Grass) => 2.0,
            (MonsterType.Ice, MonsterType.Flying) => 2.0,
            (MonsterType.Ice, MonsterType.Fire) => 0.5,
            (MonsterType.Ice, MonsterType.Water) => 0.5,
            (MonsterType.Ice, MonsterType.Ice) => 0.5,
            (MonsterType.Ice, MonsterType.Steel) => 0.5,

            // STEEL
            (MonsterType.Steel, MonsterType.Ice) => 2.0,
            (MonsterType.Steel, MonsterType.Rock) => 2.0,
            (MonsterType.Steel, MonsterType.Fire) => 0.5,
            (MonsterType.Steel, MonsterType.Water) => 0.5,
            (MonsterType.Steel, MonsterType.Electric) => 0.5,
            (MonsterType.Steel, MonsterType.Steel) => 0.5,

            // DARK
            (MonsterType.Dark, MonsterType.Psychic) => 2.0,
            (MonsterType.Dark, MonsterType.Dark) => 0.5,
            (MonsterType.Dark, MonsterType.Light) => 0.5,

            // LIGHT
            (MonsterType.Light, MonsterType.Dark) => 2.0,
            (MonsterType.Light, MonsterType.Light) => 0.5,
            (MonsterType.Light, MonsterType.Steel) => 0.5,

            // Svi ostali slučajevi su neutralni (1.0)
            _ => 1.0
        };
    }
}