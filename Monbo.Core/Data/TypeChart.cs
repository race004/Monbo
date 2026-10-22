using Monbo.Core.Models;

namespace Monbo.Core.Data;

public static class TypeChart
{
    // Vraća množač štete (2.0, 0.5 ili 1.0)
    public static double GetMultiplier(MonsterType attackType, List<MonsterType> defenderTypes)
    {
        double multiplier = 1.0;

        foreach (var defType in defenderTypes)
        {
            multiplier *= GetSingleMultiplier(attackType, defType);
        }

        return multiplier;
    }

    private static double GetSingleMultiplier(MonsterType attackType, MonsterType defenderType)
    {
        // Ovde prevodimo našu tabelu iz definicije u kod
        return (attackType, defenderType) switch
        {
            // Fire
            (MonsterType.Fire, MonsterType.Grass) => 2.0,
            (MonsterType.Fire, MonsterType.Water) => 0.5,
            (MonsterType.Fire, MonsterType.Rock) => 0.5,

            // Water
            (MonsterType.Water, MonsterType.Fire) => 2.0,
            (MonsterType.Water, MonsterType.Rock) => 2.0,
            (MonsterType.Water, MonsterType.Water) => 0.5,
            (MonsterType.Water, MonsterType.Grass) => 0.5,

            // Grass
            (MonsterType.Grass, MonsterType.Water) => 2.0,
            (MonsterType.Grass, MonsterType.Rock) => 2.0,
            (MonsterType.Grass, MonsterType.Fire) => 0.5,
            (MonsterType.Grass, MonsterType.Grass) => 0.5,
            (MonsterType.Grass, MonsterType.Flying) => 0.5,

            // Electric
            (MonsterType.Electric, MonsterType.Water) => 2.0,
            (MonsterType.Electric, MonsterType.Flying) => 2.0,
            (MonsterType.Electric, MonsterType.Grass) => 0.5,
            (MonsterType.Electric, MonsterType.Electric) => 0.5,

            // Rock
            (MonsterType.Rock, MonsterType.Fire) => 2.0,
            (MonsterType.Rock, MonsterType.Flying) => 2.0,
            (MonsterType.Rock, MonsterType.Rock) => 0.5,

            // Flying
            (MonsterType.Flying, MonsterType.Grass) => 2.0,
            (MonsterType.Flying, MonsterType.Electric) => 0.5,
            (MonsterType.Flying, MonsterType.Rock) => 0.5,

            // Psychic
            (MonsterType.Psychic, MonsterType.Psychic) => 0.5,

            // Normal
            (MonsterType.Normal, MonsterType.Rock) => 0.5,

            // Svi ostali slučajevi su 1.0 (neutralno)
            _ => 1.0
        };
    }
}