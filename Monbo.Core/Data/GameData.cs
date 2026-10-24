using Monbo.Core.Models;

namespace Monbo.Core.Data;

public static class GameData
{
    public static List<Monster> MonsterTemplates { get; private set; } = new();

    public static void Initialize()
    {
        var allAttacks = new List<Attack>
        {
            // Normal
            new Attack("Tackle", MonsterType.Normal, 35, 100, 1, 5),
            new Attack("Scratch", MonsterType.Normal, 40, 100, 1, 5),
            new Attack("Quick Attack", MonsterType.Normal, 40, 100, 5, 10),
            new Attack("Body Slam", MonsterType.Normal, 85, 100, 15, 15),
            new Attack("Hyper Beam", MonsterType.Normal, 120, 90, 20, 30),

            // Fire
            new Attack("Ember", MonsterType.Fire, 40, 100, 1, 5),
            new Attack("Flamethrower", MonsterType.Fire, 80, 95, 5, 15),
            new Attack("Flame Burst", MonsterType.Fire, 50, 90, 15, 20, isArea: true),
            new Attack("Inferno", MonsterType.Fire, 120, 80, 20, 30),

            // Water
            new Attack("Water Gun", MonsterType.Water, 40, 100, 1, 5),
            new Attack("Bubble Beam", MonsterType.Water, 65, 100, 5, 15),
            new Attack("Surf", MonsterType.Water, 60, 95, 15, 20, isArea: true),
            new Attack("Hydro Pump", MonsterType.Water, 110, 80, 20, 25),

            // Grass
            new Attack("Vine Whip", MonsterType.Grass, 45, 100, 1, 5),
            new Attack("Razor Leaf", MonsterType.Grass, 55, 95, 5, 15),
            new Attack("Petal Dance", MonsterType.Grass, 55, 100, 15, 20, isArea: true),
            new Attack("Solar Beam", MonsterType.Grass, 120, 85, 20, 25),

            // Electric
            new Attack("Thunder Shock", MonsterType.Electric, 40, 100, 1, 5),
            new Attack("Spark", MonsterType.Electric, 65, 100, 5, 15),
            new Attack("Thunderbolt", MonsterType.Electric, 90, 100, 15, 20),
            new Attack("Thunder", MonsterType.Electric, 80, 70, 20, 30, isArea: true),

            // Rock
            new Attack("Rock Throw", MonsterType.Rock, 50, 90, 1, 5),
            new Attack("Rock Slide", MonsterType.Rock, 75, 90, 5, 15),
            new Attack("Stone Edge", MonsterType.Rock, 100, 80, 15, 20),
            new Attack("Earthquake", MonsterType.Rock, 90, 100, 20, 30, isArea: true),

            // Flying
            new Attack("Gust", MonsterType.Flying, 40, 100, 1, 5),
            new Attack("Wing Attack", MonsterType.Flying, 60, 100, 5, 10),
            new Attack("Air Slash", MonsterType.Flying, 75, 95, 15, 15),
            new Attack("Hurricane", MonsterType.Flying, 90, 70, 20, 25, isArea: true),

            // Psychic
            new Attack("Confusion", MonsterType.Psychic, 50, 100, 1, 5),
            new Attack("Psybeam", MonsterType.Psychic, 65, 100, 5, 10),
            new Attack("Psychic", MonsterType.Psychic, 90, 100, 15, 20),
            new Attack("Future Sight", MonsterType.Psychic, 110, 90, 20, 30, isArea: true),

            // Ice
            new Attack("Ice Shard", MonsterType.Ice, 40, 100, 1, 5),
            new Attack("Frost Breath", MonsterType.Ice, 65, 90, 5, 15),
            new Attack("Ice Beam", MonsterType.Ice, 90, 100, 15, 20),
            new Attack("Blizzard", MonsterType.Ice, 100, 70, 20, 30, isArea: true),

            // Steel
            new Attack("Metal Claw", MonsterType.Steel, 45, 95, 1, 5),
            new Attack("Iron Head", MonsterType.Steel, 70, 90, 5, 15),
            new Attack("Flash Cannon", MonsterType.Steel, 90, 100, 15, 20),
            new Attack("Meteor Mash", MonsterType.Steel, 110, 85, 20, 30, isArea: true),

            // Dark
            new Attack("Bite", MonsterType.Dark, 45, 100, 1, 5),
            new Attack("Dark Pulse", MonsterType.Dark, 70, 100, 5, 15),
            new Attack("Crunch", MonsterType.Dark, 85, 95, 15, 20),
            new Attack("Night Slash", MonsterType.Dark, 100, 90, 20, 25),

            // Light
            new Attack("Flash", MonsterType.Light, 40, 100, 1, 5),
            new Attack("Light Beam", MonsterType.Light, 65, 100, 5, 15),
            new Attack("Dazzling Gleam", MonsterType.Light, 80, 100, 15, 20),
            new Attack("Solar Flare", MonsterType.Light, 90, 90, 20, 25, isArea: true),

            // Heal napadi
            new Attack("Recover", 50, 100, HealTarget.Self, 10, 20),
            new Attack("Synthesis", 30, 100, HealTarget.SelfAndAlly, 10, 25),
            new Attack("Shield", 0, 100, HealTarget.Self, 10, 15) // Privremeno, dok ne dodamo štit
        };

        // 12 COMMON čudovišta (sa Speed i Staminom)
        MonsterTemplates.Add(CreateMonster("Flameon", new[] { MonsterType.Fire }, Rarity.Common, 60, 15, 12, 50, allAttacks, "Tackle", "Ember", "Flamethrower", "Quick Attack", "Flame Burst", "Inferno"));
        MonsterTemplates.Add(CreateMonster("Aquari", new[] { MonsterType.Water }, Rarity.Common, 65, 12, 10, 50, allAttacks, "Tackle", "Water Gun", "Bubble Beam", "Recover", "Surf", "Hydro Pump"));
        MonsterTemplates.Add(CreateMonster("Leafling", new[] { MonsterType.Grass }, Rarity.Common, 62, 14, 11, 50, allAttacks, "Tackle", "Vine Whip", "Razor Leaf", "Synthesis", "Petal Dance", "Solar Beam"));
        MonsterTemplates.Add(CreateMonster("Sparky", new[] { MonsterType.Electric }, Rarity.Common, 58, 16, 15, 50, allAttacks, "Tackle", "Thunder Shock", "Spark", "Quick Attack", "Thunderbolt", "Thunder"));
        MonsterTemplates.Add(CreateMonster("Pebble", new[] { MonsterType.Rock }, Rarity.Common, 70, 11, 5, 50, allAttacks, "Tackle", "Rock Throw", "Rock Slide", "Shield", "Stone Edge", "Earthquake"));
        MonsterTemplates.Add(CreateMonster("Zephyr", new[] { MonsterType.Flying }, Rarity.Common, 55, 17, 16, 50, allAttacks, "Tackle", "Gust", "Wing Attack", "Quick Attack", "Air Slash", "Hurricane"));
        MonsterTemplates.Add(CreateMonster("Mystic", new[] { MonsterType.Psychic }, Rarity.Common, 60, 15, 12, 50, allAttacks, "Tackle", "Confusion", "Psybeam", "Recover", "Psychic", "Future Sight"));
        MonsterTemplates.Add(CreateMonster("Frosty", new[] { MonsterType.Ice }, Rarity.Common, 62, 14, 11, 50, allAttacks, "Tackle", "Ice Shard", "Frost Breath", "Shield", "Ice Beam", "Blizzard"));
        MonsterTemplates.Add(CreateMonster("Rusty", new[] { MonsterType.Steel }, Rarity.Common, 68, 13, 8, 50, allAttacks, "Tackle", "Metal Claw", "Iron Head", "Shield", "Flash Cannon", "Meteor Mash"));
        MonsterTemplates.Add(CreateMonster("Shadow", new[] { MonsterType.Dark }, Rarity.Common, 58, 16, 13, 50, allAttacks, "Tackle", "Bite", "Dark Pulse", "Quick Attack", "Crunch", "Night Slash"));
        MonsterTemplates.Add(CreateMonster("Lumen", new[] { MonsterType.Light }, Rarity.Common, 60, 15, 12, 50, allAttacks, "Tackle", "Flash", "Light Beam", "Recover", "Dazzling Gleam", "Solar Flare"));
        MonsterTemplates.Add(CreateMonster("Nibbles", new[] { MonsterType.Normal }, Rarity.Common, 65, 14, 10, 50, allAttacks, "Tackle", "Scratch", "Quick Attack", "Recover", "Body Slam", "Hyper Beam"));

        // Ostala čudovišta (privremeno, samo da ne puca kod)
        MonsterTemplates.Add(CreateMonster("Inferno", new[] { MonsterType.Fire, MonsterType.Rock }, Rarity.Rare, 90, 22, 16, 70, allAttacks, "Tackle", "Ember", "Flamethrower", "Recover", "Fire Blast", "Stone Edge", "Inferno"));
    }

    // Ažurirana pomoćna metoda (dodati speed i stamina)
    private static Monster CreateMonster(string name, MonsterType[] types, Rarity rarity, int hp, int atk, int speed, int stamina, List<Attack> allAttacks, params string[] moveNames)
    {
        var m = new Monster(name, types.ToList(), rarity, hp, atk, speed, stamina);
        m.AvailableMoves = allAttacks.Where(a => moveNames.Contains(a.Name)).ToList();
        return m;
    }
}