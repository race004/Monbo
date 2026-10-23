using Monbo.Core.Models;

namespace Monbo.Core.Logic;

public static class TeamBuilder
{
    public static Team BuildTeam(string teamName, List<Monster> selectedMonsters, PlayerInventory inventory)
    {
        var team = new Team(teamName);

        foreach (var monster in selectedMonsters)
        {
            // Uzimamo samo napade za level 1 (obično 2 napada)
            var startingMoves = monster.AvailableMoves.Where(m => m.RequiredLevel <= 1).Take(2).ToList();
            monster.ActiveMoves = new List<Attack>(startingMoves);

            team.Members.Add(monster);
            inventory.AddMonster(monster);
        }

        return team;
    }
}