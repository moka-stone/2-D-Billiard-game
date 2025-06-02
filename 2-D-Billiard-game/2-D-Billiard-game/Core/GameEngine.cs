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

        public GameEngine(GameWindow gameWindow)
        {
            this.gameWindow = gameWindow;                   
            clock = new Clock();
            inputHandler = new InputHandler();         
                                         
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
            bool pottedOwnBall = false;

            foreach (var ball in gameWindow._balls)
            {
                ball.Update(deltaTime, gameWindow._field);

                if (ball.Velocity.X != 0 || ball.Velocity.Y != 0)
                {
                    anyBallMoving = true;
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
                        }
                        else if (ball is BlackBall)
                        {
                            //HandleBlackBallPotted();
                        }
                        else
                        {
                            if (!areBallTypesAssigned)
                            {
                                pottedOwnBall = true; // Первый забитый шар всегда считается своим
                            }
                            else if ((ball is RedBall && currentPlayer.AssignedBallType == BallType.Red) ||
                                    (ball is OrangeBall && currentPlayer.AssignedBallType == BallType.Orange))
                            {
                                pottedOwnBall = true;
                            }
                            HandleColoredBallPotted(ball);
                            ballsToRemove.Add(ball);
                        }
                    }
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
                // Первый забитый шар определяет типы для игроков
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
                BallTypeAssigned?.Invoke(currentPlayer.AssignedBallType);
                currentPlayer.Score++;
                return;
            }

            // Определяем, чей это был шар и начисляем очки
            if (ball is RedBall)
            {
                if (currentPlayer.AssignedBallType == BallType.Red)
                    currentPlayer.Score++;
                else
                    otherPlayer.Score++;
            }
            else if (ball is OrangeBall)
            {
                if (currentPlayer.AssignedBallType == BallType.Orange)
                    currentPlayer.Score++;
                else
                    otherPlayer.Score++;
            }

            CheckIfAllBallsPotted();
        }




        private void CheckIfAllBallsPotted()
        {
            int redBallsLeft = _balls.Count(b => b is RedBall);
            int orangeBallsLeft = _balls.Count(b => b is OrangeBall);

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



        private void OnTurnEnded()
        {
            player1.IsCurrentTurn = !player1.IsCurrentTurn;
            player2.IsCurrentTurn = !player2.IsCurrentTurn;
        }

        private void OnGameWon(Player winner)
        {
            isGameOver = true;
            gameStatusText.DisplayedString = $"Игра окончена! Победитель: {winner.Name}!";
        }

        private void OnBallTypeAssigned(BallType ballType)
        {
            string ballTypeStr = ballType == BallType.Red ? "красные" : "оранжевые";
            gameStatusText.DisplayedString = $"{player1.Name}: {(player1.AssignedBallType == BallType.Red ? "красные" : "оранжевые")} | " +
                                           $"{player2.Name}: {(player2.AssignedBallType == BallType.Red ? "красные" : "оранжевые")}";
        }
    }
}
