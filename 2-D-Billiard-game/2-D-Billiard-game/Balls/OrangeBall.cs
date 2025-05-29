using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_D_Billiard_game.Balls
{
    public class OrangeBall : Ball
    {
        public OrangeBall(float x, float y, float radius) : base(x, y, radius, new Color(255, 165, 0))
        {
        }
    }
} 