using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

class Player
{
    // Active Stats and Are Being Counted Towards What The Player Can Do
    public static int AppliedMoney { get; set; } = 5;
    public static int AppliedGlory { get; set; } = 1;
    public static int AppliedAdventure { get; set; } = 1;

    // The held stats, that can be used to increase Applied Stats after a wave
    public static int HeldMoney { get; set; } = 0;
    public static int HeldGlory { get; set; } = 0;
    public static int HeldAdventure { get; set; } = 0;

    // The Choice of Multiplier used to determine mutliply what held stats they gain
    public static bool MoneyMultiplierActive { get; set; } = false;
    public static bool GloryMultiplierActive { get; set; } = false;
    public static bool AdventureMultiplierActive { get; set; } = false;

    public static int EnemiesSlain { get; set; } = 0;

    public static int CurrentPlayerHealth;
    public static int TotalPlayerHealth;
    public static int DamageTaken { get; set; } = 0;

    public static void CalculatePlayerInitialHealth()
    {
        TotalPlayerHealth = AppliedMoney * 10;
        CurrentPlayerHealth = TotalPlayerHealth;
    }

    public static void CalculatePlayerIncreasedHealth()
    {
        TotalPlayerHealth = AppliedMoney * 10;
    }

    public static void CalculatePlayerHealth()
    {
        CurrentPlayerHealth -= DamageTaken;
        CurrentPlayerHealth = Math.Max(CurrentPlayerHealth, 0);
    }

    public static void TakeDamage(int damageAmount)
    {
        DamageTaken += damageAmount;
        CalculatePlayerHealth();
    }

    public static void ResetPlayerStats()
    {
        AppliedMoney = 5;
        AppliedGlory = 1;
        AppliedAdventure = 1;
        HeldMoney = 0;
        HeldGlory = 0;
        HeldAdventure = 0;
        MoneyMultiplierActive = false;
        GloryMultiplierActive = false;
        AdventureMultiplierActive = false;
        EnemiesSlain = 0;
        DamageTaken = 0;
        CalculatePlayerInitialHealth(); // Recalculate player's health 
    }

    public static void GainRandomStatPoint()
    {
        Random rand = new Random();
        int randomNumber = rand.Next(1, 4); 

        switch (randomNumber)
        {
            case 1:
                HeldMoney += MoneyMultiplierActive ? 2 : 1;
                break;
            case 2:
                HeldGlory += GloryMultiplierActive ? 2 : 1; 
                break;
            case 3:
                HeldAdventure += AdventureMultiplierActive ? 2 : 1;
                break;
            default:
                break;
        }
    }

    public static void ApplyStatPoints()
    {
        AppliedMoney += HeldMoney;
        AppliedGlory += HeldGlory;
        AppliedAdventure += HeldAdventure;
        CalculatePlayerIncreasedHealth();
    }

    public static void DisplayStats(OpeningDecisions openingDecisions)
    {
        Console.Clear();
        
        Console.WriteLine($"Player Name: {openingDecisions.GetPlayerName()}");
        Console.WriteLine($"Health: {TotalPlayerHealth} / {CurrentPlayerHealth}");
        Console.WriteLine($"Money: {AppliedMoney}");
        Console.WriteLine($"Glory: {AppliedGlory}");
        Console.WriteLine($"Adventure: {AppliedAdventure}"); 
        Console.WriteLine($"Enemies Slain: {EnemiesSlain} \n");

        Console.WriteLine($"{openingDecisions.GetPlayerName()}s Gathered Stats:");
        Console.WriteLine($"Money: {HeldMoney}");
        Console.WriteLine($"Glory: {HeldGlory}");
        Console.WriteLine($"Adventure: {HeldAdventure} \n"); 
    }
    
    public static void DisplayHealth()
    {
        CalculatePlayerHealth(); 
        Console.WriteLine($"Current Health: {CurrentPlayerHealth}/{TotalPlayerHealth}");
    }

}

class Attacks
{
    private int standardAttackCooldown = 0;
    private int heavyAttackCooldown = 0;
    private int frenzyAttackCooldown = 0;
    private int rangedAttackCooldown = 0;
    private int strengthBonus = 0;

