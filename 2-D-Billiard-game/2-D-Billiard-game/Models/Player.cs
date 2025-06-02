using System;
using _2_D_Billiard_game.Balls;

namespace _2_D_Billiard_game.Models
{
    public enum BallType
    {
        None,
        Red,
        Orange
    }

    public class Player
    {
        public string Name { get; set; }
        public int TotalGames { get; set; }
        public int TotalWins { get; set; }
        public int TotalBallsPotted { get; set; }
        public int Score { get; set; }
        public BallType? AssignedBallType { get; set; }
        public bool HasPottedAllBalls { get; set; }

        public double WinRate => TotalGames > 0 ? (double)TotalWins / TotalGames * 100 : 0;

        public Player(string name)
        {
            Name = name;
            TotalGames = 0;
            TotalWins = 0;
            TotalBallsPotted = 0;
            Score = 0;
            AssignedBallType = null;
            HasPottedAllBalls = false;
        }

        public void ResetGameStats()
        {
            Score = 0;
            AssignedBallType = null;
            HasPottedAllBalls = false;
        }

        public void UpdateStats(bool won, int ballsPotted)
        {
            TotalGames++;
            if (won) TotalWins++;
            TotalBallsPotted += ballsPotted;
        }
    }
} 