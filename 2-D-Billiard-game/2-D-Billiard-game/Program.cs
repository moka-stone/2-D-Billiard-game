using _2_D_Billiard_game;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using _2_D_Billiard_game.BilliardField;

class Program
{
    static void Main()
    {
        // Создание окна
        var window = new RenderWindow(new VideoMode(1500, 800), "Billiard Game");
        var field = new Field(1200, 600);
        
        window.Closed += (sender, e) => window.Close();
        // Часы
        Clock clock = new Clock();
        // Основной цикл

        while (window.IsOpen)
        {
            // Обработка событий
            window.DispatchEvents();

            float deltaTime = clock.Restart().AsSeconds();

            window.MouseButtonPressed += (sender, e) =>
            {
                if (e.Button == Mouse.Button.Left)
                {
                    field.OnMouseClick(new Vector2i(e.X, e.Y));
                }
            };


            field.Update(deltaTime);

            // Очистка окна
            window.Clear(Color.Black);

            field.Draw(window);

            // Отображение содержимого окна
            window.Display();



        }
    }
}
