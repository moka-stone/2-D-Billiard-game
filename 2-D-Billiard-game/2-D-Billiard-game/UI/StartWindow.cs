using SFML.Graphics;
using SFML.System;
using SFML.Window;
using _2_D_Billiard_game.Models;
using System;
using System.IO;
using SerializationPlugin;

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
        private Text player1Stats;
        private Text player2Stats;
        private Font font;
        private string player1Name = "";
        private string player2Name = "";
        private bool isPlayer1Selected;
        private bool isPlayer2Selected;
        private readonly SerializationManager serializationManager;

        public StartWindow()
        {
            window = new RenderWindow(new VideoMode(800, 800), "Billiard Game - Start");
            font = new Font("Resources/shrift.ttf");
            serializationManager = new SerializationManager("LocalPlayersSaves");
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
                Position = new Vector2f(300, 600),
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

            // Статистика игроков
            player1Stats = new Text("", font, 18)
            {
                Position = new Vector2f(50, 320),
                FillColor = Color.Yellow
            };

            player2Stats = new Text("", font, 18)
            {
                Position = new Vector2f(550, 320),
                FillColor = Color.Yellow
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

        private void UpdatePlayerStats(string playerName, Text statsText)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                statsText.DisplayedString = "";
                return;
            }

            var playerData = serializationManager.LoadPlayer(playerName, "xml");
            if (playerData != null)
            {
                statsText.DisplayedString = $"Игр сыграно: {playerData.TotalGames}\n" +
                                         $"Побед: {playerData.TotalWins}\n" +
                                         $"Процент побед: {playerData.WinRate:F1}%\n" +
                                         $"Шаров забито: {playerData.TotalBallsPotted}";
            }
            else
            {
                statsText.DisplayedString = "Новый игрок";
            }
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
                    UpdatePlayerStats(player1Name, player1Stats);
                }
                else if (isPlayer2Selected)
                {
                    HandleTextInput(e.Unicode, ref player2Name, player2Text);
                    UpdatePlayerStats(player2Name, player2Stats);
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
                window.Draw(player1Stats);
                window.Draw(player2Stats);
                window.Draw(startButtonText);

                window.Display();
            }

            if (!string.IsNullOrWhiteSpace(player1Name) && !string.IsNullOrWhiteSpace(player2Name))
            {
                Player player1, player2;

                var player1Data = serializationManager.LoadPlayer(player1Name, "xml");
                if (player1Data != null)
                {
                    player1 = new Player(player1Name)
                    {
                        TotalGames = player1Data.TotalGames,
                        TotalWins = player1Data.TotalWins,
                        TotalBallsPotted = player1Data.TotalBallsPotted
                    };
                }
                else
                {
                    player1 = new Player(player1Name);
                }

                var player2Data = serializationManager.LoadPlayer(player2Name, "xml");
                if (player2Data != null)
                {
                    player2 = new Player(player2Name)
                    {
                        TotalGames = player2Data.TotalGames,
                        TotalWins = player2Data.TotalWins,
                        TotalBallsPotted = player2Data.TotalBallsPotted
                    };
                }
                else
                {
                    player2 = new Player(player2Name);
                }

                player1.ResetGameStats();
                player2.ResetGameStats();
                return (player1, player2);
            }

            return null;
        }
    }
} 