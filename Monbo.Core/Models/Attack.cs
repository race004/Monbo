namespace Monbo.Core.Models;

public enum HealTarget
{
    None,
    Self,
    Ally,
    SelfAndAlly
}

public class Attack
{
    public string Name { get; set; } = string.Empty;
    public MonsterType Type { get; set; } = MonsterType.Normal;

    public int PowerPercent { get; set; } = 0;
    public int Accuracy { get; set; } = 100;
    public int StaminaCost { get; set; } = 5;

    public bool IsHeal { get; set; } = false;
    public HealTarget HealTarget { get; set; } = HealTarget.None;
    public int HealPercent { get; set; } = 0;

    // NOVO: Area napad (udara sve protivnike)
    public bool IsArea { get; set; } = false;

    // NOVO: Status efekat koji napad može da nanese
    public StatusEffect? EffectToApply { get; set; }
    public int EffectChance { get; set; } = 0; // Šansa u procentima (npr. 50)

    public int RequiredLevel { get; set; } = 1;

    // Konstruktor za štetne napade
    public Attack(string name, MonsterType type, int powerPercent, int accuracy, int requiredLevel, int staminaCost = 5, bool isArea = false)
    {
        Name = name; Type = type; PowerPercent = powerPercent; Accuracy = accuracy;
        RequiredLevel = requiredLevel; StaminaCost = staminaCost; IsArea = isArea;
        IsHeal = false;
    }

    // Konstruktor za heal napade
    public Attack(string name, int healPercent, int accuracy, HealTarget target, int requiredLevel, int staminaCost = 15)
    {
        Name = name; Type = MonsterType.None; HealPercent = healPercent; Accuracy = accuracy;
        HealTarget = target; RequiredLevel = requiredLevel; StaminaCost = staminaCost;
        IsHeal = true;
    }

    public Attack() { }
}