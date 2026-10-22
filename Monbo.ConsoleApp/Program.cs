using Monbo.Core.Data;
using Monbo.Core.Models;
using Monbo.Core.Logic;
using Monbo.ConsoleApp;
using Monbo.Core.Battle;
using Monbo.Core.Save;

GameData.Initialize();

// Glavna petlja za početni meni
while (true)
{
    Console.Clear();
    Console.WriteLine("==================================================");
    Console.WriteLine("                     MONBO                        ");
    Console.WriteLine("==================================================");
    Console.WriteLine("1. Nova igra");
    Console.WriteLine("2. Učitaj igru");
    Console.WriteLine("0. Izlaz");
    Console.WriteLine("==================================================");

    int choice = InputHelper.GetIntInput("Izbor: ", 0, 2);

    if (choice == 0) break; // Izlaz iz programa

    if (choice == 1)
    {
        // Nova igra - pravljenje tima
        PlayerInventory newInventory = new PlayerInventory();
        Team? newTeam = BuildNewTeam(newInventory);

        // Ako je igrač otkazao, newTeam će biti null, pa se neće ući u igru
        if (newTeam != null)
        {
            Console.WriteLine("\nTim je uspešno napravljen! Pritisni Enter za ulazak u igru...");
            Console.ReadLine();

            // Ulazak u glavni meni igre
            GameLoop(newTeam, newInventory);
        }
    }
    else if (choice == 2)
    {
        // Učitavanje igre
        var loadedData = LoadGame();
        if (loadedData != null)
        {
            Console.WriteLine("\nIgra je uspešno učitana! Pritisni Enter...");
            Console.ReadLine();
            GameLoop(loadedData.Value.Team, loadedData.Value.Inventory);
        }
    }
}

// ==========================================
// GLAVNA PETLJA IGRE (kada je tim napravljen)
// ==========================================
void GameLoop(Team playerTeam, PlayerInventory inventory)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("==================================================");
        Console.WriteLine("                 GLAVNI MENI IGRE                 ");
        Console.WriteLine("==================================================");
        Console.WriteLine("1. Borba: Tim vs Tim");
        Console.WriteLine("2. Borba: Tim vs 1");
        Console.WriteLine("3. Borba: 1 vs 1");
        Console.WriteLine("4. Moj Inventar i Tim");
        Console.WriteLine("5. Sačuvaj igru");
        Console.WriteLine("0. Nazad na glavni meni");
        Console.WriteLine("==================================================");

        int choice = InputHelper.GetIntInput("Izbor: ", 0, 5);

        if (choice == 0) break;

        switch (choice)
        {
            case 1:
                if (CheckTeamReady(playerTeam))
                {
                    StartBattleTeamVsTeam(playerTeam);
                    AutoSave(playerTeam, inventory);
                }
                break;
            case 2:
                if (CheckTeamReady(playerTeam))
                {
                    StartBattleTeamVs1(playerTeam, inventory);
                    AutoSave(playerTeam, inventory);
                }
                break;
            case 3:
                if (CheckTeamReady(playerTeam))
                {
                    StartBattle1v1(playerTeam, inventory);
                    AutoSave(playerTeam, inventory);
                }
                break;
            case 4:
                ManageTeamAndInventory(playerTeam, inventory);
                break;
            case 5:
                SaveGame(playerTeam, inventory);
                break;
        }
    }
}

// ==========================================
// POMOĆNE METODE
// ==========================================

bool CheckTeamReady(Team team)
{
    if (team.Members.Count == 0)
    {
        Console.WriteLine("Vaš tim je prazan! Idite na opciju 4 da dodate čudovište. Pritisni Enter...");
        Console.ReadLine();
        return false;
    }
    return true;
}

