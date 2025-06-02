using SFML.Graphics;
using SFML.System;
using SFML.Window;
using _2_D_Billiard_game.Models;
using System;

namespace _2_D_Billiard_game.UI
{
    public class StartWindow
    {
        private RenderWindow window;
        private RectangleShape player1Input;
        private RectangleShape player2Input;
        private RectangleShape startButton;
        private Text player1Text;
        private Text player2Text;
        private Text startButtonText;
        private Text player1Label;
        private Text player2Label;
        private Font font;
        private string player1Name = "";
        private string player2Name = "";
        private bool isPlayer1Selected;
        private bool isPlayer2Selected;

        public StartWindow()
        {
            window = new RenderWindow(new VideoMode(800, 600), "Billiard Game - Start");
            font = new Font("Resources/shrift.ttf");
            InitializeComponents();
            SetupEventHandlers();
        }

        private void InitializeComponents()
        {
            // Поле ввода для первого игрока
            player1Input = new RectangleShape(new Vector2f(200, 40))
            {
                Position = new Vector2f(50, 250),
                FillColor = new Color(50, 50, 50),
                OutlineColor = Color.White,
                OutlineThickness = 2
            };

            // Поле ввода для второго игрока
            player2Input = new RectangleShape(new Vector2f(200, 40))
            {
                Position = new Vector2f(550, 250),
                FillColor = new Color(50, 50, 50),
                OutlineColor = Color.White,
                OutlineThickness = 2
            };

            // Кнопка старта
            startButton = new RectangleShape(new Vector2f(200, 50))
            {
                Position = new Vector2f(300, 400),
                FillColor = new Color(0, 100, 0),
                OutlineColor = Color.White,
                OutlineThickness = 2
            };

            // Надписи
            player1Label = new Text("Player 1", font, 24)
            {
                Position = new Vector2f(50, 210),
                FillColor = Color.White
            };

            player2Label = new Text("Player 2", font, 24)
            {
                Position = new Vector2f(550, 210),
                FillColor = Color.White
            };

            player1Text = new Text("", font, 20)
            {
                Position = new Vector2f(60, 260),
                FillColor = Color.White
            };

            player2Text = new Text("", font, 20)
            {
                Position = new Vector2f(560, 260),
                FillColor = Color.White
            };

            startButtonText = new Text("Start Game", font, 24)
            {
                FillColor = Color.White
            };
            
            // Центрируем текст на кнопке
            var bounds = startButtonText.GetLocalBounds();
            startButtonText.Position = new Vector2f(
                startButton.Position.X + (startButton.Size.X - bounds.Width) / 2,
                startButton.Position.Y + (startButton.Size.Y - bounds.Height) / 2
            );
        }

        private void SetupEventHandlers()
        {
            window.Closed += (s, e) => window.Close();
            
            window.MouseButtonPressed += (s, e) =>
            {
                var mousePos = new Vector2f(e.X, e.Y);
                
                if (player1Input.GetGlobalBounds().Contains(e.X, e.Y))
                {
                    isPlayer1Selected = true;
                    isPlayer2Selected = false;
                    player1Input.OutlineColor = Color.Yellow;
                    player2Input.OutlineColor = Color.White;
                }
                else if (player2Input.GetGlobalBounds().Contains(e.X, e.Y))
                {
                    isPlayer2Selected = true;
                    isPlayer1Selected = false;
                    player2Input.OutlineColor = Color.Yellow;
                    player1Input.OutlineColor = Color.White;
                }
                else if (startButton.GetGlobalBounds().Contains(e.X, e.Y))
                {
                    if (!string.IsNullOrWhiteSpace(player1Name) && !string.IsNullOrWhiteSpace(player2Name))
                    {
                        window.Close();
                    }
                }
            };

            window.TextEntered += (s, e) =>
            {
                if (isPlayer1Selected)
                {
                    HandleTextInput(e.Unicode, ref player1Name, player1Text);
                }
                else if (isPlayer2Selected)
                {
                    HandleTextInput(e.Unicode, ref player2Name, player2Text);
                }
            };
        }

        private void HandleTextInput(string unicode, ref string playerName, Text playerText)
        {
            if (unicode == "\b" && playerName.Length > 0) // Backspace
            {
                playerName = playerName.Substring(0, playerName.Length - 1);
            }
            else if (unicode != "\b" && playerName.Length < 15) // Ограничение на длину имени
            {
                playerName += unicode;
            }
            playerText.DisplayedString = playerName;
        }

        public (Player player1, Player player2)? Show()
        {
            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear(new Color(30, 30, 30));

                window.Draw(player1Input);
                window.Draw(player2Input);
                window.Draw(startButton);
                window.Draw(player1Label);
                window.Draw(player2Label);
                window.Draw(player1Text);
                window.Draw(player2Text);
                window.Draw(startButtonText);

                window.Display();
            }

            if (!string.IsNullOrWhiteSpace(player1Name) && !string.IsNullOrWhiteSpace(player2Name))
            {
                return (new Player(player1Name), new Player(player2Name));
            }

            return null;
        }
    }
} 