using Monbo.Core.Models;
using Monbo.Core.Data;
using System.Threading;

namespace Monbo.Core.Battle;

public static class BattleEngine
{
    private static readonly Random _random = new();

    private static void TypeWrite(string text, int delay = 15)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay);
        }
        Console.WriteLine();
    }

    private static void WaitAndClear()
    {
        Thread.Sleep(1200);
        Console.Clear();
    }

    // ==========================================
    // BORBA 1 NA 1
    // ==========================================
    public static bool Battle1v1(Monster playerMonster, Monster enemyMonster, PlayerInventory inventory)
    {
        playerMonster.CurrentHp = playerMonster.MaxHp;
        enemyMonster.CurrentHp = enemyMonster.MaxHp;

        while (playerMonster.IsAlive && enemyMonster.IsAlive)
        {
            Console.Clear();
            DrawHeader("BORBA 1 vs 1");
            DrawMonsterStatus(playerMonster, "TVOJ TIM", true);
            Console.WriteLine("\n--------------------------------------------------\n");
            DrawMonsterStatus(enemyMonster, "PROTIVNIK", false);
            Console.WriteLine("\n==================================================");

            var playerMove = GetPlayerMove(playerMonster);
            Console.Clear();
            ExecuteAttack(playerMonster, new List<Monster> { playerMonster }, new List<Monster> { enemyMonster }, playerMove, true);
            WaitAndClear();

            if (!enemyMonster.IsAlive) break;

            Console.Clear();
            DrawHeader("BORBA 1 vs 1");
            var enemyMove = enemyMonster.ActiveMoves[_random.Next(enemyMonster.ActiveMoves.Count)];
            ExecuteAttack(enemyMonster, new List<Monster> { enemyMonster }, new List<Monster> { playerMonster }, enemyMove, false);
            WaitAndClear();
        }

        Console.Clear();
        if (playerMonster.IsAlive)
        {
            TypeWrite($"\n🏆 {playerMonster.Name} je pobedio!");

            int xpReward = enemyMonster.Level * 20;
            playerMonster.AddXP(xpReward);
            TypeWrite($"{playerMonster.Name} je dobio {xpReward} XP!");
            if (playerMonster.Level > 1) TypeWrite($"✨ {playerMonster.Name} je dostigao nivo {playerMonster.Level}!");

            TypeWrite($"\n✨ UHVAĆENO: {enemyMonster.Name} se pridružuje tvom inventaru!");
            inventory.AddMonster(enemyMonster, isCaught: true);
            return true;
        }
        else
        {
            TypeWrite($"\n💀 {playerMonster.Name} je izgubio...");
            return false;
        }
    }

    // ==========================================
    // BORBA TIM PROTIV TIMA (3v3)
    // ==========================================
    public static bool BattleTeamVsTeam(Team playerTeam, Team enemyTeam)
    {
        foreach (var m in playerTeam.Members) m.CurrentHp = m.MaxHp;
        foreach (var m in enemyTeam.Members) m.CurrentHp = m.MaxHp;

        var turnOrder = new List<(Monster monster, bool isPlayer)>();
        foreach (var m in playerTeam.Members) turnOrder.Add((m, true));
        foreach (var m in enemyTeam.Members) turnOrder.Add((m, false));
        turnOrder = turnOrder.OrderBy(x => _random.Next()).ToList();

        int turnIndex = 0;

        while (playerTeam.HasAlive && enemyTeam.HasAlive)
        {
            var current = turnOrder[turnIndex];

            if (!current.monster.IsAlive)
            {
                turnIndex = (turnIndex + 1) % turnOrder.Count;
                continue;
            }

            Console.Clear();
            DrawHeader($"BORBA TIM PROTIV TIMA");
            DrawTurnOrder(turnOrder, turnIndex);

            Console.WriteLine("\n=== TVOJ TIM ===");
            foreach (var m in playerTeam.Members) DrawMonsterStatus(m, "", true);

            Console.WriteLine("\n=== PROTIVNICKI TIM ===");
            foreach (var m in enemyTeam.Members) DrawMonsterStatus(m, "", false);

            Console.WriteLine("\n==================================================");

            var attacker = current.monster;
            var ownTeam = current.isPlayer ? playerTeam.Members : enemyTeam.Members;
            var enemyTeamList = current.isPlayer ? enemyTeam.Members : playerTeam.Members;

            if (current.isPlayer)
            {
                Console.WriteLine($"\nNa potezu je: {attacker.Name}");
                var move = GetPlayerMove(attacker);

                // Ako je heal, pitamo koga leči (ako je Ally ili SelfAndAlly)
                if (move.IsHeal && (move.HealTarget == HealTarget.Ally || move.HealTarget == HealTarget.SelfAndAlly))
                {
                    // Ako je SelfAndAlly, lečiće ceo tim, ne treba meta
                }
                else if (!move.IsHeal)
                {
                    // Ako je šteta, igrač bira metu
                    var target = ChooseTarget(enemyTeamList, "Izaberi metu za napad:");
                    enemyTeamList = new List<Monster> { target }; // Sužavamo listu na samo tu metu
                }

                Console.Clear();
                DrawHeader("TOK BORBE");
                ExecuteAttack(attacker, ownTeam, enemyTeamList, move, true);
            }
            else
            {
                TypeWrite($"\nNa potezu je PROTIVNIK: {attacker.Name}...");
                var move = attacker.ActiveMoves[_random.Next(attacker.ActiveMoves.Count)];

                // AI bira metu za štetu
                if (!move.IsHeal)
                {
                    var aliveEnemies = enemyTeamList.Where(m => m.IsAlive).ToList();
                    if (aliveEnemies.Count > 0)
                    {
                        var target = aliveEnemies[_random.Next(aliveEnemies.Count)];
                        enemyTeamList = new List<Monster> { target };
                    }
                }

                ExecuteAttack(attacker, ownTeam, enemyTeamList, move, false);
            }

            WaitAndClear();
            turnIndex = (turnIndex + 1) % turnOrder.Count;
        }

        Console.Clear();
        bool playerWon = playerTeam.HasAlive;
        if (playerWon)
        {
            TypeWrite($"\n🏆 {playerTeam.Name} pobeđuje!");
            int avgEnemyLevel = (int)enemyTeam.Members.Average(m => m.Level);
            int xpReward = avgEnemyLevel * 20;
            var survivors = playerTeam.Members.Where(m => m.IsAlive).ToList();
            foreach (var survivor in survivors)
            {
                int oldLevel = survivor.Level;
                survivor.AddXP(xpReward);
                TypeWrite($"{survivor.Name} dobija {xpReward} XP!");
                if (survivor.Level > oldLevel)
                {
                    TypeWrite($"✨ {survivor.Name} je dostigao nivo {survivor.Level}!");
                }
            }
        }
        else
        {
            TypeWrite($"\n💀 {enemyTeam.Name} pobeđuje...");
        }
        return playerWon;
    }

    // ==========================================
    // BORBA TIM PROTIV 1
    // ==========================================
    public static bool BattleTeamVs1(Team playerTeam, Monster enemyMonster, PlayerInventory inventory)
    {
        foreach (var m in playerTeam.Members) m.CurrentHp = m.MaxHp;
        enemyMonster.CurrentHp = enemyMonster.MaxHp;

        var turnOrder = new List<(Monster monster, bool isPlayer)>();
        foreach (var m in playerTeam.Members) turnOrder.Add((m, true));
        turnOrder.Add((enemyMonster, false));
        turnOrder = turnOrder.OrderBy(x => _random.Next()).ToList();

        int turnIndex = 0;

        while (playerTeam.HasAlive && enemyMonster.IsAlive)
        {
            var current = turnOrder[turnIndex];

            if (!current.monster.IsAlive)
            {
                turnIndex = (turnIndex + 1) % turnOrder.Count;
                continue;
            }

            Console.Clear();
            DrawHeader($"BORBA TIM PROTIV 1");
            DrawTurnOrder(turnOrder, turnIndex);

            Console.WriteLine("\n=== TVOJ TIM ===");
            foreach (var m in playerTeam.Members) DrawMonsterStatus(m, "", true);

            Console.WriteLine("\n=== PROTIVNIK ===");
            DrawMonsterStatus(enemyMonster, "", false);

            Console.WriteLine("\n==================================================");

            var attacker = current.monster;
            var ownTeam = current.isPlayer ? playerTeam.Members : new List<Monster> { enemyMonster };
            var enemyTeamList = current.isPlayer ? new List<Monster> { enemyMonster } : playerTeam.Members;

            if (current.isPlayer)
            {
                Console.WriteLine($"\nNa potezu je: {attacker.Name}");
                var move = GetPlayerMove(attacker);

                if (!move.IsHeal)
                {
                    // U 3v1, protivnik je samo jedan, pa nema potrebe birati metu (uvek je on)
                    // ali možemo ostaviti da se odmah napadne.
                }

                Console.Clear();
                DrawHeader("TOK BORBE");
                ExecuteAttack(attacker, ownTeam, enemyTeamList, move, true);
            }
            else
            {
                TypeWrite($"\nNa potezu je PROTIVNIK: {attacker.Name}...");
                var move = attacker.ActiveMoves[_random.Next(attacker.ActiveMoves.Count)];

                if (!move.IsHeal)
                {
                    var aliveEnemies = enemyTeamList.Where(m => m.IsAlive).ToList();
                    if (aliveEnemies.Count > 0)
                    {
                        var target = aliveEnemies[_random.Next(aliveEnemies.Count)];
                        enemyTeamList = new List<Monster> { target };
                    }
                }

                ExecuteAttack(attacker, ownTeam, enemyTeamList, move, false);
            }

            WaitAndClear();
            turnIndex = (turnIndex + 1) % turnOrder.Count;
        }

        Console.Clear();
        bool playerWon = playerTeam.HasAlive;
        if (playerWon)
        {
            TypeWrite($"\n🏆 {playerTeam.Name} pobeđuje!");
            int xpReward = enemyMonster.Level * 20;
            var survivors = playerTeam.Members.Where(m => m.IsAlive).ToList();
            foreach (var survivor in survivors)
            {
                int oldLevel = survivor.Level;
                survivor.AddXP(xpReward);
                TypeWrite($"{survivor.Name} dobija {xpReward} XP!");
                if (survivor.Level > oldLevel)
                {
                    TypeWrite($"✨ {survivor.Name} je dostigao nivo {survivor.Level}!");
                }
            }

            TypeWrite($"\n✨ UHVAĆENO: {enemyMonster.Name} se pridružuje tvom inventaru!");
            inventory.AddMonster(enemyMonster, isCaught: true);

            return true;
        }
        else
        {
            TypeWrite($"\n💀 {enemyMonster.Name} pobeđuje...");
        }
        return playerWon;
    }

    // ==========================================
    // UI I POMOĆNE METODE
    // ==========================================

    private static void DrawHeader(string title)
    {
        Console.WriteLine("==================================================");
        Console.WriteLine($"  {title.ToUpper()}");
        Console.WriteLine("==================================================");
    }

    private static void DrawTurnOrder(List<(Monster monster, bool isPlayer)> order, int currentIndex)
    {
        Console.Write("REDOSLED: ");
        for (int i = 0; i < order.Count; i++)
        {
            if (!order[i].monster.IsAlive) continue;

            string name = order[i].isPlayer ? $"[{order[i].monster.Name}]" : order[i].monster.Name;
            if (i == currentIndex)
                Console.Write($"👉 {name} 👈  ->  ");
            else
                Console.Write($"{name}  ->  ");
        }
        Console.WriteLine();
    }

    private static void DrawMonsterStatus(Monster m, string prefix, bool isPlayer)
    {
        if (!m.IsAlive)
        {
            Console.WriteLine($"{prefix} {m.Name} [MRTAV]");
            return;
        }

        string prefixText = string.IsNullOrEmpty(prefix) ? "" : $"{prefix} ";

        string typesStr = string.Join("/", m.Types);

        // DODAT XP U ISPIS
        int xpNeeded = m.Level * 50;
        Console.WriteLine($"{prefixText}{m.Name} ({typesStr}) (Lvl {m.Level}) | XP: {m.XP}/{xpNeeded}");
        Console.WriteLine($"  HP: {m.CurrentHp}/{m.MaxHp} | ATK: {m.Atk}");

        Console.Write("  Napadi: ");
        foreach (var move in m.ActiveMoves)
        {
            string typeStr = move.IsHeal ? "Heal" : move.Type.ToString().Substring(0, 3);
            string targetStr = "";
            if (move.IsHeal)
            {
                targetStr = move.HealTarget switch
                {
                    HealTarget.Self => " [Self]",
                    HealTarget.Ally => " [Ally]",
                    HealTarget.SelfAndAlly => " [Team]",
                    _ => ""
                };
            }
            Console.Write($"[{move.Name} ({typeStr}){targetStr}] ");
        }
        Console.WriteLine();
    }

    private static Attack GetPlayerMove(Monster playerMonster)
    {
        Console.WriteLine($"\nIzaberi napad za {playerMonster.Name}:");
        for (int i = 0; i < playerMonster.ActiveMoves.Count; i++)
        {
            var move = playerMonster.ActiveMoves[i];
            string type = move.IsHeal ? "Heal" : move.Type.ToString();
            string power = move.IsHeal ? $"Heal: {move.HealPercent}%" : $"Snaga: {move.PowerPercent}%";

            string targetStr = "";
            if (move.IsHeal)
            {
                targetStr = move.HealTarget switch
                {
                    HealTarget.Self => " [Self]",
                    HealTarget.Ally => " [Ally]",
                    HealTarget.SelfAndAlly => " [Team]",
                    _ => ""
                };
            }

            Console.WriteLine($"{i + 1}. {move.Name} ({type}){targetStr} - {power} | Preciznost: {move.Accuracy}%");
        }

        int choice;
        while (true)
        {
            Console.Write("> ");
            if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= playerMonster.ActiveMoves.Count)
                return playerMonster.ActiveMoves[choice - 1];
            Console.WriteLine("Neispravan unos.");
        }
    }

    private static Monster ChooseTarget(List<Monster> enemies, string prompt)
    {
        var aliveEnemies = enemies.Where(m => m.IsAlive).ToList();
        if (aliveEnemies.Count == 1) return aliveEnemies[0]; // Ako je samo jedan, uzmi ga

        Console.WriteLine($"\n{prompt}");
        for (int i = 0; i < aliveEnemies.Count; i++)
        {
            var e = aliveEnemies[i];
            string typesStr = string.Join("/", e.Types);
            Console.WriteLine($"{i + 1}. {e.Name} ({typesStr}) - HP: {e.CurrentHp}/{e.MaxHp}");
        }

        int choice;
        while (true)
        {
            Console.Write("> ");
            if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= aliveEnemies.Count)
                return aliveEnemies[choice - 1];
            Console.WriteLine("Neispravan unos.");
        }
    }

    private static void ExecuteAttack(Monster attacker, List<Monster> ownTeam, List<Monster> enemyTeam, Attack move, bool isPlayerAttacking)
    {
        string attackerName = isPlayerAttacking ? attacker.Name : $"Protivnik ({attacker.Name})";

        if (_random.Next(1, 101) > move.Accuracy)
        {
            TypeWrite($"  {attackerName} koristi {move.Name}... ali PROMAŠUJE!");
            return;
        }

        if (move.IsHeal)
        {
            int healAmount = (int)(attacker.MaxHp * (move.HealPercent / 100.0));

            if (move.HealTarget == HealTarget.Self)
            {
                int oldHp = attacker.CurrentHp;
                attacker.CurrentHp = Math.Min(attacker.MaxHp, attacker.CurrentHp + healAmount);
                TypeWrite($"  {attackerName} koristi {move.Name} i leči sebe za {attacker.CurrentHp - oldHp} HP!");
            }
            else if (move.HealTarget == HealTarget.Ally)
            {
                // Nalazimo živog saveznika (ne sebe)
                var ally = ownTeam.FirstOrDefault(m => m != attacker && m.IsAlive);
                if (ally != null)
                {
                    int oldHp = ally.CurrentHp;
                    ally.CurrentHp = Math.Min(ally.MaxHp, ally.CurrentHp + healAmount);
                    TypeWrite($"  {attackerName} koristi {move.Name} i leči saveznika {ally.Name} za {ally.CurrentHp - oldHp} HP!");
                }
                else
                {
                    TypeWrite($"  {attackerName} koristi {move.Name}, ali nema živog saveznika da izleči!");
                }
            }
            else if (move.HealTarget == HealTarget.SelfAndAlly)
            {
                // Leči ceo tim (uključujući sebe)
                int totalHealed = 0;
                foreach (var member in ownTeam.Where(m => m.IsAlive))
                {
                    int oldHp = member.CurrentHp;
                    member.CurrentHp = Math.Min(member.MaxHp, member.CurrentHp + healAmount);
                    totalHealed += (member.CurrentHp - oldHp);
                }
                TypeWrite($"  {attackerName} koristi {move.Name} i leči ceo tim (ukupno {totalHealed} HP)!");
            }
            return;
        }

        var defender = enemyTeam.FirstOrDefault(m => m.IsAlive);
        if (defender == null) return;

        double typeMultiplier = TypeChart.GetMultiplier(move.Type, defender.Types);
        string effectiveness = typeMultiplier > 1.0 ? " (Veoma efikasno!)" :
                               typeMultiplier < 1.0 ? " (Nije baš efikasno...)" : "";

        double baseDamage = attacker.Atk * (move.PowerPercent / 100.0);
        int finalDamage = (int)Math.Max(1, Math.Round(baseDamage * typeMultiplier));

        defender.CurrentHp = Math.Max(0, defender.CurrentHp - finalDamage);
        TypeWrite($"  {attackerName} koristi {move.Name} na {defender.Name} i nanosi {finalDamage} štete!{effectiveness}");

        if (!defender.IsAlive)
        {
            TypeWrite($"  💀 {defender.Name} je onesvešćen!");
        }
    }
}