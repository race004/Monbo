using Monbo.Core.Models;

namespace Monbo.Core.Data;

public static class GameData
{
    // Ovde ćemo čuvati sve šablone čudovišta
    public static List<Monster> MonsterTemplates { get; private set; } = new();

    // Statička metoda koja se poziva jednom na početku igre
    public static void Initialize()
    {
        // 1. Pravimo listu svih mogućih napada u igri
        var allAttacks = new List<Attack>
        {
            // Štetni napadi (Name, Type, Power, Accuracy, RequiredLevel)
            new Attack("Ember", MonsterType.Fire, 40, 100, 1),
            new Attack("Flamethrower", MonsterType.Fire, 90, 95, 10),
            new Attack("Water Gun", MonsterType.Water, 40, 100, 1),
            new Attack("Hydro Pump", MonsterType.Water, 110, 80, 15),
            new Attack("Vine Whip", MonsterType.Grass, 45, 100, 1),
            new Attack("Solar Beam", MonsterType.Grass, 120, 85, 20),
            new Attack("Thunder Shock", MonsterType.Electric, 40, 100, 1),
            new Attack("Thunderbolt", MonsterType.Electric, 90, 100, 12),
            new Attack("Rock Throw", MonsterType.Rock, 50, 90, 1),
            new Attack("Stone Edge", MonsterType.Rock, 100, 80, 18),
            new Attack("Gust", MonsterType.Flying, 40, 100, 1),
            new Attack("Hurricane", MonsterType.Flying, 110, 70, 22),
            new Attack("Confusion", MonsterType.Psychic, 50, 100, 1),
            new Attack("Psybeam", MonsterType.Psychic, 65, 100, 8),
            new Attack("Tackle", MonsterType.Normal, 35, 100, 1),
            new Attack("Hyper Beam", MonsterType.Normal, 150, 90, 25),

            // Heal napadi (Name, HealPercent, Accuracy, HealTarget, RequiredLevel)
            new Attack("Recover", 50, 100, HealTarget.Self, 5),
            new Attack("Synthesis", 30, 100, HealTarget.SelfAndAlly, 10),
            new Attack("Heal Bell", 40, 100, HealTarget.Ally, 5)
        };

        // 2. Pravimo šablone čudovišta
        // Monster(Name, Types, Rarity, BaseHp, BaseAtk)

        // --- COMMON (6 napada ukupno) ---
        var flameon = new Monster("Flameon", new List<MonsterType> { MonsterType.Fire }, Rarity.Common, 40, 12);
        flameon.AvailableMoves = allAttacks.Where(a => a.Name == "Tackle" || a.Name == "Ember" || a.Name == "Recover" || a.Name == "Flamethrower" || a.Name == "Quick Attack" || a.Name == "Hyper Beam").ToList();
        // Fallback ako neki napad ne postoji u listi, dodeljujemo šta imamo
        if (flameon.AvailableMoves.Count < 6) flameon.AvailableMoves = allAttacks.Take(6).ToList();
        MonsterTemplates.Add(flameon);

        var aquari = new Monster("Aquari", new List<MonsterType> { MonsterType.Water }, Rarity.Common, 45, 10);
        aquari.AvailableMoves = allAttacks.Where(a => a.Type == MonsterType.Water || a.Type == MonsterType.Normal || a.IsHeal).Take(6).ToList();
        MonsterTemplates.Add(aquari);

        // --- UNCOMMON (7 napada ukupno) ---
        var leafling = new Monster("Leafling", new List<MonsterType> { MonsterType.Grass }, Rarity.Uncommon, 50, 11);
        leafling.AvailableMoves = allAttacks.Where(a => a.Type == MonsterType.Grass || a.Type == MonsterType.Normal || a.IsHeal).Take(7).ToList();
        MonsterTemplates.Add(leafling);

        var sparky = new Monster("Sparky", new List<MonsterType> { MonsterType.Electric }, Rarity.Uncommon, 45, 14);
        sparky.AvailableMoves = allAttacks.Where(a => a.Type == MonsterType.Electric || a.Type == MonsterType.Normal || a.IsHeal).Take(7).ToList();
        MonsterTemplates.Add(sparky);

        // --- RARE (8 napada ukupno) ---
        var rocko = new Monster("Rocko", new List<MonsterType> { MonsterType.Rock }, Rarity.Rare, 60, 13);
        rocko.AvailableMoves = allAttacks.Where(a => a.Type == MonsterType.Rock || a.Type == MonsterType.Normal || a.IsHeal).Take(8).ToList();
        MonsterTemplates.Add(rocko);

        var windy = new Monster("Windy", new List<MonsterType> { MonsterType.Flying, MonsterType.Normal }, Rarity.Rare, 55, 15);
        windy.AvailableMoves = allAttacks.Where(a => a.Type == MonsterType.Flying || a.Type == MonsterType.Normal || a.IsHeal).Take(8).ToList();
        MonsterTemplates.Add(windy);

        // --- EPIC (9 napada ukupno) ---
        var psykid = new Monster("Psykid", new List<MonsterType> { MonsterType.Psychic }, Rarity.Epic, 65, 18);
        psykid.AvailableMoves = allAttacks.Where(a => a.Type == MonsterType.Psychic || a.Type == MonsterType.Normal || a.IsHeal).Take(9).ToList();
        MonsterTemplates.Add(psykid);
    }
}