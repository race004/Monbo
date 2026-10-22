using Monbo.Core.Models;

namespace Monbo.Core.Logic;

public static class TeamBuilder
{
    public static Team BuildTeam(string teamName, List<Monster> selectedMonsters, PlayerInventory inventory)
    {
        var team = new Team(teamName);

        foreach (var monster in selectedMonsters)
        {
            monster.ActiveMoves = monster.AvailableMoves.Take(4).ToList();
            team.Members.Add(monster);

            // Dodajemo čudovište u inventar odmah pri pravljenju tima
            inventory.AddMonster(monster);
        }

        return team;
    }
}