using System.Text.Json;
using System.Text.Json.Serialization;

namespace Monbo.Core.Save;

public static class SaveManager
{
    private static readonly string SaveFilePath = "saves.json";

    // Pomoćna klasa koja predstavlja jedan slot u fajlu
    public class SaveSlot
    {
        public string Name { get; set; } = "";
        public GameState State { get; set; } = new();
    }

    // Čuva celu listu slotova u fajl
    public static void SaveSlots(List<SaveSlot> slots)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.Preserve // Sprečava greške sa kružnim referencama
        };

        string json = JsonSerializer.Serialize(slots, options);
        File.WriteAllText(SaveFilePath, json);
    }

    // Učitava listu slotova iz fajla
    public static List<SaveSlot> LoadSlots()
    {
        if (!File.Exists(SaveFilePath))
        {
            // Ako fajl ne postoji, vraćamo 4 prazna slota
            return new List<SaveSlot>
            {
                new SaveSlot { Name = "Prazan slot 1" },
                new SaveSlot { Name = "Prazan slot 2" },
                new SaveSlot { Name = "Prazan slot 3" },
                new SaveSlot { Name = "Prazan slot 4" }
            };
        }

        string json = File.ReadAllText(SaveFilePath);
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve
        };

        return JsonSerializer.Deserialize<List<SaveSlot>>(json, options) ?? new List<SaveSlot>();
    }
}