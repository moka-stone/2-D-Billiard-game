using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SerializationPlugin
{
    public class XmlPlayerSerializer : ISerializer
    {
        private readonly XmlSerializer _serializer;

        public XmlPlayerSerializer()
        {
            _serializer = new XmlSerializer(typeof(PlayerData));
        }

        public void SavePlayer(PlayerData player, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                _serializer.Serialize(writer, player);
            }
        }

        public PlayerData LoadPlayer(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            using (var reader = new StreamReader(filePath))
            {
                return (PlayerData)_serializer.Deserialize(reader);
            }
        }

        public List<PlayerData> LoadAllPlayers(string directoryPath)
        {
            var players = new List<PlayerData>();
            if (!Directory.Exists(directoryPath))
                return players;

            foreach (string file in Directory.GetFiles(directoryPath, "*.xml"))
            {
                var player = LoadPlayer(file);
                if (player != null)
                    players.Add(player);
            }
            return players;
        }
    }
}