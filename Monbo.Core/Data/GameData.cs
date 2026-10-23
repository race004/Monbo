using Monbo.Core.Models;

namespace Monbo.Core.Data;

public static class GameData
{
    public static List<Monster> MonsterTemplates { get; private set; } = new();

    public static void Initialize()
    {
        // 1. Definišemo SVE napade u igri
        var allAttacks = new List<Attack>
        {
            // Normal
            new Attack("Tackle", MonsterType.Normal, 35, 100, 1),
            new Attack("Quick Attack", MonsterType.Normal, 40, 100, 5),
            new Attack("Hyper Beam", MonsterType.Normal, 120, 90, 20),

            // Fire
            new Attack("Ember", MonsterType.Fire, 40, 100, 1),
            new Attack("Flamethrower", MonsterType.Fire, 80, 95, 5),
            new Attack("Fire Blast", MonsterType.Fire, 110, 85, 15),
            new Attack("Inferno", MonsterType.Fire, 120, 80, 30),

            // Water
            new Attack("Water Gun", MonsterType.Water, 40, 100, 1),
            new Attack("Bubble Beam", MonsterType.Water, 65, 100, 5),
            new Attack("Hydro Pump", MonsterType.Water, 110, 80, 15),
            new Attack("Surf", MonsterType.Water, 90, 100, 30),

            // Grass
            new Attack("Vine Whip", MonsterType.Grass, 45, 100, 1),
            new Attack("Razor Leaf", MonsterType.Grass, 55, 95, 5),
            new Attack("Solar Beam", MonsterType.Grass, 120, 85, 15),
            new Attack("Petal Dance", MonsterType.Grass, 90, 100, 30),

            // Electric
            new Attack("Thunder Shock", MonsterType.Electric, 40, 100, 1),
            new Attack("Spark", MonsterType.Electric, 65, 100, 5),
            new Attack("Thunderbolt", MonsterType.Electric, 90, 100, 15),
            new Attack("Thunder", MonsterType.Electric, 110, 70, 30),

            // Rock
            new Attack("Rock Throw", MonsterType.Rock, 50, 90, 1),
            new Attack("Rock Slide", MonsterType.Rock, 75, 90, 5),
            new Attack("Stone Edge", MonsterType.Rock, 100, 80, 15),
            new Attack("Earthquake", MonsterType.Rock, 100, 100, 30),

            // Flying
            new Attack("Gust", MonsterType.Flying, 40, 100, 1),
            new Attack("Wing Attack", MonsterType.Flying, 60, 100, 5),
            new Attack("Air Slash", MonsterType.Flying, 75, 95, 15),
            new Attack("Hurricane", MonsterType.Flying, 110, 70, 30),

            // Psychic
            new Attack("Confusion", MonsterType.Psychic, 50, 100, 1),
            new Attack("Psybeam", MonsterType.Psychic, 65, 100, 5),
            new Attack("Psychic", MonsterType.Psychic, 90, 100, 15),
            new Attack("Future Sight", MonsterType.Psychic, 120, 90, 30),

            // Heal napadi
            new Attack("Recover", 50, 100, HealTarget.Self, 5),
            new Attack("Synthesis", 30, 100, HealTarget.SelfAndAlly, 10),
            new Attack("Heal Bell", 40, 100, HealTarget.Ally, 15),
            new Attack("Moonlight", 50, 100, HealTarget.SelfAndAlly, 40)
        };

        // 2. Pravimo šablone čudovišta
        // --- 7 COMMON (Max level 20) ---
        MonsterTemplates.Add(CreateMonster("Flameon", new[] { MonsterType.Fire }, Rarity.Common, 60, 15, allAttacks, "Ember", "Tackle", "Quick Attack", "Flamethrower", "Recover", "Fire Blast"));
        MonsterTemplates.Add(CreateMonster("Aquari", new[] { MonsterType.Water }, Rarity.Common, 65, 12, allAttacks, "Water Gun", "Tackle", "Quick Attack", "Bubble Beam", "Recover", "Hydro Pump"));
        MonsterTemplates.Add(CreateMonster("Leafling", new[] { MonsterType.Grass }, Rarity.Common, 62, 14, allAttacks, "Vine Whip", "Tackle", "Quick Attack", "Razor Leaf", "Recover", "Solar Beam"));
        MonsterTemplates.Add(CreateMonster("Sparky", new[] { MonsterType.Electric }, Rarity.Common, 58, 16, allAttacks, "Thunder Shock", "Tackle", "Quick Attack", "Spark", "Recover", "Thunderbolt"));
        MonsterTemplates.Add(CreateMonster("Pebble", new[] { MonsterType.Rock }, Rarity.Common, 70, 11, allAttacks, "Rock Throw", "Tackle", "Quick Attack", "Rock Slide", "Recover", "Stone Edge"));
        MonsterTemplates.Add(CreateMonster("Zephyr", new[] { MonsterType.Flying }, Rarity.Common, 55, 17, allAttacks, "Gust", "Tackle", "Quick Attack", "Wing Attack", "Recover", "Air Slash"));
        MonsterTemplates.Add(CreateMonster("Mystic", new[] { MonsterType.Psychic }, Rarity.Common, 60, 15, allAttacks, "Confusion", "Tackle", "Quick Attack", "Psybeam", "Recover", "Psychic"));

        // --- 5 UNCOMMON (Max level 30) ---
        MonsterTemplates.Add(CreateMonster("Blaze", new[] { MonsterType.Fire }, Rarity.Uncommon, 75, 18, allAttacks, "Ember", "Tackle", "Quick Attack", "Flamethrower", "Recover", "Fire Blast", "Inferno"));
        MonsterTemplates.Add(CreateMonster("Tide", new[] { MonsterType.Water }, Rarity.Uncommon, 80, 15, allAttacks, "Water Gun", "Tackle", "Quick Attack", "Bubble Beam", "Recover", "Hydro Pump", "Surf"));
        MonsterTemplates.Add(CreateMonster("Thorn", new[] { MonsterType.Grass }, Rarity.Uncommon, 78, 16, allAttacks, "Vine Whip", "Tackle", "Quick Attack", "Razor Leaf", "Recover", "Solar Beam", "Petal Dance"));
        MonsterTemplates.Add(CreateMonster("Volt", new[] { MonsterType.Electric }, Rarity.Uncommon, 70, 20, allAttacks, "Thunder Shock", "Tackle", "Quick Attack", "Spark", "Recover", "Thunderbolt", "Thunder"));
        MonsterTemplates.Add(CreateMonster("Gale", new[] { MonsterType.Flying, MonsterType.Normal }, Rarity.Uncommon, 68, 19, allAttacks, "Gust", "Tackle", "Quick Attack", "Wing Attack", "Recover", "Air Slash", "Hurricane"));

        // --- 3 RARE (Max level 40) ---
        MonsterTemplates.Add(CreateMonster("Inferno", new[] { MonsterType.Fire, MonsterType.Rock }, Rarity.Rare, 90, 22, allAttacks, "Ember", "Tackle", "Quick Attack", "Flamethrower", "Recover", "Fire Blast", "Stone Edge", "Inferno"));
        MonsterTemplates.Add(CreateMonster("Glacier", new[] { MonsterType.Water, MonsterType.Flying }, Rarity.Rare, 95, 19, allAttacks, "Water Gun", "Tackle", "Quick Attack", "Bubble Beam", "Recover", "Hydro Pump", "Air Slash", "Hurricane"));
        MonsterTemplates.Add(CreateMonster("Terra", new[] { MonsterType.Grass, MonsterType.Rock }, Rarity.Rare, 100, 20, allAttacks, "Vine Whip", "Tackle", "Quick Attack", "Razor Leaf", "Recover", "Solar Beam", "Stone Edge", "Earthquake"));

        // --- 1 EPIC (Max level 50) ---
        MonsterTemplates.Add(CreateMonster("Psykid", new[] { MonsterType.Psychic }, Rarity.Epic, 110, 25, allAttacks, "Confusion", "Tackle", "Quick Attack", "Psybeam", "Recover", "Psychic", "Future Sight", "Heal Bell", "Moonlight"));
    }

    // Pomoćna metoda za lakše kreiranje
    private static Monster CreateMonster(string name, MonsterType[] types, Rarity rarity, int hp, int atk, List<Attack> allAttacks, params string[] moveNames)
    {
        var m = new Monster(name, types.ToList(), rarity, hp, atk);
        m.AvailableMoves = allAttacks.Where(a => moveNames.Contains(a.Name)).ToList();
        return m;
    }
}