Team? BuildNewTeam(PlayerInventory inventory)
{
    Console.Clear();
    Console.WriteLine("--- Biranje TVOG PRVOG čudovišta ---");
    Console.WriteLine("(Unesi 0 da se vratiš na glavni meni)\n");

    for (int i = 0; i < GameData.MonsterTemplates.Count; i++)
    {
        var m = GameData.MonsterTemplates[i];
        Console.WriteLine($"{i + 1}. {m.Name} ({m.Rarity}) - Tip: {string.Join(", ", m.Types)} - HP: {m.MaxHp}, ATK: {m.Atk}");
    }

    int choice = InputHelper.GetIntInput("Izaberi čudovište: ", 0, GameData.MonsterTemplates.Count);

    if (choice == 0)
    {
        Console.WriteLine("Otkazano pravljenje tima. Pritisni Enter...");
        Console.ReadLine();
        return null;
    }

    var selectedTemplate = GameData.MonsterTemplates[choice - 1];
    var newMonster = new Monster(selectedTemplate.Name, new List<MonsterType>(selectedTemplate.Types), selectedTemplate.Rarity, selectedTemplate.MaxHp, selectedTemplate.Atk);
    newMonster.AvailableMoves = new List<Attack>(selectedTemplate.AvailableMoves);

    Console.WriteLine($"\nIzabrao si {newMonster.Name}. Sada izaberi 4 napada:");
    while (newMonster.ActiveMoves.Count < 4)
    {
        Console.WriteLine($"\nDostupni napadi (Izabrano {newMonster.ActiveMoves.Count}/4):");
        for (int i = 0; i < newMonster.AvailableMoves.Count; i++)
        {
            var a = newMonster.AvailableMoves[i];
            Console.WriteLine($"{i + 1}. {a.Name} ({(a.IsHeal ? "Heal" : a.Type.ToString())})");
        }

        int moveChoice = InputHelper.GetIntInput("Izaberi napad: ", 1, newMonster.AvailableMoves.Count);
        var selectedMove = newMonster.AvailableMoves[moveChoice - 1];

        if (newMonster.ActiveMoves.Contains(selectedMove))
            Console.WriteLine("Već si izabrao taj napad!");
        else
        {
            newMonster.ActiveMoves.Add(selectedMove);
            Console.WriteLine($"Dodat napad: {selectedMove.Name}");
        }
    }

    // Pravimo tim sa samo jednim članom
    List<Monster> startingMonsters = new() { newMonster };
    return TeamBuilder.BuildTeam("Igrač", startingMonsters, inventory);
}

