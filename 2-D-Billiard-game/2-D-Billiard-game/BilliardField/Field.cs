using _2_D_Billiard_game.Balls;
using _2_D_Billiard_game.Models;
using _2_D_Billiard_game.Utils;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace _2_D_Billiard_game.BilliardField
{
    public class Field
    {
        public RectangleShape Shape { get; }
        public Field(int width, int height,int posx, int posy)
        {
            Shape = new RectangleShape(new Vector2f(width, height))
            {
                FillColor = Color.Green,
                Position = new Vector2f(posx, posy)
            };
        }
        public void Draw(RenderWindow window) // Draw
        {
            window.Draw(Shape);
        }
        
        
    }
}
