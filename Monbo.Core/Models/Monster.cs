using System;
using System.Collections.Generic;
using System.Linq;

namespace Monbo.Core.Models;

public class Monster
{
    public string Name { get; set; } = string.Empty;
    public List<MonsterType> Types { get; set; } = new();
    public Rarity Rarity { get; set; }

    public int Level { get; set; } = 1;
    public int XP { get; set; } = 0;

    public int MaxHp { get; set; }
    public int CurrentHp { get; set; }
    public int Atk { get; set; }

    // Svi napadi koje čudovište MOŽE da nauči
    public List<Attack> AvailableMoves { get; set; } = new();

    // Tačno 4 napada koja koristi u borbi
    public List<Attack> ActiveMoves { get; set; } = new();

    public Monster(string name, List<MonsterType> types, Rarity rarity, int baseHp, int baseAtk)
    {
        Name = name;
        Types = types;
        Rarity = rarity;
        MaxHp = baseHp;
        CurrentHp = baseHp;
        Atk = baseAtk;
    }

    public Monster() { }

    public bool IsAlive => CurrentHp > 0;

    public int GetMaxLevel()
    {
        return Rarity switch
        {
            Rarity.Common => 20,
            Rarity.Uncommon => 30,
            Rarity.Rare => 40,
            Rarity.Epic => 50,
            _ => 20
        };
    }

    // Metoda koja se poziva kada čudovište dobije XP
    public void AddXP(int amount)
    {
        if (Level >= GetMaxLevel()) return;

        XP += amount;

        while (XP >= Level * 30 && Level < GetMaxLevel())
        {
            XP -= Level * 30;
            Level++;

            // Rast statistika
            int hpGain = Rarity switch
            {
                Rarity.Common => 3,
                Rarity.Uncommon => 5,
                Rarity.Rare => 7,
                Rarity.Epic => 9,
                _ => 3
            };

            int atkGain = Rarity switch
            {
                Rarity.Common => 1,
                Rarity.Uncommon => 2,
                Rarity.Rare => 3,
                Rarity.Epic => 4,
                _ => 1
            };

            MaxHp += hpGain;
            Atk += atkGain;
            CurrentHp = MaxHp; // Reset HP na max

            // PROVERA ZA OTKLJUČAVANJE NAPADA (svakih 5 levela, a kasnije svakih 10)
            bool shouldUnlock = (Level <= 20 && Level % 5 == 0) || (Level > 20 && Level % 10 == 0);

            if (shouldUnlock)
            {
                // Nalazimo napad koji se otključava na ovom levelu
                var newMove = AvailableMoves.FirstOrDefault(m => m.RequiredLevel == Level);
                if (newMove != null)
                {
                    // Ako imamo manje od 4 aktivna napada, dodajemo ga odmah
                    if (ActiveMoves.Count < 4 && !ActiveMoves.Contains(newMove))
                    {
                        ActiveMoves.Add(newMove);
                        Console.WriteLine($"\n✨ {Name} je naučio novi napad: {newMove.Name}!");
                    }
                    else
                    {
                        // Ako već imamo 4, samo obaveštavamo igrača da je napad dostupan u inventaru
                        Console.WriteLine($"\n✨ {Name} može da nauči novi napad: {newMove.Name}! (Promeni u inventaru)");
                    }
                }
            }
        }
    }
}