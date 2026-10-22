using Monbo.Core.Models;

namespace Monbo.Core.Save;

public class GameState
{
    public Team PlayerTeam { get; set; } = new();
    public PlayerInventory Inventory { get; set; } = new();
}