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
        playerMonster.ResetForBattle();
        enemyMonster.ResetForBattle();

        while (playerMonster.IsAlive && enemyMonster.IsAlive)
        {
            Console.Clear();
            DrawHeader("BORBA 1 vs 1");
            DrawMonsterStatus(playerMonster, "TVOJ TIM", true);
            Console.WriteLine("\n--------------------------------------------------\n");
            DrawMonsterStatus(enemyMonster, "PROTIVNIK", false);
            Console.WriteLine("\n==================================================");

            // 1. Provera statusa na početku poteza
            ProcessStatusEffects(playerMonster);
            ProcessStatusEffects(enemyMonster);
            if (!playerMonster.IsAlive || !enemyMonster.IsAlive) break;

            // 2. Određivanje redosleda na osnovu Speed
            bool playerFirst = playerMonster.Speed >= enemyMonster.Speed;

            if (playerFirst)
            {
                // TI IDEŠ PRVI
                if (!ExecuteTurn(playerMonster, enemyMonster, true, inventory)) return false;
                if (!enemyMonster.IsAlive) break;

                // Protivnik ide posle tebe
                Console.WriteLine("\nPritisni Enter da vidiš protivnikov potez...");
                Console.ReadLine();
                Console.Clear();
                DrawHeader("PROTIVNIKOV POTEZ");

                if (!ExecuteTurn(enemyMonster, playerMonster, false, inventory)) return false;
            }
            else
            {
                // PROTIVNIK IDE PRVI
                Console.Clear();
                DrawHeader("PROTIVNIKOV POTEZ");
                if (!ExecuteTurn(enemyMonster, playerMonster, false, inventory)) return false;
                if (!playerMonster.IsAlive) break;

                // Sada je tvoj red - čekamo da pritisneš Enter da bi video svoj status
                Console.WriteLine("\nPritisni Enter za tvoj potez...");
                Console.ReadLine();

                // Prikazujemo tvoj status pre nego što izabereš napad
                Console.Clear();
                DrawHeader("TVOJ POTEZ");
                DrawMonsterStatus(playerMonster, "TVOJ TIM", true);
                Console.WriteLine("\n--------------------------------------------------\n");
                DrawMonsterStatus(enemyMonster, "PROTIVNIK", false);
                Console.WriteLine("\n==================================================");

                if (!ExecuteTurn(playerMonster, enemyMonster, true, inventory)) return false;
            }

            WaitAndClear();
        }

        Console.Clear();
        if (playerMonster.IsAlive)
        {
            TypeWrite($"\n🏆 {playerMonster.Name} je pobedio!");
            int xpReward = enemyMonster.Level * 20;
            playerMonster.AddXP(xpReward);
            TypeWrite($"{playerMonster.Name} je dobio {xpReward} XP!");

            TypeWrite($"\n✨ UHVAĆENO: {enemyMonster.Name} se pridružuje tvom inventaru!");
            inventory.AddMonster(enemyMonster, isCaught: true);
            return true;
        }
        else
        {
            TypeWrite($"\n💀 {playerMonster.Name} je izgubio...");
            return false;
        }
    }    // ==========================================
    // BORBA TIM PROTIV TIMA (3v3) i TIM PROTIV 1
    // ==========================================
    public static bool BattleTeam(Team playerTeam, Team enemyTeam, PlayerInventory inventory, bool canCatch)
    {
        foreach (var m in playerTeam.Members) m.ResetForBattle();
        foreach (var m in enemyTeam.Members) m.ResetForBattle();

        while (playerTeam.HasAlive && enemyTeam.HasAlive)
        {
            // Proveravamo statuse za sve žive na početku kruga
            foreach (var m in playerTeam.Members.Where(x => x.IsAlive)) ProcessStatusEffects(m);
            foreach (var m in enemyTeam.Members.Where(x => x.IsAlive)) ProcessStatusEffects(m);
            if (!playerTeam.HasAlive || !enemyTeam.HasAlive) break;

            // Pravimo listu svih živih i sortiramo po brzini
            var allCombatants = new List<(Monster monster, bool isPlayer)>();
            foreach (var m in playerTeam.Members.Where(x => x.IsAlive)) allCombatants.Add((m, true));
            foreach (var m in enemyTeam.Members.Where(x => x.IsAlive)) allCombatants.Add((m, false));

            var sortedBySpeed = allCombatants.OrderByDescending(x => x.monster.Speed).ToList();

            // Svi napadaju po redosledu
            foreach (var (attacker, isPlayer) in sortedBySpeed)
            {
                if (!attacker.IsAlive) continue;

                var targetTeam = isPlayer ? enemyTeam : playerTeam;
                var target = targetTeam.Members.FirstOrDefault(m => m.IsAlive);

                if (target == null) break;

                // Ako je igrač na potezu, prikaži mu status pre nego što odluči
                if (isPlayer)
                {
                    Console.Clear();
                    DrawHeader($"NA POTEZU: {attacker.Name}");
                    Console.WriteLine("=== TVOJ TIM ===");
                    foreach (var m in playerTeam.Members) DrawMonsterStatus(m, "", true);
                    Console.WriteLine("\n=== PROTIVNICKI TIM ===");
                    foreach (var m in enemyTeam.Members) DrawMonsterStatus(m, "", false);
                    Console.WriteLine("\n==================================================");

                    if (!ExecuteTurn(attacker, target, isPlayer, inventory)) return false;

                    Console.WriteLine("\nPritisni Enter za nastavak...");
                    Console.ReadLine();
                }
                else
                {
                    Console.Clear();
                    DrawHeader($"NA POTEZU JE PROTIVNIK: {attacker.Name}");
                    if (!ExecuteTurn(attacker, target, isPlayer, inventory)) return false;

                    Console.WriteLine("\nPritisni Enter za nastavak...");
                    Console.ReadLine();
                }
            }

            WaitAndClear();
        }

        Console.Clear();
        bool playerWon = playerTeam.HasAlive;
        if (playerWon)
        {
            TypeWrite($"\n🏆 {playerTeam.Name} pobeđuje!");
            int totalEnemyLevels = enemyTeam.Members.Sum(m => m.Level);
            int totalXpPool = totalEnemyLevels * 20;
            int xpPerMonster = totalXpPool / playerTeam.Members.Count;

            TypeWrite($"\nUkupno osvojeno XP: {totalXpPool}");
            var survivors = playerTeam.Members.Where(m => m.IsAlive).ToList();
            foreach (var survivor in survivors)
            {
                int oldLevel = survivor.Level;
                survivor.AddXP(xpPerMonster);
                TypeWrite($"{survivor.Name} dobija {xpPerMonster} XP!");
                if (survivor.Level > oldLevel) TypeWrite($"✨ {survivor.Name} je dostigao nivo {survivor.Level}!");
            }

            if (canCatch && enemyTeam.Members.Count == 1)
            {
                TypeWrite($"\n✨ UHVAĆENO: {enemyTeam.Members[0].Name} se pridružuje tvom inventaru!");
                inventory.AddMonster(enemyTeam.Members[0], isCaught: true);
            }
        }
        else
        {
            TypeWrite($"\n💀 {enemyTeam.Name} pobeđuje...");
        }
        return playerWon;
    }
    // ==========================================
    // POMOĆNE METODE ZA POTEZE
    // ==========================================

    private static bool ExecuteTurn(Monster attacker, Monster target, bool isPlayer, PlayerInventory inventory)
    {
        // Provera da li je čudovište pod Stun/Freeze efektom
        var stunEffect = attacker.ActiveStatuses.FirstOrDefault(s => s.SkipTurn);
        if (stunEffect != null)
        {
            TypeWrite($"  {attacker.Name} ne može da napadne! ({stunEffect.Name})");
            stunEffect.Duration--;
            if (stunEffect.Duration <= 0) attacker.ActiveStatuses.Remove(stunEffect);
            return true;
        }

        Attack? chosenMove = null;
        if (isPlayer)
        {
            chosenMove = GetPlayerMove(attacker);
            if (chosenMove == null) return false; // Pobegao
        }
        else
        {
            // AI bira nasumičan napad za koji ima stamine
            var validMoves = attacker.ActiveMoves.Where(m => attacker.CurrentStamina >= m.StaminaCost).ToList();
            if (validMoves.Count > 0) chosenMove = validMoves[_random.Next(validMoves.Count)];
            else chosenMove = new Attack("Charge", MonsterType.Normal, 0, 100, 1, 0); // AI puni energiju
        }

        Console.Clear();
        DrawHeader("TOK BORBE");

        if (chosenMove.Name == "Charge")
        {
            int staminaRestored = (int)(attacker.MaxStamina * 0.5);
            attacker.CurrentStamina = Math.Min(attacker.MaxStamina, attacker.CurrentStamina + staminaRestored);
            TypeWrite($"  {attacker.Name} puni energiju i vraća {staminaRestored} stamine!");
        }
        else
        {
            ExecuteAttack(attacker, new List<Monster> { target }, chosenMove, isPlayer);
        }

        return true;
    }

    private static Attack? GetPlayerMove(Monster playerMonster)
    {
        Console.WriteLine($"\nIzaberi napad za {playerMonster.Name} (Stamina: {playerMonster.CurrentStamina}/{playerMonster.MaxStamina}):");
        for (int i = 0; i < playerMonster.ActiveMoves.Count; i++)
        {
            var move = playerMonster.ActiveMoves[i];
            string type = move.IsHeal ? "Heal" : move.Type.ToString();
            string power = move.IsHeal ? $"Heal: {move.HealPercent}%" : $"Snaga: {move.PowerPercent}%";
            string area = move.IsArea ? " [AREA]" : "";

            string costColor = playerMonster.CurrentStamina < move.StaminaCost ? "[NEMA STAMINE]" : $"{move.StaminaCost} Stam";
            Console.WriteLine($"{i + 1}. {move.Name} ({type}){area} - {power} | Preciznost: {move.Accuracy}% | {costColor}");
        }
        Console.WriteLine($"{playerMonster.ActiveMoves.Count + 1}. PUNI ENERGIJU (+50% stamine)");
        Console.WriteLine("0. Pobegni iz borbe");

        int choice;
        while (true)
        {
            Console.Write("> ");
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                if (choice == 0) return null;
                if (choice == playerMonster.ActiveMoves.Count + 1) return new Attack("Charge", MonsterType.Normal, 0, 100, 1, 0);
                if (choice >= 1 && choice <= playerMonster.ActiveMoves.Count)
                {
                    var selectedMove = playerMonster.ActiveMoves[choice - 1];
                    if (playerMonster.CurrentStamina >= selectedMove.StaminaCost) return selectedMove;
                    else Console.WriteLine("Nemaš dovoljno stamine za taj napad!");
                }
            }
            Console.WriteLine("Neispravan unos.");
        }
    }

    private static void ExecuteAttack(Monster attacker, List<Monster> targets, Attack move, bool isPlayerAttacking)
    {
        string attackerName = isPlayerAttacking ? attacker.Name : $"Protivnik ({attacker.Name})";

        // 1. Trošenje stamine
        attacker.UseStamina(move.StaminaCost);

        // 2. Provera preciznosti (sa modifikatorom od Blind/Daze)
        double accuracyMod = 1.0;
        var blind = attacker.ActiveStatuses.FirstOrDefault(s => s.AccuracyModifier < 0);
        if (blind != null) accuracyMod += blind.AccuracyModifier; // npr. 1.0 - 0.5 = 0.5
        if (_random.Next(1, 101) > move.Accuracy * accuracyMod)
        {
            TypeWrite($"  {attackerName} koristi {move.Name}... ali PROMAŠUJE!");
            return;
        }

        // 3. Heal napadi
        if (move.IsHeal)
        {
            int healAmount = (int)(attacker.MaxHp * (move.HealPercent / 100.0));
            attacker.CurrentHp = Math.Min(attacker.MaxHp, attacker.CurrentHp + healAmount);
            TypeWrite($"  {attackerName} koristi {move.Name} i leči sebe za {healAmount} HP!");
            return;
        }

        // 4. Štetni napadi
        var actualTargets = new List<Monster>();
        if (move.IsArea)
        {
            actualTargets = targets.Where(t => t.IsAlive).ToList();
        }
        else
        {
            var singleTarget = targets.FirstOrDefault(t => t.IsAlive);
            if (singleTarget != null) actualTargets.Add(singleTarget);
        }

        foreach (var defender in actualTargets)
        {
            if (defender == null) continue;

            // Provera ranjivosti (Soak -> +50% Electric damage)
            double typeMultiplier = TypeChart.GetMultiplier(move.Type, defender.Types);
            if (move.Type == MonsterType.Electric && defender.ActiveStatuses.Any(s => s.Name == "Soak"))
                typeMultiplier *= 1.5;

            string effectiveness = typeMultiplier > 1.0 ? " (Veoma efikasno!)" : typeMultiplier < 1.0 ? " (Nije baš efikasno...)" : "";

            double baseDamage = attacker.Atk * (move.PowerPercent / 100.0);
            int finalDamage = (int)Math.Max(1, Math.Round(baseDamage * typeMultiplier));

            defender.CurrentHp = Math.Max(0, defender.CurrentHp - finalDamage);
            TypeWrite($"  {attackerName} koristi {move.Name} na {defender.Name} i nanosi {finalDamage} štete!{effectiveness}");

            // 5. Primena statusa (ako napad ima efekat i ako je meta živa)
            if (defender.IsAlive && move.EffectToApply != null && _random.Next(1, 101) <= move.EffectChance)
            {
                ApplyStatus(defender, move.EffectToApply);
            }

            if (!defender.IsAlive) TypeWrite($"  💀 {defender.Name} je onesvešćen!");
        }
    }

    // ==========================================
    // STATUS EFEKTI
    // ==========================================

    private static void ApplyStatus(Monster target, StatusEffect effect)
    {
        var existing = target.ActiveStatuses.FirstOrDefault(s => s.Name == effect.Name);
        if (existing != null)
        {
            if (effect.IsStackable)
            {
                // Ako je stekabilno (npr. Bane), samo dodajemo novi efekat
                target.ActiveStatuses.Add(new StatusEffect(effect.Name, effect.Element, effect.Duration) { DamagePercentPerTurn = effect.DamagePercentPerTurn });
            }
            else
            {
                // Ako nije, osvežavamo trajanje
                existing.Duration = effect.Duration;
            }
        }
        else
        {
            target.ActiveStatuses.Add(new StatusEffect(effect.Name, effect.Element, effect.Duration)
            {
                DamagePercentPerTurn = effect.DamagePercentPerTurn,
                AtkModifier = effect.AtkModifier,
                SpeedModifier = effect.SpeedModifier,
                AccuracyModifier = effect.AccuracyModifier,
                SkipTurn = effect.SkipTurn,
                IsConfusion = effect.IsConfusion,
                IsStackable = effect.IsStackable
            });
            TypeWrite($"  ✨ {target.Name} je dobio status: {effect.Name}!");
        }
    }

    private static void ProcessStatusEffects(Monster monster)
    {
        for (int i = monster.ActiveStatuses.Count - 1; i >= 0; i--)
        {
            var status = monster.ActiveStatuses[i];

            // Šteta po potezu
            if (status.DamagePercentPerTurn > 0)
            {
                int damage = (int)(monster.MaxHp * (status.DamagePercentPerTurn / 100.0));
                monster.CurrentHp = Math.Max(0, monster.CurrentHp - damage);
                TypeWrite($"  {monster.Name} trpi {damage} štete od {status.Name}!");
                if (!monster.IsAlive)
                {
                    TypeWrite($"  💀 {monster.Name} je podlegao efektu {status.Name}!");
                    return;
                }
            }

            // Smanjenje stamine (Energy Leak)
            if (status.Element == MonsterType.Electric && status.Name == "Energy Leak")
            {
                int staminaLoss = (int)(monster.MaxStamina * 0.15);
                monster.CurrentStamina = Math.Max(0, monster.CurrentStamina - staminaLoss);
                TypeWrite($"  {monster.Name} gubi {staminaLoss} stamine od {status.Name}!");
            }

            status.Duration--;
            if (status.Duration <= 0)
            {
                monster.ActiveStatuses.RemoveAt(i);
            }
        }
    }

    // ==========================================
    // UI
    // ==========================================

    private static void DrawHeader(string title)
    {
        Console.WriteLine("==================================================");
        Console.WriteLine($"  {title.ToUpper()}");
        Console.WriteLine("==================================================");
    }

    private static void DrawMonsterStatus(Monster m, string prefix, bool isPlayer)
    {
        if (!m.IsAlive) { Console.WriteLine($"{prefix} {m.Name} [MRTAV]"); return; }

        string prefixText = string.IsNullOrEmpty(prefix) ? "" : $"{prefix} ";
        string typesStr = string.Join("/", m.Types);
        int xpNeeded = m.Level * 30;

        Console.WriteLine($"{prefixText}{m.Name} ({typesStr}) (Lvl {m.Level}) | XP: {m.XP}/{xpNeeded}");
        Console.WriteLine($"  HP: {m.CurrentHp}/{m.MaxHp} | ATK: {m.Atk} | SPD: {m.Speed} | Stam: {m.CurrentStamina}/{m.MaxStamina}");

        if (m.ActiveStatuses.Count > 0)
        {
            Console.Write("  Statusi: ");
            foreach (var s in m.ActiveStatuses)
                Console.Write($"[{s.Name} ({s.Duration})] ");
            Console.WriteLine();
        }
        else Console.WriteLine();
    }
}