using _2_D_Billiard_game.Balls;
using _2_D_Billiard_game.BilliardField;
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

namespace _2_D_Billiard_game.UI
{
    
    public class GameWindow
    {
        public RenderWindow window;
        public Field _field;
        public List<Hole> _holes;
        public List<Ball> _balls;
        public const int posx = 150;
        public const int posy = 100;

        public SFML.Graphics.Font font;
        public SFML.Graphics.Text strengthText;
        public SFML.Graphics.Text player1ScoreText;
        public SFML.Graphics.Text player2ScoreText;
        public SFML.Graphics.Text currentTurnText;
        public SFML.Graphics.Text gameStatusText;
        public Sprite backgroundSprite;
        public SoundBuffer collisionSoundBuffer;
        public Sound collisionSound;
        public Sound gameMusic;

        public float cueAngle;
        public bool isPlacingWhiteBall;
        public bool areBallsMoving;
        public Vector2f mousePosition;
        public float hitStrength;

        public Player player1;
        public Player player2;
        public GameWindow(Player player1, Player player2) 
        {
            this.player1 = player1;
            this.player2 = player2;
            LoadResources();
            InitializeNewWindow(1200,600);
            InitializeScoreDisplay();
        }
        public void InitializeNewWindow(int width, int height) 
        {
            
            _field = new Field(width,height,posx,posy);          
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

            new CueBall(-200+posx + width / 2, posy-15 + height / 2, 15),
            new RedBall(posx + width / 2, posy-15 + height / 2, 15),
            new OrangeBall((posx + width / 2)+35,(posy-15 + height / 2),15),
            new OrangeBall((posx + width / 2)+35,(posy-15 + height / 2)+35,15),
            new OrangeBall((posx + width / 2)+35,(posy-15 + height / 2)-35,15),
            new RedBall((posx + width / 2)+70,(posy-15 + height / 2)+70,15),
            new RedBall((posx + width / 2)+70,(posy-15 + height / 2)+35,15),
            new BlackBall((posx + width / 2)+70,(posy-15 + height / 2),15),
            new RedBall((posx + width / 2)+70,(posy-15 + height / 2)-35,15),
            new RedBall((posx + width / 2)+70,(posy-15 + height / 2)-70,15),
            new RedBall((posx + width / 2)+105,(posy-15 + height / 2)-90,15),
            new OrangeBall((posx + width / 2)+105,(posy-15 + height / 2)-55,15),
            new OrangeBall((posx + width / 2)+105,(posy-15 + height / 2)-20,15),
            new RedBall((posx + width / 2)+105,(posy-15 + height / 2)+15,15),
            new OrangeBall((posx + width / 2)+105,(posy-15 + height / 2)+50,15),
            new OrangeBall((posx + width / 2)+105,(posy-15 + height / 2)+90,15),

        };
        }
        private void LoadResources()
        {
            this.window = new RenderWindow(new VideoMode(1920, 1080), "Billiard Game");

            Texture backgroundTexture = new Texture("Resources/background.jpg");
            this.backgroundSprite = new Sprite(backgroundTexture);
            this.collisionSoundBuffer = new SoundBuffer("Resources/soundcollision.wav");
            this.collisionSound = new Sound(collisionSoundBuffer);
            //Font
            this.font = new SFML.Graphics.Font("Resources/shrift.ttf");
            this.strengthText = new SFML.Graphics.Text("Strength: 0", font, 20)
            {
                Position = new Vector2f(posx - 100, posy - 50),
                FillColor = new Color(0, 255, 255)
            };
            window.Closed += (sender, e) => window.Close();

            // Установка иконки
            SFML.Graphics.Image icon = new SFML.Graphics.Image("Resources/Mok2.jpg");
            window.SetIcon(icon.Size.X, icon.Size.Y, icon.Pixels);

            // Добавляем обработчик движения мыши
            window.MouseMoved += (sender, e) =>
            {
                if (isPlacingWhiteBall)
                {
                    UpdateMousePosition(new Vector2f(e.X, e.Y));
                }
            };
        }

