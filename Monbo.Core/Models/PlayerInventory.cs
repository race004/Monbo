namespace Monbo.Core.Models;

public class PlayerInventory
{
    // Sva čudovišta koja igrač poseduje (uključujući i ona u timu)
    public List<Monster> AllMonsters { get; set; } = new();

    public void AddMonster(Monster monster, bool isCaught = false)
    {
        if (isCaught)
        {
            // Ako je uhvaćeno, resetujemo ga na level 1
            monster.Level = 1;
            monster.XP = 0;
            monster.ActiveMoves = monster.AvailableMoves.Take(4).ToList();
        }

        monster.CurrentHp = monster.MaxHp;
        AllMonsters.Add(monster);
    }
}