using System;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace SerializationPlugin
{
    [Serializable]
    public class PlayerData
    {
        public string Name { get; set; }
        public int TotalGames { get; set; }
        public int TotalWins { get; set; }
        public int TotalBallsPotted { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public double WinRate => TotalGames > 0 ? (double)TotalWins / TotalGames * 100 : 0;

        public PlayerData() { }

        public PlayerData(string name)
        {
            Name = name;
            TotalGames = 0;
            TotalWins = 0;
            TotalBallsPotted = 0;
        }
    }
}