        public void Draw(RenderWindow window)
        {
            _field.Draw(window); // Field
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
            Vector2f cueStart = _balls.OfType<CueBall>().FirstOrDefault().Shape.Position;
            cueStart.X += 15;
            cueStart.Y += 15;

            Vector2f cueEnd = cueStart + new Vector2f(300 * (float)Math.Cos((float)(Math.PI / 180) * cueAngle),
                                                          300 * (float)Math.Sin((float)(Math.PI / 180) * cueAngle));
            VertexArray cueLine = new VertexArray(PrimitiveType.Lines, 2)
            {
                [0] = new Vertex(cueStart, Color.White),
                [1] = new Vertex(cueEnd, Color.White)
            };
            window.Draw(cueLine);

        }
        public void Render()
        {
            window.Draw(backgroundSprite);
            window.Draw(player1ScoreText);
            window.Draw(player2ScoreText);
            window.Draw(currentTurnText);
            window.Draw(gameStatusText);
            Draw(window); 
            window.Display();
        }
        private void InitializeScoreDisplay()
        {
            float centerX = 650 + window.Size.X / 2;

            player1ScoreText = new SFML.Graphics.Text($"{player1.Name}: {player1.Score}", font, 24)
            {
                Position = new Vector2f(centerX - 200, 10),
                FillColor = Color.White
            };

            player2ScoreText = new SFML.Graphics.Text($"{player2.Name}: {player2.Score}", font, 24)
            {
                Position = new Vector2f(centerX + 25, 10),
                FillColor = Color.White
            };

            currentTurnText = new SFML.Graphics.Text("", font, 24)
            {
                Position = new Vector2f(centerX - 200, 40),
                FillColor = Color.Yellow
            };

            gameStatusText = new SFML.Graphics.Text("", font, 24)
            {
                Position = new Vector2f(centerX - 200, 70),
                FillColor = Color.Green
            };
        }
        public void RotateCue(float angle)
        {
            if (!areBallsMoving && !isPlacingWhiteBall)
            {
                cueAngle += angle;
            }
        }

        public void IncreaseHitStrength(float amount)
        {
            if (!areBallsMoving && !isPlacingWhiteBall)
            {
                if (hitStrength >= 2000f) { return; }
                hitStrength += amount;
            }
        }

        public void DecreaseHitStrength(float amount)
        {
            if (!areBallsMoving && !isPlacingWhiteBall)
            {
                hitStrength = Math.Max(1, hitStrength - amount);
            }
        }

        public void PerformHit()
        {
            if (areBallsMoving || isPlacingWhiteBall)
            {
                return;
            }

            Ball whiteBall = _balls.Find(b => b is CueBall);
            if (whiteBall != null)
            {
                Vector2f hitDirection = new Vector2f((float)Math.Cos((float)(Math.PI / 180) * cueAngle),
                                                    (float)Math.Sin((float)(Math.PI / 180) * cueAngle));
                hitDirection.Normalize();
                hitDirection *= 5;
                whiteBall.Velocity += hitDirection * hitStrength;             
            }
            hitStrength = 0;
        }

        public void UpdateMousePosition(Vector2f position)
        {
            mousePosition = position;
            if (isPlacingWhiteBall)
            {
                var whiteBall = _balls.OfType<CueBall>().FirstOrDefault();
                if (whiteBall != null)
                {
                    // Ограничиваем позицию белого шара пределами стола
                    float x = MathF.Max(_field.Shape.Position.X, MathF.Min(position.X - whiteBall.Shape.Radius,
                        _field.Shape.Position.X + _field.Shape.Size.X - whiteBall.Shape.Radius * 2));
                    float y = MathF.Max(_field.Shape.Position.Y, MathF.Min(position.Y - whiteBall.Shape.Radius,
                        _field.Shape.Position.Y + _field.Shape.Size.Y - whiteBall.Shape.Radius * 2));

                    whiteBall.Shape.Position = new Vector2f(x, y);
                }
            }
        }

        public void OnMouseClick()
        {
            if (isPlacingWhiteBall)
            {
                isPlacingWhiteBall = false;
            }
        }
    }
}
