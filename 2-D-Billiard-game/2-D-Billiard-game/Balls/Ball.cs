using SFML.Graphics;
using SFML.System;
using _2_D_Billiard_game.BilliardField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2_D_Billiard_game.Utils;

namespace _2_D_Billiard_game.Balls
{
    public abstract class Ball
    {
        public CircleShape Shape { get; }
        public Vector2f Velocity { get; set; }
        private const float Friction = 0.9994f; // 9994f
        private const float Friction2 = 0.00028f; // 9994f

        protected Ball(float x, float y, float radius, Color color) // Init params
        {
            Shape = new CircleShape(radius)
            {
                Position = new Vector2f(x, y),
                FillColor = color
            };
            Velocity = new Vector2f(0, 0);
        }

        public virtual void Update(float deltaTime, Field fieldShape)
        {
            Shape.Position += Velocity * deltaTime; // Update position, distance=speed*time          
            float velo = VectorExtensions.Length(Velocity);
            velo -= velo*Friction2;
            Velocity = VectorExtensions.Normalize(Velocity) * velo;
            //Velocity *= Friction;// Friction

            // Left and right
            if (Shape.Position.X < fieldShape.Shape.Position.X)
            {
                Shape.Position = new Vector2f(fieldShape.Shape.Position.X, Shape.Position.Y);
                Velocity = new Vector2f(-Velocity.X, Velocity.Y); 
            }
            else if (Shape.Position.X + Shape.Radius * 2 > fieldShape.Shape.Position.X + fieldShape.Shape.Size.X)
            {
                Shape.Position = new Vector2f(fieldShape.Shape.Position.X + fieldShape.Shape.Size.X - Shape.Radius * 2, Shape.Position.Y); 
                Velocity = new Vector2f(-Velocity.X, Velocity.Y); 
            }
            // Up and down
            if (Shape.Position.Y < fieldShape.Shape.Position.Y)
            {
                Shape.Position = new Vector2f(Shape.Position.X, fieldShape.Shape.Position.Y);
                Velocity = new Vector2f(Velocity.X, -Velocity.Y); 
            }
            else if (Shape.Position.Y + Shape.Radius * 2 > fieldShape.Shape.Position.Y + fieldShape.Shape.Size.Y)
            {
                Shape.Position = new Vector2f(Shape.Position.X, fieldShape.Shape.Position.Y + fieldShape.Shape.Size.Y - Shape.Radius * 2); 
                Velocity = new Vector2f(Velocity.X, -Velocity.Y);
            }
 
            // Friction check
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

            return distance <= (Shape.Radius + other.Shape.Radius);
        }
    }
}

