using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SerializationPlugin
{
    public class JsonPlayerSerializer : ISerializer
    {
        private readonly JsonSerializerOptions _options;

        public JsonPlayerSerializer()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
        }

        public void SavePlayer(PlayerData player, string filePath)
        {
            string jsonString = JsonSerializer.Serialize(player, _options);
            File.WriteAllText(filePath, jsonString);
        }

        public PlayerData LoadPlayer(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<PlayerData>(jsonString, _options);
        }

        public List<PlayerData> LoadAllPlayers(string directoryPath)
        {
            var players = new List<PlayerData>();
            if (!Directory.Exists(directoryPath))
                return players;

            foreach (string file in Directory.GetFiles(directoryPath, "*.json"))
            {
                var player = LoadPlayer(file);
                if (player != null)
                    players.Add(player);
            }
            return players;
        }
    }
}