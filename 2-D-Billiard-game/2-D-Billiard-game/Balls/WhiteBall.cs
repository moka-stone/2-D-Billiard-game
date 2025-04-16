using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_D_Billiard_game.Balls
{
    public class WhiteBall : Ball
    {
        public WhiteBall(float x, float y, float radius) : base(x, y, radius, Color.White)
        {
        }

        public void Hit(Vector2f direction, float force)
        {
            Velocity = direction * force;
        }
    }
}
