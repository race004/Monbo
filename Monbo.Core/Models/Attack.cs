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

    // Ako je 0, napad ne nanosi štetu (npr. čist heal)
    public int PowerPercent { get; set; } = 0;

    public int Accuracy { get; set; } = 100;

    // Da li je napad štetan ili leči
    public bool IsHeal { get; set; } = false;
    public HealTarget HealTarget { get; set; } = HealTarget.None;
    public int HealPercent { get; set; } = 0;

    public int RequiredLevel { get; set; } = 1;

    // Konstruktor za štetne napade
    public Attack(string name, MonsterType type, int powerPercent, int accuracy, int requiredLevel)
    {
        Name = name;
        Type = type;
        PowerPercent = powerPercent;
        Accuracy = accuracy;
        RequiredLevel = requiredLevel;
        IsHeal = false;
    }

    // Konstruktor za heal napade
    public Attack(string name, int healPercent, int accuracy, HealTarget target, int requiredLevel)
    {
        Name = name;
        Type = MonsterType.None; // Heal napadi nemaju tip
        HealPercent = healPercent;
        Accuracy = accuracy;
        HealTarget = target;
        RequiredLevel = requiredLevel;
        IsHeal = true;
    }

    // Prazan konstruktor za JSON kasnije
    public Attack() { }
}