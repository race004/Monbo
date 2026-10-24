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
    public int Speed { get; set; }

    public int MaxStamina { get; set; }
    public int CurrentStamina { get; set; }

    // NOVO: Lista aktivnih statusa tokom borbe
    public List<StatusEffect> ActiveStatuses { get; set; } = new();

    public List<Attack> AvailableMoves { get; set; } = new();
    public List<Attack> ActiveMoves { get; set; } = new();

    // Konstruktor
    public Monster(string name, List<MonsterType> types, Rarity rarity, int baseHp, int baseAtk, int baseSpeed, int baseStamina)
    {
        Name = name;
        Types = types;
        Rarity = rarity;
        MaxHp = baseHp;
        CurrentHp = baseHp;
        Atk = baseAtk;
        Speed = baseSpeed;

        MaxStamina = baseStamina;
        CurrentStamina = MaxStamina;
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

    public void AddXP(int amount)
    {
        // (Ostavljamo postojeću logiku za XP i levelovanje)
        if (Level >= GetMaxLevel()) return;

        XP += amount;

        while (XP >= Level * 30 && Level < GetMaxLevel())
        {
            XP -= Level * 30;
            Level++;

            int hpGain = Rarity switch { Rarity.Common => 3, Rarity.Uncommon => 5, Rarity.Rare => 7, Rarity.Epic => 9, _ => 3 };
            int atkGain = Rarity switch { Rarity.Common => 1, Rarity.Uncommon => 2, Rarity.Rare => 3, Rarity.Epic => 4, _ => 1 };
            int speedGain = Rarity switch { Rarity.Common => 1, Rarity.Uncommon => 2, Rarity.Rare => 2, Rarity.Epic => 3, _ => 1 };
            int staminaGain = Rarity switch { Rarity.Common => 2, Rarity.Uncommon => 3, Rarity.Rare => 4, Rarity.Epic => 5, _ => 2 };

            MaxHp += hpGain;
            Atk += atkGain;
            Speed += speedGain;
            MaxStamina += staminaGain;

            CurrentHp = MaxHp;
            CurrentStamina = MaxStamina;

            // Logika za otključavanje napada ostaje ista
            // ...
        }
    }

    // NOVO: Trošenje stamine
    public bool UseStamina(int amount)
    {
        if (CurrentStamina >= amount)
        {
            CurrentStamina -= amount;
            return true;
        }
        return false;
    }

    // NOVO: Resetovanje stanja pred borbu
    public void ResetForBattle()
    {
        CurrentHp = MaxHp;
        CurrentStamina = MaxStamina;
        ActiveStatuses.Clear(); // Uklanja sve statuse pred novu borbu
    }
}