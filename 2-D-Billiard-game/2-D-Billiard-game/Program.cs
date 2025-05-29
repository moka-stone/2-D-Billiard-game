using _2_D_Billiard_game;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using _2_D_Billiard_game.BilliardField;

class Program
{
    private static void HandleKeyboardInput(Field field)
    {
        // Rotating
        if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
        {
            field.RotateCue(-0.01f); 
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
        {
            field.RotateCue(0.01f); 
        }
        // Power
        if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
        {
            field.IncreaseHitStrength(0.1f); 
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
        {
            field.DecreaseHitStrength(0.1f); 
        }
        // Hit
        if (Keyboard.IsKeyPressed(Keyboard.Key.Enter))
        {
            field.PerformHit(); 
        }
    }
    static void Main()
    {
        // Init window
        var window = new RenderWindow(new VideoMode(1920, 1080), "Billiard Game");
        var field = new Field(1200, 600);
        
        // Background
        Texture backgroundTexture = new Texture("Resources/background.jpg");
        Sprite backgroundSprite = new Sprite(backgroundTexture);

        // Icon
        Image icon = new Image("Resources/Mok2.jpg");
        window.SetIcon(icon.Size.X, icon.Size.Y, icon.Pixels);

        window.Closed += (sender, e) => window.Close();

        // Clock to update pos
        Clock clock = new Clock();

        // Music
        SoundBuffer gamsondbuf = new SoundBuffer("Resources/temporary.mp3");
        Sound gameMusic = new Sound(gamsondbuf);
        gameMusic.Volume = 5;
        gameMusic.Play();

        while (window.IsOpen)
        {
            window.DispatchEvents();

            float deltaTime = clock.Restart().AsSeconds();

            /*window.MouseButtonPressed += (sender, e) =>
            {
                if (e.Button == Mouse.Button.Left)
                {
                    field.OnMouseClick(new Vector2i(e.X, e.Y));
                }
            };*/

            HandleKeyboardInput(field);

            field.Update(deltaTime); // fieldUpdate-->BallsUpdate & CollisionCheck

            window.Draw(backgroundSprite);
            
            field.Draw(window);
           
            window.Display();

        }

    }
    
}
