using System.Collections.Generic;

namespace SerializationPlugin
{
    public interface ISerializer
    {
        void SavePlayer(PlayerData player, string filePath);
        PlayerData LoadPlayer(string filePath);
        List<PlayerData> LoadAllPlayers(string directoryPath);
    }
}