void ManageTeamAndInventory(Team team, PlayerInventory inv)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("=== INVENTAR I TIM ===");

        // Prikaz TIMSKIH čudovišta
        Console.WriteLine($"Tvoj tim ({team.Members.Count}/3):");
        if (team.Members.Count == 0) Console.WriteLine("  (Tim je prazan)");
        for (int i = 0; i < team.Members.Count; i++)
        {
            var m = team.Members[i];
            int xpNeeded = m.Level * 50;
            Console.WriteLine($"{i + 1}. {m.Name} (Lvl {m.Level}) HP:{m.MaxHp} ATK:{m.Atk} | XP: {m.XP}/{xpNeeded}");
        }

        // Prikaz SVIH čudovišta iz inventara
        Console.WriteLine("\nSva uhvaćena i posedovana čudovišta (sortirano po levelu):");
        var sortedMonsters = inv.AllMonsters.OrderByDescending(m => m.Level).ToList();

        for (int i = 0; i < sortedMonsters.Count; i++)
        {
            var m = sortedMonsters[i];
            bool inTeam = team.Members.Contains(m);
            int xpNeeded = m.Level * 50;
            string teamStatus = inTeam ? " [U TIMU]" : "";
            Console.WriteLine($"{i + 1}. {m.Name} (Lvl {m.Level}) HP:{m.MaxHp} ATK:{m.Atk} | XP: {m.XP}/{xpNeeded}{teamStatus}");
        }

        // Novi, pametniji meni
        Console.WriteLine("\n=== OPCIJE ===");
        if (team.Members.Count < 3) Console.WriteLine("1. Dodaj člana u tim");
        if (team.Members.Count > 0) Console.WriteLine("2. Izbaci člana iz tima");
        if (team.Members.Count == 3) Console.WriteLine("3. Zameni člana tima");
        Console.WriteLine("4. Promeni aktivne napade čudovištu");
        Console.WriteLine("0. Nazad na glavni meni");

        int choice = InputHelper.GetIntInput("Izbor: ", 0, 4);

        if (choice == 0) break;

        if (choice == 1 && team.Members.Count < 3)
        {
            // DODAVANJE
            Console.WriteLine("\nIzaberi KOGA dodaješ u tim (broj iz liste, 0 za nazad):");
            int invIndex = InputHelper.GetIntInput("> ", 0, sortedMonsters.Count);
            if (invIndex == 0) continue;

            var monsterToAdd = sortedMonsters[invIndex - 1];
            if (team.Members.Contains(monsterToAdd)) { Console.WriteLine("To čudovište je već u timu!"); Console.ReadLine(); continue; }

            team.Members.Add(monsterToAdd);
            Console.WriteLine($"{monsterToAdd.Name} je dodat u tim! Pritisni Enter...");
            Console.ReadLine();
        }
        else if (choice == 2 && team.Members.Count > 0)
        {
            // IZBACIVANJE
            Console.WriteLine("\nIzaberi KOGA izbacuješ iz tima (0 za nazad):");
            int teamIndex = InputHelper.GetIntInput("> ", 0, team.Members.Count);
            if (teamIndex == 0) continue;

            var removed = team.Members[teamIndex - 1];
            team.Members.RemoveAt(teamIndex - 1);
            Console.WriteLine($"{removed.Name} je izbačen iz tima! Pritisni Enter...");
            Console.ReadLine();
        }
        else if (choice == 3 && team.Members.Count == 3)
        {
            // ZAMENA
            Console.WriteLine("\nIzaberi koga MENJAŠ iz tima (0 za nazad):");
            int teamIndex = InputHelper.GetIntInput("> ", 0, team.Members.Count);
            if (teamIndex == 0) continue;

            Console.WriteLine("\nIzaberi KOGA ubacuješ iz inventara:");
            int invIndex = InputHelper.GetIntInput("> ", 1, sortedMonsters.Count);

            var monsterToAdd = sortedMonsters[invIndex - 1];
            if (team.Members.Contains(monsterToAdd)) { Console.WriteLine("To čudovište je već u timu!"); Console.ReadLine(); continue; }

            team.Members[teamIndex - 1] = monsterToAdd;
            Console.WriteLine("Tim je ažuriran! Pritisni Enter...");
            Console.ReadLine();
        }
        else if (choice == 4)
        {
            // MENJANJE NAPADA (ostaje isto)
            Console.WriteLine("\nIzaberi čudovište kome menjaš napade (0 za nazad):");
            int monsterIndexInput = InputHelper.GetIntInput("> ", 0, sortedMonsters.Count);
            if (monsterIndexInput == 0) continue;

            var selectedMonster = sortedMonsters[monsterIndexInput - 1];

            Console.WriteLine($"\nTrenutni aktivni napadi za {selectedMonster.Name}:");
            for (int i = 0; i < selectedMonster.ActiveMoves.Count; i++)
            {
                var a = selectedMonster.ActiveMoves[i];
                Console.WriteLine($"{i + 1}. {a.Name} ({(a.IsHeal ? "Heal" : a.Type.ToString())})");
            }

            Console.WriteLine("\nKoji napad želiš da zameniš? (0 za nazad):");
            int moveToReplaceInput = InputHelper.GetIntInput("> ", 0, selectedMonster.ActiveMoves.Count);
            if (moveToReplaceInput == 0) continue;

            var moveToReplace = selectedMonster.ActiveMoves[moveToReplaceInput - 1];

            var availableMoves = selectedMonster.AvailableMoves
                .Where(m => !selectedMonster.ActiveMoves.Contains(m))
                .ToList();

            if (availableMoves.Count == 0)
            {
                Console.WriteLine("Ovo čudovište nema drugih dostupnih napada!");
                Console.ReadLine();
                continue;
            }

            Console.WriteLine($"\nDostupni napadi koje {selectedMonster.Name} može da nauči:");
            for (int i = 0; i < availableMoves.Count; i++)
            {
                var a = availableMoves[i];
                Console.WriteLine($"{i + 1}. {a.Name} ({(a.IsHeal ? "Heal" : a.Type.ToString())}) - Snaga: {a.PowerPercent}%");
            }

            Console.WriteLine("\nKoji napad želiš da DODAŠ? (0 za nazad):");
            int newMoveInput = InputHelper.GetIntInput("> ", 0, availableMoves.Count);
            if (newMoveInput == 0) continue;

            var newMove = availableMoves[newMoveInput - 1];

            selectedMonster.ActiveMoves[moveToReplaceInput - 1] = newMove;
            Console.WriteLine($"\nUspešno! {selectedMonster.Name} je zamenio napad {moveToReplace.Name} sa {newMove.Name}.");
            Console.ReadLine();
        }
    }
}

