using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_D_Billiard_game.BilliardField
{
    public class Hole
    {
        public CircleShape Shape { get; }

        public Hole(float x, float y, float radius)
        {
            Shape = new CircleShape(radius)
            {
                Position = new Vector2f(x, y),
                FillColor = Color.Red // Цвет лунки
            };
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(Shape);
        }
    }
}