    public void StandardAttack(World map)
    {
        if (standardAttackCooldown == 0)
        {
            DealDamageToEnemies(map, 5);
            standardAttackCooldown = 3;
        }
    }

    public void HeavyAttack(World map)
    {
        if (heavyAttackCooldown == 0)
        {
            DealDamageToEnemies(map, 10);
            heavyAttackCooldown = 5;
        }
    }

    public void FrenzyAttack(World map)
    {
        if (frenzyAttackCooldown == 0)
        {
            // If Adventuere = 10, then farther reach
            if (Player.AppliedAdventure == 10)
            {
                // If Adventure = 20, then more damage
                if (Player.AppliedAdventure == 20)
                {
                    // If Adventure = 30, then less cooldown
                    if (Player.AppliedAdventure == 30)
                    {
                        DealDamageToEnemies(map, 25, 6);
                        frenzyAttackCooldown = 3;
                    }
                }
                else
                {
                    DealDamageToEnemies(map, 15, 6);
                    frenzyAttackCooldown = 5;
                }
            }
            else 
            {
                DealDamageToEnemies(map, 15, 4);
                frenzyAttackCooldown = 3;
            }
        }
    }

    public void RangedAttack(World map)
    {
        if (rangedAttackCooldown == 0)
        {
            // Create a list to store new projectiles
            List<Projectile> newProjectiles = new List<Projectile>();

            foreach (Entity entity in map.entities)
            {
                if (entity is Enemy enemy && Vector2.DistanceBetween(map.playerPosition, enemy.position) <= 15)
                {
                    Vector2 direction = enemy.position - map.playerPosition;

                    if (Player.AppliedAdventure >= 10 && Player.AppliedAdventure <= 30)
                    {                        
                        if (Player.AppliedAdventure == 20)
                        {
                            // If adventure = 20, unleash a wall of fire with ranged attacks (spawn them +1, -1 apart)
                            newProjectiles.Add(new Projectile(map.playerPosition, direction + new Vector2(1, 1)));
                            newProjectiles.Add(new Projectile(map.playerPosition, direction - new Vector2(1, 1)));
                        }
                        else if (Player.AppliedAdventure == 30)
                        {
                            // If adventure = 30, unleash a wave of fire with ranged attacks (in all directions +y, -y, +x, -x)
                            newProjectiles.Add(new Projectile(map.playerPosition, direction + new Vector2(0, 1)));
                            newProjectiles.Add(new Projectile(map.playerPosition, direction - new Vector2(0, 1)));
                            newProjectiles.Add(new Projectile(map.playerPosition, direction + new Vector2(1, 0)));
                            newProjectiles.Add(new Projectile(map.playerPosition, direction - new Vector2(1, 0)));
                        }
                    }
                    else
                    {
                        newProjectiles.Add(new Projectile(map.playerPosition, direction));
                    }
                }
            }
        }
    }

    public void ApplyStrengthBonus()
    {
        // Increase all damage by 5
        strengthBonus += 5;
    }

    public void ResetStrengthBonus()
    {
        strengthBonus = 0;
    }

    public void DealDamageToEnemies(World map, int baseDamage, double attackRange = 1)
    {
        int totalDamage = baseDamage + strengthBonus;
        totalDamage += Player.AppliedGlory; 
        
        foreach (Entity entity in map.entities)
        {
            if (entity is Enemy enemy)
            {
                double distance = Math.Sqrt(Math.Pow(map.playerPosition.x - enemy.position.x, 2) + Math.Pow(map.playerPosition.y - enemy.position.y, 2));
                
                if (distance <= attackRange)
                {
                    // Apply damage with glory multiplier and considering strength bonus
                    enemy.health -= totalDamage;
                }
            }
        }
    }

    public void UpdateCooldowns(int tick)
    {
        if (tick % 3 == 0) 
        { 
            standardAttackCooldown = Math.Max(0, standardAttackCooldown - 1);
            heavyAttackCooldown = Math.Max(0, heavyAttackCooldown - 1);
            frenzyAttackCooldown = Math.Max(0, frenzyAttackCooldown - 1);
            rangedAttackCooldown = Math.Max(0, rangedAttackCooldown - 1);
        }
    }
}

