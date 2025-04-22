using System;

class Program
{
    // Variables to determine the size of the Terminal window
    static int windowWidth = Console.WindowWidth;
    static int windowHeight = Console.WindowHeight;
    
    // Variables to track the locations of the player and food
    static int playerX = 0;
    static int playerY = 0;
    static int foodX = 0;
    static int foodY = 0;
    
    // Arrays for player and food appearances
    static string[] states = { "('-')", "(^-^)", "(X_X)" };
    static string[] foods = { "@@@@@", "$$$$$", "#####" };
    
    // Variables to track the current player and food appearance
    static string playerState = states[0];
    static string food = foods[0];
    
    static void Main(string[] args)
    {
        Setup();
        
        // Main game loop
        while (true)
        {
            // Get player input
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            
            // Check if window was resized
            if (TerminalResized())
            {
                Console.Clear();
                Console.WriteLine("Console was resized. Program exiting.");
                return;
            }
            
            // Check if player should freeze
            if (ShouldFreeze())
            {
                FreezePlayer();
            }
            
            // Move the player based on input - with optional parameter for non-directional keys and speed boost
            bool exitOnNonDirectional = true; // Set to false to disable this feature
            int speedBoost = 3; // Set to 0 to disable increased speed
            if (!Move(keyInfo.Key, exitOnNonDirectional, speedBoost))
            {
                return; // Exit the program if Move returned false (non-directional key was pressed)
            }
            
            // Check if player has consumed food
            if (HasConsumedFood())
            {
                ChangePlayerState();
                ShowFood();
            }
        }
    }
    
    // Method to check if the terminal window was resized
    static bool TerminalResized()
    {
        return Console.WindowWidth != windowWidth || Console.WindowHeight != windowHeight;
    }
    
    // Method to display random food at a random location
    static void ShowFood()
    {
        // Clear the current food
        Console.SetCursorPosition(foodX, foodY);
        for (int i = 0; i < food.Length; i++)
        {
            Console.Write(" ");
        }
        
        // Select a random food and position
        Random random = new Random();
        food = foods[random.Next(0, foods.Length)];
        
        foodX = random.Next(0, windowWidth - food.Length);
        foodY = random.Next(0, windowHeight - 1);
        
        // Display the new food
        Console.SetCursorPosition(foodX, foodY);
        Console.Write(food);
        
        // Reset cursor position
        Console.SetCursorPosition(0, 0);
    }
    
    // Method that changes player appearance to match the food consumed
    static void ChangePlayerState()
    {
        for (int i = 0; i < foods.Length; i++)
        {
            if (food == foods[i])
            {
                playerState = states[i];
                break;
            }
        }
    }
    
    // Method that temporarily freezes player movement
    static void FreezePlayer()
    {
        Console.SetCursorPosition(playerX, playerY);
        Console.Write(playerState);
        System.Threading.Thread.Sleep(1000); // Freeze for 1 second
    }
    
    // Method that moves the player according to directional input
    // Returns false if a non-directional key was pressed and exitOnNonDirectional is true
    static bool Move(ConsoleKey key, bool exitOnNonDirectional = false, int speedBoost = 0)
    {
        // Clear current player position
        Console.SetCursorPosition(playerX, playerY);
        for (int i = 0; i < playerState.Length; i++)
        {
            Console.Write(" ");
        }
        
        int moveAmount = 1;
        // Apply speed boost if player has the happy face
        if (ShouldSpeedUp() && speedBoost > 0)
        {
            moveAmount = speedBoost;
        }
        
        // Update player position based on key
        switch (key)
        {
            case ConsoleKey.UpArrow:
                playerY = Math.Max(0, playerY - 1);
                break;
            case ConsoleKey.DownArrow:
                playerY = Math.Min(windowHeight - 1, playerY + 1);
                break;
            case ConsoleKey.LeftArrow:
                playerX = Math.Max(0, playerX - moveAmount);
                break;
            case ConsoleKey.RightArrow:
                playerX = Math.Min(windowWidth - playerState.Length, playerX + moveAmount);
                break;
            default:
                // Non-directional key was pressed
                if (exitOnNonDirectional)
                {
                    return false;
                }
                break;
        }
        
        // Draw player at new position
        Console.SetCursorPosition(playerX, playerY);
        Console.Write(playerState);
        
        return true;
    }
    
    // Method that sets up the initial game state
    static void Setup()
    {
        Console.Clear();
        Console.CursorVisible = false;
        
        // Initialize window dimensions
        windowWidth = Console.WindowWidth;
        windowHeight = Console.WindowHeight;
        
        // Set initial player position to the center of the window
        playerX = windowWidth / 2;
        playerY = windowHeight / 2;
        
        // Display the player
        Console.SetCursorPosition(playerX, playerY);
        Console.Write(playerState);
        
        // Display initial food
        ShowFood();
    }
    
    // Method to determine if player consumed food
    static bool HasConsumedFood()
    {
        // Check if player position overlaps with food position
        return (playerX <= foodX + food.Length && playerX + playerState.Length >= foodX) &&
               (playerY == foodY);
    }
    
    // Method to check if player should freeze
    static bool ShouldFreeze()
    {
        // Player should freeze when appearance is (X_X)
        return playerState == states[2]; // states[2] is "(X_X)"
    }
    
    // Method to check if player should have increased speed
    static bool ShouldSpeedUp()
    {
        // Player should speed up when appearance is (^-^)
        return playerState == states[1]; // states[1] is "(^-^)"
    }
}
