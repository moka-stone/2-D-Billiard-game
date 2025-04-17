using _2_D_Billiard_game.Balls;
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
        private readonly RectangleShape _shape;
        private readonly List<Hole> _holes;
        private readonly List<Ball> _balls;

        private const int posx = 150;
        private const int posy = 100;

        private float cueAngle; // Угол кийа
        private float hitStrength; // Сила удара
        private bool isSelectingStrength; // 
        private int currentWhiteBall = 1;

        private SFML.Graphics.Font font;
        private SFML.Graphics.Text strengthText;

        private SoundBuffer collisionSoundBuffer;
        private Sound collisionSound;
        private Sound gameMusic;

        Random random = new Random();

        public Field(int width, int height)
        {
            //Cue
            cueAngle = 0f;
            hitStrength = 0f;
            isSelectingStrength = false;
            //Font
            font = new SFML.Graphics.Font("Resources/shrift.ttf"); // Укажите путь к вашему шрифту
            strengthText = new SFML.Graphics.Text("Strength: 0", font, 20)
            {
                Position = new Vector2f(posx - 100, posy - 50), // Укажите позицию текста
                FillColor = new Color(0, 255, 255)
            }
            ;
            //Sound
            collisionSoundBuffer = new SoundBuffer("Resources/soundcollision.wav");
            collisionSound = new Sound(collisionSoundBuffer);
            

            _shape = new RectangleShape(new Vector2f(width, height))
            {
                FillColor = Color.Green,
                Position = new Vector2f(posx,posy)
            };

            _holes = new List<Hole>
        {
            new Hole(posx, posy, 20),   // Up Left
            new Hole(posx-20+width/2, posy, 20),   // Up Centre
            new Hole(width+posx-40, posy, 20),  // Up Right
            new Hole(posx, posy+height-40, 20),   // Down Left
            new Hole(posx-20+width/2, posy+height-40, 20),    // Down Centre
            new Hole(width+posx-40, posy+height-40, 20),  // Down Right


        };
            _balls = new List<Ball>
        {

            new WhiteBall(-50+posx + width / 2, posy-15 + height / 2, 15),
            new DefaultBall(posx + width / 2, posy-15 + height / 2, 15, new Color(0,0,255)), 
            new DefaultBall(posx+30 + width / 2, posy-45 + height / 2, 15, new Color(200,255,50)), 
            new DefaultBall(posx+30 + width / 2, posy+15 + height / 2, 15, new Color(255, 0, 0)), 
            new DefaultBall(posx+60 + width / 2, posy-75 + height / 2, 15, new Color(255, 255, 0)), 
            new DefaultBall(posx+60 + width / 2, posy+45 + height / 2, 15, new Color(0, 255, 255)), 
            new DefaultBall(posx+80 + width / 2, posy-100 + height / 2, 15, new Color(255, 0, 255)), 
            new DefaultBall(posx+70 + width / 2, posy+100 +  height / 2, 15, new Color(0,0,255)), 
            new DefaultBall(posx+200 + width / 2, posy+200 + height / 2, 15, new Color(125,0,0)), 
            new DefaultBall(posx+100 + width / 2, posy-159 + height / 2, 15, new Color(0, 125, 0)), 
            new DefaultBall(posx-200 + width / 2, posy+60 + height / 2, 15, new Color(0, 0, 125)), 
            new DefaultBall(posx + 150 + width / 2, posy-200 + height / 2, 15, new Color(0, 125, 255)), 
            new DefaultBall(posx - 100 + width / 2, posy + height / 2, 15, new Color(255, 0, 125))         
        };
        }
        public void Update(float deltaTime)
        {
            foreach (var ball in _balls)
            {
                ball.Update(deltaTime, _shape);
                //foreach (var hole in _holes) 
                //{
                    //if (ball.Shape.Position.X - hole.Shape.Position.X <= 5 && ball.Shape.Position.Y - hole.Shape.Position.Y<=5)
                    //{ _balls.Remove(ball); }
                //}
            }
            // Collision
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
            strengthText.DisplayedString = $"Strength: {hitStrength:F1}"; // update power text
        }
        private void HandleCollision(Ball ballA, Ball ballB)
        {
            // Получаем вектора скорости
            Vector2f velocityA = ballA.Velocity;
            Vector2f velocityB = ballB.Velocity;

            // Вычисляем вектор нормали
            Vector2f collisionNormal = new Vector2f(
                (ballA.Shape.Position.X + ballA.Shape.Radius) - (ballB.Shape.Position.X + ballB.Shape.Radius),
                (ballA.Shape.Position.Y + ballA.Shape.Radius) - (ballB.Shape.Position.Y + ballB.Shape.Radius)
            );

            float distance = collisionNormal.Length();
            if (distance == 0) return; // Избегаем деления на ноль

            collisionNormal = VectorExtensions.Normalize(collisionNormal);

            // Вычисляем относительную скорость
            Vector2f relativeVelocity = velocityA - velocityB;

            // Находим скалярное произведение
            float velocityAlongNormal = VectorExtensions.Dot(relativeVelocity, collisionNormal);

            // Проверяем, движутся ли шары друг к другу
            if (velocityAlongNormal > 0) return; // Если шары движутся в разные стороны, ничего не делаем

            // Рассчитываем импульс
            float coefficientOfRestitution = 0.1f; // Установите значение < 1 для учета энергии
            float impulseMagnitude = -(1 + coefficientOfRestitution) * velocityAlongNormal;

            // Обновляем скорости
            Vector2f impulse = impulseMagnitude * collisionNormal;
            ballA.Velocity += impulse; // Шар A получает импульс
            ballB.Velocity -= impulse; // Шар B получает обратный импульс

            // Перемещаем шары, чтобы избежать залипания
            float overlap = (ballA.Shape.Radius + ballB.Shape.Radius) - distance;
            if (overlap > 0)
            {
                ballA.Shape.Position += collisionNormal * (overlap / 2);
                ballB.Shape.Position -= collisionNormal * (overlap / 2);
            }
            double randomValue = random.NextDouble() * (1.05 - 0.95) + 0.95;
            collisionSound.Pitch = (float)randomValue;
            collisionSound.Play(); //Sound Collision
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(_shape); // Field
            foreach (var hole in _holes)  // Holes
            {
                hole.Draw(window);
            }
            foreach (var ball in _balls) // Balls
            {
                ball.Draw(window);
             
            }
            DrawCue(window); 
            window.Draw(strengthText); // Text
        }

        private void DrawCue(RenderWindow window)
        {
            Vector2f cueStart = _balls.OfType<WhiteBall>().FirstOrDefault().Shape.Position;
            cueStart.X += 15;
            cueStart.Y += 15;

            Vector2f cueEnd = cueStart + new Vector2f(200 * (float)Math.Cos((float)(Math.PI / 180) * cueAngle),
                                                          200 * (float)Math.Sin((float)(Math.PI / 180) * cueAngle));
                VertexArray cueLine = new VertexArray(PrimitiveType.Lines, 2)
                {
                    [0] = new Vertex(cueStart, Color.White),
                    [1] = new Vertex(cueEnd, Color.White)
                };
                window.Draw(cueLine);
            
        }

        public void RotateCue(float angle)
        {
            cueAngle += angle;
        }

        public void IncreaseHitStrength(float amount)
        {
            if (hitStrength >= 2000f) { return; }
            hitStrength += amount;
        }

        public void DecreaseHitStrength(float amount)
        {
            hitStrength = Math.Max(1, hitStrength - amount); 
        }

        public void PerformHit()
        {
            Ball whiteBall = _balls.Find(b => b is WhiteBall); // Need to change
            if (whiteBall != null)
            {
                Vector2f hitDirection = new Vector2f((float)Math.Cos((float)(Math.PI / 180) * cueAngle),
                                                      (float)Math.Sin((float)(Math.PI / 180) * cueAngle));
                hitDirection.Normalize();
                hitDirection*=5;
                whiteBall.Velocity += hitDirection * hitStrength;
            }
            hitStrength = 0; //Reset power
        }

    }
}