void StartBattleTeamVsTeam(Team team)
{
    int avgLevel = (int)team.Members.Average(m => m.Level);
    var enemyTeam = new Team("Protivnik");
    for (int i = 0; i < 3; i++)
    {
        int lvl = Math.Max(1, avgLevel + new Random().Next(-2, 3));
        enemyTeam.Members.Add(CreateRandomEnemy(lvl));
    }
    BattleEngine.BattleTeamVsTeam(team, enemyTeam);
}

void StartBattleTeamVs1(Team team, PlayerInventory inv)
{
    int avgLevel = (int)team.Members.Average(m => m.Level);
    int lvl = Math.Max(1, avgLevel + 5);
    var enemy = CreateRandomEnemy(lvl);
    BattleEngine.BattleTeamVs1(team, enemy, inv);
}

void StartBattle1v1(Team team, PlayerInventory inv)
{
    Console.Clear();
    Console.WriteLine("Izaberi svoje čudovište za 1v1 borbu:");
    for (int i = 0; i < team.Members.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {team.Members[i].Name} (Lvl {team.Members[i].Level})");
    }
    int choice = InputHelper.GetIntInput("> ", 1, team.Members.Count) - 1;
    var myFighter = team.Members[choice];

    int avgLevel = (int)team.Members.Average(m => m.Level);
    int lvl = Math.Max(1, avgLevel + new Random().Next(-2, 3));
    var enemy = CreateRandomEnemy(lvl);

    BattleEngine.Battle1v1(myFighter, enemy, inv);
}

Monster CreateRandomEnemy(int level)
{
    var template = GameData.MonsterTemplates[new Random().Next(GameData.MonsterTemplates.Count)];
    var enemy = new Monster(template.Name, new List<MonsterType>(template.Types), template.Rarity, template.MaxHp, template.Atk);
    enemy.AvailableMoves = new List<Attack>(template.AvailableMoves);
    enemy.ActiveMoves = enemy.AvailableMoves.Take(4).ToList();
    enemy.Level = level;
    return enemy;
}

// ==========================================
// METODE ZA SAVE/LOAD
// ==========================================

void SaveGame(Team team, PlayerInventory inv)
{
    var slots = SaveManager.LoadSlots();

    Console.WriteLine("\n=== IZABERI SLOT ZA ČUVANJE ===");
    for (int i = 0; i < slots.Count; i++)
        Console.WriteLine($"{i + 1}. {slots[i].Name}");

    int choice = InputHelper.GetIntInput("Izbor (1-4): ", 1, 4) - 1;

    Console.Write("Unesi ime za ovaj save: ");
    string name = Console.ReadLine() ?? "Moj Save";

    slots[choice].Name = name;
    slots[choice].State.PlayerTeam = team;
    slots[choice].State.Inventory = inv;

    SaveManager.SaveSlots(slots);
    Console.WriteLine("Igra je uspešno sačuvana! Pritisni Enter...");
    Console.ReadLine();
}

(Team Team, PlayerInventory Inventory)? LoadGame()
{
    var slots = SaveManager.LoadSlots();

    Console.WriteLine("\n=== IZABERI SLOT ZA UČITAVANJE ===");
    for (int i = 0; i < slots.Count; i++)
        Console.WriteLine($"{i + 1}. {slots[i].Name}");

    int choice = InputHelper.GetIntInput("Izbor (1-4): ", 1, 4) - 1;

    if (slots[choice].State.PlayerTeam.Members.Count == 0)
    {
        Console.WriteLine("Izabrali ste prazan slot. Pritisni Enter...");
        Console.ReadLine();
        return null;
    }

    return (slots[choice].State.PlayerTeam, slots[choice].State.Inventory);
}

void AutoSave(Team team, PlayerInventory inv)
{
    var slots = SaveManager.LoadSlots();
    int slotIndex = slots.FindIndex(s => s.Name != "Prazan slot 1" && s.Name != "Prazan slot 2" && s.Name != "Prazan slot 3" && s.Name != "Prazan slot 4");
    if (slotIndex == -1) slotIndex = 0;

    slots[slotIndex].State.PlayerTeam = team;
    slots[slotIndex].State.Inventory = inv;
    if (slots[slotIndex].Name.Contains("Prazan")) slots[slotIndex].Name = "AutoSave";

    SaveManager.SaveSlots(slots);
    Console.WriteLine("\n[Automatski sačuvano]");
    System.Threading.Thread.Sleep(1000);
}