using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using _2_D_Billiard_game.BilliardField;
using _2_D_Billiard_game.Models;
using _2_D_Billiard_game.UI;
using _2_D_Billiard_game.Balls;
using _2_D_Billiard_game.Utils;

namespace _2_D_Billiard_game.Core
{
    public class GameEngine
    {
        private GameWindow gameWindow;
        private Clock clock;
        private InputHandler inputHandler;
        public Random random = new Random();

        private bool areBallTypesAssigned = false;
        private Player currentPlayer;
        private Player otherPlayer;
        public event Action<BallType> BallTypeAssigned;

        public GameEngine(GameWindow gameWindow)
        {
            this.gameWindow = gameWindow;                   
            clock = new Clock();
            inputHandler = new InputHandler();
            currentPlayer = gameWindow.player1;
            otherPlayer = gameWindow.player2;
            UpdateTurnDisplay();
        }

        private void UpdateTurnDisplay()
        {
            string ballTypeText = "";
            if (areBallTypesAssigned)
            {
                ballTypeText = currentPlayer.AssignedBallType == BallType.Red ? " (Red)" : " (Orange)";
            }
            gameWindow.currentTurnText.DisplayedString = $"Current Turn: {currentPlayer.Name}{ballTypeText}";
            gameWindow.currentTurnText.FillColor = Color.Yellow;
        }

        public void Run()
        {
            while (gameWindow.window.IsOpen)
            {
                float deltaTime = clock.Restart().AsSeconds();
                
                ProcessEvents();
                Update(deltaTime);
                gameWindow.Render();
            }
        }

        private void ProcessEvents()
        {
            gameWindow.window.DispatchEvents();
            inputHandler.HandleInput(gameWindow);
        }

        private void Update(float deltaTime)
        {
            if (gameWindow.isPlacingWhiteBall)
            {
                return;
            }

            List<Ball> ballsToRemove = new List<Ball>();
            bool anyBallMoving = false;
            bool anyBallPotted = false;

            foreach (var ball in gameWindow._balls)
            {
                ball.Update(deltaTime, gameWindow._field);

                if (ball.Velocity.X != 0 || ball.Velocity.Y != 0)
                {
                    anyBallMoving = true;
                    gameWindow.areBallsMoving = true;
                }

                foreach (var hole in gameWindow._holes)
                {
                    float distance = MathF.Sqrt(
                        MathF.Pow((ball.Shape.Position.X + ball.Shape.Radius) - (hole.Shape.Position.X + hole.Shape.Radius), 2) +
                        MathF.Pow((ball.Shape.Position.Y + ball.Shape.Radius) - (hole.Shape.Position.Y + hole.Shape.Radius), 2)
                    );

                    if (distance < hole.Shape.Radius)
                    {
                        anyBallPotted = true;
                        if (ball is CueBall)
                        {
                            gameWindow.isPlacingWhiteBall = true;
                            ball.Velocity = new Vector2f(0, 0);
                            ball.Shape.Position = new Vector2f(gameWindow._field.Shape.Position.X + gameWindow._field.Shape.Size.X / 4, gameWindow._field.Shape.Position.Y + gameWindow._field.Shape.Size.Y / 2);
                            SwitchTurn();
                            gameWindow.gameStatusText.DisplayedString = "White ball potted - switching turn";
                        }
                        else if (ball is BlackBall)
                        {
                            HandleBlackBallPotted();
                        }
                        else
                        {
                            if (!areBallTypesAssigned)
                            {
                                gameWindow.pottedOwnBallThisTurn = true;
                                gameWindow.gameStatusText.DisplayedString = "First ball potted - assigning types";
                            }
                            else if ((ball is RedBall && currentPlayer.AssignedBallType == BallType.Red) ||
                                    (ball is OrangeBall && currentPlayer.AssignedBallType == BallType.Orange))
                            {
                                gameWindow.pottedOwnBallThisTurn = true;
                                gameWindow.gameStatusText.DisplayedString = "Potted own ball - keeping turn";
                            }
                            HandleColoredBallPotted(ball);
                            ballsToRemove.Add(ball);
                        }
                    }
                }
            }

            // Проверяем окончание движения шаров
            if (!anyBallMoving && gameWindow.areBallsMoving)
            {
                gameWindow.areBallsMoving = false;

                // Проверяем был ли сделан удар
                if (gameWindow.shotMade)
                {
                    // Если шары не назначены и не было забито шаров
                    if (!areBallTypesAssigned && !anyBallPotted)
                    {
                        SwitchTurn();
                        gameWindow.gameStatusText.DisplayedString = "No balls potted - switching turn (types not assigned)";
                    }
                    // Если шары назначены и не было забито своих шаров за весь удар
                    else if (areBallTypesAssigned && !gameWindow.pottedOwnBallThisTurn)
                    {
                        SwitchTurn();
                        gameWindow.gameStatusText.DisplayedString = "No own balls potted - switching turn";
                    }
                    
                    // Сбрасываем флаг удара
                    gameWindow.shotMade = false;
                }
            }

            foreach (var ball in ballsToRemove)
            {
                gameWindow._balls.Remove(ball);
            }

            // Collision handling
            for (int i = 0; i < gameWindow._balls.Count; i++)
            {
                for (int j = i + 1; j < gameWindow._balls.Count; j++)
                {
                    if (gameWindow._balls[i].CheckCollision(gameWindow._balls[j]))
                    {
                        HandleCollision(gameWindow._balls[i], gameWindow._balls[j]);
                    }
                }
            }
            gameWindow.strengthText.DisplayedString = $"Strength: {gameWindow.hitStrength:F1}";
            UpdateScoreDisplay();
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
            gameWindow.collisionSound.Pitch = (float)randomValue;
            gameWindow.collisionSound.Play(); //Sound Collision
        }

