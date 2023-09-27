using SQLitePCL;
using SpellingBee;

internal class Program
{
/*
 * Program Class - SpellingBee
 * 
 * Purpose:
 * --------
 * This class serves as the entry point for the Spelling Bee game. It sets up and manages the 
 * primary game loop, accepting and handling user inputs until the game is terminated. 
 * 
 * Main Features:
 * --------------
 * - Initialize SQLitePCL: Ensures that SQLite functionalities are initialized and ready for use.
 * - Game MVC Setup: Instantiates the Model-View-Controller classes for the game.
 * - Game Loop: Continuously waits for and processes user inputs, facilitating gameplay.
 * 
 * Game Flow:
 * ----------
 * 1. SQLitePCL is initialized.
 * 2. The core game components (Model, View, Controller) are instantiated.
 * 3. The game's introduction screen is displayed.
 * 4. The program enters a persistent loop, waiting for user inputs and handling them.
 * 
 * Dependencies:
 * -------------
 * This class relies on the GameController to process user inputs and on the GameView to 
 * display feedback to the user. The GameModel contains the core game logic and data.
 * 
 * External Libraries:
 * -------------------
 * - SQLitePCL: Enables database operations, particularly for fetching word lists.
 * 
 * Usage:
 * ------
 * To start the game, execute this program. The user can then interact with the game 
 * by entering commands when prompted.
 */

    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            //Run GUI
        }
        else if (args[0].Equals("-cli"))
        {
            //Run CLI version

            //Initialize SQLitePCL
            Batteries.Init();

            GameModel model = new GameModel();
            GameView view = new GameView();
            GameController gameController = new GameController(model, view);

            //Intro Screen
            gameController.BeginScreen();

            //While loop that allows the game to keep going
            while (true)
            {
                string input = Console.ReadLine().ToLower().Trim();
                gameController.HandleCommand(input);
            }
        }
    }
}
