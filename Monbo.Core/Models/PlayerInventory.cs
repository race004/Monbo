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

            // Uzimamo samo napade koji su dostupni na levelu 1 (obično 2 napada)
            var startingMoves = monster.AvailableMoves.Where(m => m.RequiredLevel <= 1).Take(2).ToList();
            monster.ActiveMoves = new List<Attack>(startingMoves);
        }

        monster.CurrentHp = monster.MaxHp;
        AllMonsters.Add(monster);
    }
}