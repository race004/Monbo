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
        if (Level >= GetMaxLevel()) return; // Već je na maksimumu

        XP += amount;

        // Provera da li je sakupio dovoljno za sledeći nivo
        while (XP >= Level * 50 && Level < GetMaxLevel())
        {
            XP -= Level * 50;
            Level++;

            // Rast statistika u zavisnosti od redkosti
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
            CurrentHp = MaxHp; // Resetujemo HP na novi maksimum

            // Otključavanje novih napada na svakih 5 levela
            if (Level % 5 == 0)
            {
                var newMove = AvailableMoves.FirstOrDefault(m => m.RequiredLevel == Level);
                if (newMove != null)
                {
                    // Ako imamo manje od 4 aktivna napada, dodajemo ga odmah
                    if (ActiveMoves.Count < 4 && !ActiveMoves.Contains(newMove))
                    {
                        ActiveMoves.Add(newMove);
                    }
                    // Ako već imamo 4, za sada ga ne dodajemo automatski (to će kasnije igrač birati)
                }
            }
        }
    }
}