        private void HandleColoredBallPotted(Ball ball)
        {
            if (!areBallTypesAssigned)
            {
                if (ball is RedBall)
                {
                    currentPlayer.AssignedBallType = BallType.Red;
                    otherPlayer.AssignedBallType = BallType.Orange;
                }
                else if (ball is OrangeBall)
                {
                    currentPlayer.AssignedBallType = BallType.Orange;
                    otherPlayer.AssignedBallType = BallType.Red;
                }
                areBallTypesAssigned = true;
                BallTypeAssigned?.Invoke(currentPlayer.AssignedBallType.Value);
                currentPlayer.Score++;
                UpdateTurnDisplay();
                return;
            }

            if (ball is RedBall)
            {
                if (currentPlayer.AssignedBallType == BallType.Red)
                {
                    currentPlayer.Score++;
                }
                else
                {
                    otherPlayer.Score++;
                }
            }
            else if (ball is OrangeBall)
            {
                if (currentPlayer.AssignedBallType == BallType.Orange)
                {
                    currentPlayer.Score++;
                }
                else
                {
                    otherPlayer.Score++;
                }
            }

            CheckIfAllBallsPotted();
        }

        private void HandleBlackBallPotted()
        {
            bool currentPlayerWins = false;
            
            if (currentPlayer.Score == 7)
            {
                currentPlayerWins = true;
            }

            
            currentPlayer.UpdateStats(currentPlayerWins, currentPlayer.Score);
            otherPlayer.UpdateStats(!currentPlayerWins, otherPlayer.Score);
            gameWindow.SavePlayerStats(currentPlayer);
            gameWindow.SavePlayerStats(otherPlayer);

            if (currentPlayerWins)
            {
                gameWindow.gameStatusText.DisplayedString = $"{currentPlayer.Name} wins!";
            }
            else
            {
                gameWindow.gameStatusText.DisplayedString = $"{otherPlayer.Name} wins!";
            }
            gameWindow.Render();          
            Thread.Sleep(3000);
            gameWindow.window.Close();
        }

        private void CheckIfAllBallsPotted()
        {
            int redBallsLeft = gameWindow._balls.Count(b => b is RedBall);
            int orangeBallsLeft = gameWindow._balls.Count(b => b is OrangeBall);

            if (currentPlayer.AssignedBallType == BallType.Red && redBallsLeft == 0)
            {
                currentPlayer.HasPottedAllBalls = true;
            }
            else if (currentPlayer.AssignedBallType == BallType.Orange && orangeBallsLeft == 0)
            {
                currentPlayer.HasPottedAllBalls = true;
            }

            if (otherPlayer.AssignedBallType == BallType.Red && redBallsLeft == 0)
            {
                otherPlayer.HasPottedAllBalls = true;
            }
            else if (otherPlayer.AssignedBallType == BallType.Orange && orangeBallsLeft == 0)
            {
                otherPlayer.HasPottedAllBalls = true;
            }
        }

        private void SwitchTurn()
        {
            var temp = currentPlayer;
            currentPlayer = otherPlayer;
            otherPlayer = temp;
            UpdateTurnDisplay();
            // Добавляем статус в gameStatusText для отладки
            gameWindow.gameStatusText.DisplayedString = $"Turn switched to: {currentPlayer.Name}";
        }

        private void UpdateScoreDisplay()
        {
            gameWindow.player1ScoreText.DisplayedString = $"{gameWindow.player1.Name}: {gameWindow.player1.Score}";
            gameWindow.player2ScoreText.DisplayedString = $"{gameWindow.player2.Name}: {gameWindow.player2.Score}";
        }
    }
}
