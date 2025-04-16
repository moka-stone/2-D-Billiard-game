using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_D_Billiard_game
{
    public static class VectorExtensions
    {
        public static float Length(this Vector2f vector)
        {
            return MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }

        public static Vector2f Normalize(this Vector2f vector)
        {
            float length = vector.Length();
            if (length == 0) return new Vector2f(0, 0); // Избегаем деления на ноль
            else if (length > 10) return new Vector2f(5, 0);
                return new Vector2f(vector.X / length, vector.Y / length);
        }
    }
}