class OpeningDecisions
{
    public string playerName;

    public void OpeningText()
    {
        // Opening Storyline
        Console.WriteLine("Welcome to the Great Lumia City, the city of the night");
        Console.WriteLine("Unlike most of the world, the city is mostly asleep during the day and instead awake during the night.");
        Console.WriteLine("We hope you'll enjoy your stay. May I ask brave adventurer what do you seek here?");
    }

    public bool Tutorial()
    {
        Console.WriteLine("Would you like to go through the tutorial? (Yes/No)");
        string tutorialChoice = Console.ReadLine();

        return (tutorialChoice.ToLower() == "yes");
    }

    public void ChoosePlayerName()
    {
        Console.WriteLine("Please enter your player name:");
        playerName = Console.ReadLine();
        Console.WriteLine("Your player name is " + playerName);
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void DisplayTutorial()
    {
        Console.WriteLine("Welcome to the tutorial!");
        Console.WriteLine("In this game, you will embark on an adventure in Lumia City.");
        Console.WriteLine("You'll have to make decisions to survive and thrive.");
        Console.WriteLine("Let's start with the basics:");

        // Explanation of movement controls
        Console.WriteLine("\nMovement Controls:");
        Console.WriteLine("Use 'W', 'A', 'S', 'D' keys to move your character UP, LEFT, DOWN, RIGHT respectively.");
        Console.WriteLine("Press 'Q' to quit the game at any time.");

        // Explanation of class selection
        Console.WriteLine("\nClasses:");
        Console.WriteLine("You have three classes to choose from, each with unique abilities:");
        Console.WriteLine("- Rogue: Agile and stealthy, able to dash past enemies.");
        Console.WriteLine("- Wizard: Masters of arcane magic, capable of ranged attacks and fiery spells.");
        Console.WriteLine("- Fighter: Strong and resilient, specializing in close combat and powerful strikes.");

        // Explanation of seeking selection
        Console.WriteLine("\nSeeking Your Path:");
        Console.WriteLine("Before you begin your adventure, you must choose what you seek in Lumia City.");
        Console.WriteLine("Your choice will determine the multiplier for your stats as you progress:");
        Console.WriteLine("- Glory: Multiplier for the Glory stat, increasing damage dealt to enemies.");
        Console.WriteLine("- Money: Multiplier for the Money stat, increasing your maximum health.");
        Console.WriteLine("- Adventure: Multiplier for the Adventure stat, increasing your Adventure Level.");

        // Explanation of stat selection
        Console.WriteLine("\nStats and Upgrades:");
        Console.WriteLine("During your adventure, you'll earn points that will upgrade your abiltiies.");
        Console.WriteLine("There are three stats that cna increase:");
        Console.WriteLine("- Glory: Increases damage dealt to enemies.");
        Console.WriteLine("- Money: Increases your health, making you more resilient against attacks.");
        Console.WriteLine("- Adventure: Increases your special class ability strength.");
        Console.WriteLine("Choose wisely where to allocate your points to suit your playstyle.");

        // Explanation of Adventure stat and upgraded abilities
        Console.WriteLine("\nAdventure Stat and Class Abilities:");
        Console.WriteLine("As you progress through your adventure and gain points, you'll unlock more powerful versions of your class abilities.");
        Console.WriteLine("The Adventure stat determines the level of your abilities and their effectiveness.");
        Console.WriteLine("Here are the upgraded abilities for each class:");

        // Upgraded abilities for each class based on Adventure level
        Console.WriteLine("- Rogue:");
        Console.WriteLine("   - Adventure Level 10: Dash ability distance increases.");
        Console.WriteLine("   - Adventure Level 20: Enemies lose vision on the player for a certain period.");
        Console.WriteLine("   - Adventure Level 30: Player deals double damage to enemies when they have no vision.");

        Console.WriteLine("- Wizard:");
        Console.WriteLine("   - Adventure Level 10: Player becomes invulnerable after using a ranged attack for a brief period.");
        Console.WriteLine("   - Adventure Level 20: Releases walls of flame with ranged attacks.");
        Console.WriteLine("   - Adventure Level 30: Unleashes a spiral of flames with ranged attacks.");

        Console.WriteLine("- Fighter:");
        Console.WriteLine("   - Adventure Level 10: Dash ability distance increases.");
        Console.WriteLine("   - Adventure Level 20: Frenzy attack cooldown decreases.");
        Console.WriteLine("   - Adventure Level 30: Player becomes temporarily invulnerable during a frenzy attack.");

        // Explanation of game objectives and consequences
        Console.WriteLine("\nGame Objectives and Consequences:");
        Console.WriteLine("Your objective is to survive waves of enemies and earn points by defeating them.");
        Console.WriteLine("If your health reaches zero, the game ends, and your adventure comes to a halt.");
        Console.WriteLine("Make strategic decisions, upgrade your character wisely, and claim victory in Lumia City!");

        Console.WriteLine("\nPress any key to begin your adventure...");
        Console.ReadKey(true);
    }

    public void ChoiceSeek()
    {
        while (true) 
        {   
            Console.WriteLine("Most Seek either Glory, Money, or Adventure");
            string playerGoal = Console.ReadLine();

            if (playerGoal.Equals("Glory", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("You've chosen to activate the Glory multiplier.");
                Player.GloryMultiplierActive = true; // Activate the Glory multiplier
                break;
            }

            else if (playerGoal.Equals("Money", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("You've chosen to activate the Money multiplier.");
                Player.MoneyMultiplierActive = true; // Activate the Money multiplier
                break;
            }

            else if (playerGoal.Equals("Adventure", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("You've chosen to activate the Adventure multiplier.");
                Player.AdventureMultiplierActive = true; // Activate the Adventure multiplier
                break;
            }     

            else
            {
                Console.WriteLine("Sorry, choose one of the options or ask for INFO");
            }
        }
    }

    public void ChoiceClass()
    {
        while (true)
        {
            string currentPlayerClass = GlobalVariables.playerClass;
            
            Console.WriteLine("Please select a class: (Rogue, Wizard, Fighter)");
            string classChoice = Console.ReadLine().ToLower();

            if (classChoice == "rogue")
            {
                Console.WriteLine("You've Selected The Rogue Class, You gain the dash ability");
                GlobalVariables.playerClass = "Rogue";
                break;
            }

            else if (classChoice == "wizard")
            {
                Console.WriteLine("You've Selected The Wizard Class, You gain the dash ability");
                GlobalVariables.playerClass = "Wizard";
                break;
            }

            else if (classChoice == "fighter")
            {
                Console.WriteLine("You've Selected The Fighter Class, You gain the frenzy ability");
                GlobalVariables.playerClass = "Fighter";
                break;
            }

            else if (classChoice == "modclass")
            {
                while(true)
                {
                    Console.WriteLine("So you're a mod, yeah? I don't believe you.");
                    Console.WriteLine("Tell the truth, are you a mod?");
                    string ModAnswer = Console.ReadLine().ToLower();


                    if (ModAnswer == "yes")
                    {
                        Console.WriteLine("Fair Enough, move along");
                        GlobalVariables.playerClass = "ModClass";
                        break;
                    }

                    else if (ModAnswer == "no")
                    {
                        Console.WriteLine("Get Out of Here");
                        GlobalVariables.playerClass = "HardMode";
                        break;
                    }
                }
            }

            else
            {
                Console.WriteLine("Sorry, please choose one of the options.");
            }
        }
    }
}

public static class Leaderboard
{
    private static List<Tuple<string, int>> scores = new List<Tuple<string, int>>();

    public static void AddScore(string playerName, int score)
    {
        scores.Add(new Tuple<string, int>(playerName, score));
        scores.Sort((a, b) => b.Item2.CompareTo(a.Item2)); // Sort scores in descending order
        if (scores.Count > 5)
        {
            scores.RemoveAt(5); // Remove the sixth score
        }
    }

    // Method to display the leaderboard
    public static void DisplayLeaderboard()
    {
        Console.WriteLine("Leaderboard:");
        for (int i = 0; i < Math.Min(scores.Count, 5); i++) // Display top 5 scores
        {
            Console.WriteLine($"#{i + 1}: {scores[i].Item1} - {scores[i].Item2}");
        }
    }
}
