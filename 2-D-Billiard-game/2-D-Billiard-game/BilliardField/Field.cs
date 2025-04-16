using _2_D_Billiard_game.Balls;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_D_Billiard_game.BilliardField
{
    public class Field
    {
        private readonly RectangleShape _shape;
        private readonly List<Hole> _holes;
        private readonly List<Ball> _balls;
        private int currentWhiteBall = 1;
        private const int posx = 150;
        private const int posy = 100;

        public Field(int width, int height)
        {

            _shape = new RectangleShape(new Vector2f(width, height))
            {
                FillColor = Color.Green,
                Position = new Vector2f(posx,posy)
            };

            _holes = new List<Hole>
        {
            new Hole(posx, posy, 20),   // Верхний левый угол
            new Hole(posx-20+width/2, posy, 20),   // Верхний центр
            new Hole(width+posx-40, posy, 20),  // Верхний правый угол
            new Hole(posx, posy+height-40, 20),   // Нижний левый угол
            new Hole(posx-20+width/2, posy+height-40, 20),    // Нижний центр
            new Hole(width+posx-40, posy+height-40, 20),  // Нижний правый угол


        };
            _balls = new List<Ball>
        {
            new DefaultBall(posx + width / 2, posy + height / 2, 15, Color.Black), // Пример белого шара
            new WhiteBall(-50+posx + width / 2, posy + height / 2, 15),
            new DefaultBall(posx + width / 2, posy-100 + height / 2, 15, Color.Red), // Пример белого шара
            new DefaultBall(posx + width / 2, posy+100 + height / 2, 15, Color.Yellow), // Пример белого шара
            // Можно добавить больше шаров
        };
        }
        public void Update(float deltaTime)
        {
            foreach (var ball in _balls)
            {
                ball.Update(deltaTime, _shape); // Передаем границы стола
            }
            // Проверка столкновений между шарами
            for (int i = 0; i < _balls.Count; i++)
            {
                for (int j = i + 1; j < _balls.Count; j++)
                {
                    if (_balls[i].CheckCollision(_balls[j]))
                    {
                        HandleCollision(_balls[i], _balls[j]);
                    }
                }
            }
        }
        private void HandleCollision(Ball ballA, Ball ballB)
        {
            // Получаем вектора скорости
            Vector2f velocityA = ballA.Velocity;
            Vector2f velocityB = ballB.Velocity;

            // Простая физика отскока
            Vector2f newVelocityA = new Vector2f(velocityB.X, velocityB.Y);
            Vector2f newVelocityB = new Vector2f(velocityA.X, velocityA.Y);

            // Обновляем скорости
            ballA.Velocity = newVelocityA;
            ballB.Velocity = newVelocityB;

            // Перемещаем шары, чтобы избежать залипания
            Vector2f overlap = (ballA.Shape.Position - ballB.Shape.Position);
            float distance = MathF.Sqrt(overlap.X * overlap.X + overlap.Y * overlap.Y);
            float overlapAmount = (ballA.Shape.Radius + ballB.Shape.Radius) - distance;

            if (distance > 0)
            {
                overlap /= distance; // Нормализация
                ballA.Shape.Position += overlap * (overlapAmount / 2);
                ballB.Shape.Position -= overlap * (overlapAmount / 2);
            }
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(_shape);
            foreach (var hole in _holes)
            {
                hole.Draw(window);
            }
            foreach (var ball in _balls)
            {
                ball.Draw(window);
            }
        }
        public void WhiteHit(Vector2f direction, float force) 
        {
            var whiteBall = _balls.OfType<WhiteBall>().FirstOrDefault();
            var whiteBall2 = _balls.OfType<WhiteBall>().LastOrDefault();
            if (whiteBall != null)
            {
                whiteBall.Hit(direction, force);
                Vector2f direction2 = new Vector2f(1, 0); // Направление удара
                float force2 = 100f; // Сила удара
                whiteBall2.Hit(direction2, force2);
            }
        }
        public void OnMouseClick(Vector2i mousePosition)
        {
            var whiteBall = _balls.OfType<WhiteBall>().FirstOrDefault();
            if (whiteBall != null)
            {
                // Получаем позицию белого шара
                Vector2f ballPosition = whiteBall.Shape.Position + new Vector2f(whiteBall.Shape.Radius, whiteBall.Shape.Radius);
                Vector2f direction = new Vector2f(mousePosition.X, mousePosition.Y) - ballPosition;

                // Вычисляем силу удара
                float force = MathF.Min(direction.Length() / 10f, 500f); // Примерное ограничение по силе

                // Нормализуем вектор направления и применяем силу
                direction = direction.Normalize() * force;
                whiteBall.Velocity = direction;
            }
        }
    }
}
