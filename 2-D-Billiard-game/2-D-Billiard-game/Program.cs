using _2_D_Billiard_game.Core;
using _2_D_Billiard_game.UI;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using _2_D_Billiard_game.BilliardField;

class Program
{
    public static void MainMusic() 
    {
        SoundBuffer gamesondbufer = new SoundBuffer("Resources/temporary.mp3");
        Sound gameMusic = new Sound(gamesondbufer);
        gameMusic.Volume = 5;
        gameMusic.Play();
    }
    static void Main()
    {
        var startWindow = new StartWindow();
        var players = startWindow.Show();

        if (players.HasValue)
        {
            var (player1, player2) = players.Value;
            var gameWindow = new GameWindow(player1, player2);
            var gameEngine = new GameEngine(gameWindow);          
            gameEngine.Run();
            MainMusic();
        }
    }
    
}
