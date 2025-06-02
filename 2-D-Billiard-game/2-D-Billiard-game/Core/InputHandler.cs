using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.System;
using _2_D_Billiard_game.BilliardField;
using _2_D_Billiard_game.UI;

namespace _2_D_Billiard_game.Core
{
    public class InputHandler
    {
        public void HandleInput(GameWindow gwindow)
        {
            if (!gwindow.isPlacingWhiteBall)
            {
                HandleKeyboardInput(gwindow);
            }
            HandleMouseInput(gwindow);
        }

        private void HandleMouseInput(GameWindow gwindow)
        {
            // Обновляем позицию мыши
            Vector2i mousePosition = Mouse.GetPosition();
            gwindow.UpdateMousePosition(new Vector2f(mousePosition.X, mousePosition.Y));

            // Обрабатываем клик мыши
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                gwindow.OnMouseClick();
            }
        }

        private void HandleKeyboardInput(GameWindow gwindow)
        {
            // Вращение
            if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
            {
                gwindow.RotateCue(-0.01f);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
            {
                gwindow.RotateCue(0.01f);
            }
            
            // Сила удара
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
            {
                gwindow.IncreaseHitStrength(0.1f);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
            {
                gwindow.DecreaseHitStrength(0.1f);
            }
            
            // Удар
            if (Keyboard.IsKeyPressed(Keyboard.Key.Enter))
            {
                gwindow.PerformHit();
            }
        }
    }
}
