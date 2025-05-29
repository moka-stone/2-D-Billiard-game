using System.Numerics;

namespace _2_D_Billiard_game.Domain.Models
{
    public class WhiteBall : Ball
    {
        public WhiteBall(float x, float y, float radius) : base(x, y, radius)
        {
        }

        public void ApplyForce(Vector2 force)
        {
            Velocity += force;
        }
    }
} 