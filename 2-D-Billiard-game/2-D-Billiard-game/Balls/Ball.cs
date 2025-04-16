using SFML.Graphics;
using SFML.System;
using _2_D_Billiard_game.BilliardField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_D_Billiard_game.Balls
{
    public abstract class Ball
    {
        public CircleShape Shape { get; }
        public Vector2f Velocity { get; set; }
        private const float Friction = 0.9994f; // Коэффициент затухания (0 < Friction < 1)

        protected Ball(float x, float y, float radius, Color color)
        {
            Shape = new CircleShape(radius)
            {
                Position = new Vector2f(x, y),
                FillColor = color
            };
            Velocity = new Vector2f(0, 0);
        }

        public virtual void Update(float deltaTime, RectangleShape fieldShape)
        {
            Shape.Position += Velocity * deltaTime;
            // Применение фрикции
            Velocity *= Friction;

            // Проверка на столкновение с границами стола
            if (Shape.Position.X < fieldShape.Position.X)
            {
                Shape.Position = new Vector2f(fieldShape.Position.X, Shape.Position.Y); // Возврат в предел
                Velocity = new Vector2f(-Velocity.X, Velocity.Y); // Отскок по X
            }
            else if (Shape.Position.X + Shape.Radius * 2 > fieldShape.Position.X + fieldShape.Size.X)
            {
                Shape.Position = new Vector2f(fieldShape.Position.X + fieldShape.Size.X - Shape.Radius * 2, Shape.Position.Y); // Возврат в предел
                Velocity = new Vector2f(-Velocity.X, Velocity.Y); // Отскок по X
            }

            if (Shape.Position.Y < fieldShape.Position.Y)
            {
                Shape.Position = new Vector2f(Shape.Position.X, fieldShape.Position.Y); // Возврат в предел
                Velocity = new Vector2f(Velocity.X, -Velocity.Y); // Отскок по Y
            }
            else if (Shape.Position.Y + Shape.Radius * 2 > fieldShape.Position.Y + fieldShape.Size.Y)
            {
                Shape.Position = new Vector2f(Shape.Position.X, fieldShape.Position.Y + fieldShape.Size.Y - Shape.Radius * 2); // Возврат в предел
                Velocity = new Vector2f(Velocity.X, -Velocity.Y); // Отскок по Y
            }


            if (Velocity.X * Velocity.X + Velocity.Y * Velocity.Y < 0.01f)
            {
                Velocity = new Vector2f(0, 0);
            }
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(Shape);
        }

        public bool CheckCollision(Ball other)
        {
            float distance = MathF.Sqrt(
                (Shape.Position.X - other.Shape.Position.X) * (Shape.Position.X - other.Shape.Position.X) +
                (Shape.Position.Y - other.Shape.Position.Y) * (Shape.Position.Y - other.Shape.Position.Y)
            );

            return distance < (Shape.Radius + other.Shape.Radius);
        }
    }
}

