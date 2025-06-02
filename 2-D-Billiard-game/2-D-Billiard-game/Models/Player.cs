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
        public int Score { get; set; }
        public bool IsCurrentTurn { get; set; }
        public BallType AssignedBallType { get; set; }
        public bool HasPottedAllBalls { get; set; }

        public Player(string name)
        {
            Name = name;
            Score = 0;
            IsCurrentTurn = false;
            AssignedBallType = BallType.None;
            HasPottedAllBalls = false;
        }
    }
} 