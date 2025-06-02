using System.Collections.Generic;
using System.IO;

namespace SerializationPlugin
{
    public class SerializationManager
    {
        private readonly ISerializer _jsonSerializer;
        private readonly ISerializer _xmlSerializer;
        private readonly string _dataDirectory;

        public SerializationManager(string dataDirectory)
        {
            _jsonSerializer = new JsonPlayerSerializer();
            _xmlSerializer = new XmlPlayerSerializer();
            _dataDirectory = dataDirectory;
            Directory.CreateDirectory(_dataDirectory);
        }

        public void SavePlayer(PlayerData player)
        {
            _jsonSerializer.SavePlayer(player, Path.Combine(_dataDirectory, $"{player.Name}.json"));
            _xmlSerializer.SavePlayer(player, Path.Combine(_dataDirectory, $"{player.Name}.xml"));
        }

        public PlayerData LoadPlayer(string playerName, string format = "json")
        {
            string filePath = Path.Combine(_dataDirectory, $"{playerName}.{format}");
            return format.ToLower() == "json"
                ? _jsonSerializer.LoadPlayer(filePath)
                : _xmlSerializer.LoadPlayer(filePath);
        }

        public List<PlayerData> LoadAllPlayers(string format = "json")
        {
            return format.ToLower() == "json"
                ? _jsonSerializer.LoadAllPlayers(_dataDirectory)
                : _xmlSerializer.LoadAllPlayers(_dataDirectory);
        }
    }
}