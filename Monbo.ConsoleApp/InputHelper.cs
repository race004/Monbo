namespace Monbo.ConsoleApp;

public static class InputHelper
{
    // Traži od korisnika broj u određenom opsegu
    public static int GetIntInput(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int result) && result >= min && result <= max)
            {
                return result;
            }

            Console.WriteLine($"Neispravan unos. Molimo unesite broj između {min} i {max}.");
        }
    }
}