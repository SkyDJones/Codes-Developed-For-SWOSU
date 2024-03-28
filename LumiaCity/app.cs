using System;
using System.Collections.Generic;
using System.Data;

class MainProgram
{
	static public int tick = 0;
	static OpeningDecisions opening;

	static void Main() 
	{
		Console.Title = "Lumia City";
        Console.ForegroundColor = ConsoleColor.DarkRed;

		opening = new OpeningDecisions();
        opening.OpeningText();
        bool doTutorial = opening.Tutorial();
        if (doTutorial)
            opening.DisplayTutorial();
		opening.ChoosePlayerName();
        opening.ChoiceSeek();
        opening.ChoiceClass();

		Extra.EnableANSIEscape();

		Extra.ClearScreen();
		Extra.HideCursor();

		Player.CalculatePlayerInitialHealth();

		Waves();
	}

	static void Waves()
	{
		int wave = 1;

		while (true)
		{
			World currentWorld = new World(70, 30); 
			currentWorld.playerPosition.x = 1;
			currentWorld.playerPosition.y = 1;

			if (wave > 1)
			{
				for (int i = 1; i < wave; i++) 
				{
					SpawnRandomEnemy(currentWorld);
				}
			}

			while (true)
			{
				currentWorld.Render();

				bool quit = handleInput(currentWorld);
				if (quit)
					return;

				update(currentWorld);

				if (currentWorld.entities.Count(entity => entity is Enemy) == 0) 
				{
					Console.WriteLine("Wave cleared!");
					wave++;
					break; 
				}

				System.Threading.Thread.Sleep(100);
			}
		}
	}

	public static void GameOver(OpeningDecisions opening)
    {
        Console.Clear();

        string playerName = opening.GetPlayerName();
    	int score = Player.EnemiesSlain * 100;

		Player.DisplayStats(opening);

        Console.WriteLine("Game Over! Unfortunately, Lumia City has lost another adventurer... poor poor soul..");
        Console.WriteLine($"Let's review this intrepid hero's adventure. {playerName} has slain {Player.EnemiesSlain} enemies.");
        Console.WriteLine("You've served the city well. Let's see how well of service you've done.");

        Leaderboard.AddScore(playerName, score);
        Leaderboard.DisplayLeaderboard(); 

        Console.WriteLine("Would you like to:");
        Console.WriteLine("1. Quit the game");
        Console.WriteLine("2. Restart the game");

        string choice = Console.ReadLine().Trim();

        switch (choice)
        {
            case "1":
                Environment.Exit(0); // Quit the game
                break;
            case "2":
				Player.ResetPlayerStats();
				Console.Clear();
				opening.OpeningText();
				bool doTutorial = opening.Tutorial();
				if (doTutorial)
					opening.DisplayTutorial();
				opening.ChoosePlayerName();
				opening.ChoiceSeek();
				opening.ChoiceClass();
				Console.Clear();
				MainProgram.Waves(); // Restart the game
                break;
            default:
                Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                break;
        }
    }

	static void SpawnRandomEnemy(World map)
	{
		Random rand = new Random();
		int enemyX, enemyY;
		bool placed = false;

		while (!placed)
		{
			enemyX = rand.Next(1, map.width - 1);
			enemyY = rand.Next(1, map.height - 1);

			
			if (!map.CheckWall(new Vector2(enemyX, enemyY)) && !CheckEntityCollision(map, enemyX, enemyY))
			{
				int enemyType = rand.Next(1, 4);
				switch (enemyType)
				{
					case 1:
						map.entities.Add(new Enemy(enemyX, enemyY));
						break;
					case 2:
						map.entities.Add(new HeavyEnemy(enemyX, enemyY));
						break;
					case 3:
						map.entities.Add(new RangedEnemy(enemyX, enemyY));
						break;
				}
				placed = true;
			}
		}
	}

	static bool CheckEntityCollision(World map, int x, int y)
	{
		foreach (Entity entity in map.entities)
		{
			if (entity.position.x == x && entity.position.y == y)
			{
				return true;
			}
		}
		return false;
	}

	static int GetDashDistance()
	{
		return Player.AppliedAdventure == 10 ? 7 : 5;
	}

	static bool handleInput(World theWorld) 
	{
		
		Vector2 newPos = new Vector2(theWorld.playerPosition);

		string currentPlayerClass = GlobalVariables.playerClass;
		Attacks TheAttacks = new Attacks();
		OpeningDecisions openingDecisions = new OpeningDecisions();

		while (Console.KeyAvailable) 
		{
			char key = Console.ReadKey(true).KeyChar;
			switch (key) 
			{
				case 'q':
					return true;

				// Movement keys
				case 'w':
					newPos.y = theWorld.playerPosition.y - 1;
					break;

				case 's':
					newPos.y = theWorld.playerPosition.y + 1;
					break;

				case 'a':
					newPos.x = theWorld.playerPosition.x - 1;
					break;

				case 'd':
					newPos.x = theWorld.playerPosition.x + 1;
					break;
				
				case 'A':
					if (currentPlayerClass == "Rogue")
					{
						int dashDistance = GetDashDistance();
						while (dashDistance > 0)
						{
							if (!theWorld.CheckWall(new Vector2(newPos.x - 1, newPos.y))) 
							{
								newPos.x--; 
								
								foreach (Entity entity in theWorld.entities)
								{
									if (entity is Enemy enemy && entity.position.Equals(newPos))
									{
										if (Player.AppliedAdventure >= 20)
										{
											TheAttacks.DealDamageToEnemies(theWorld, 5);
										}
									}
								}
								 
								dashDistance--; 
							}
							else
							{
								break; 
							}
						}
					}
					break;

				case 'D':
					if (currentPlayerClass == "Rogue")
					{
						int dashDistance = GetDashDistance(); 
						while (dashDistance > 0)
						{
							if (!theWorld.CheckWall(new Vector2(newPos.x + 1, newPos.y)))
							{
								newPos.x++;
								
								foreach (Entity entity in theWorld.entities)
								{
									if (entity is Enemy enemy && entity.position.Equals(newPos))
									{
										if (Player.AppliedAdventure >= 20)
										{
											TheAttacks.DealDamageToEnemies(theWorld, 5); 
										}
									}
								}
								
								dashDistance--; 
							}
							else
							{
								break; 
							}
						}
					}
					break;

				case 'W':
					if (currentPlayerClass == "Rogue")
					{
						int dashDistance = GetDashDistance(); 
						while (dashDistance > 0)
						{
							if (!theWorld.CheckWall(new Vector2(newPos.x, newPos.y - 1))) 
							{
								newPos.y--; 
								
								foreach (Entity entity in theWorld.entities)
								{
									if (entity is Enemy enemy && entity.position.Equals(newPos))
									{
										if (Player.AppliedAdventure >= 20)
										{
											TheAttacks.DealDamageToEnemies(theWorld, 5); 
										}
									}
								}
								
								dashDistance--;
							}
							else
							{
								break; 
							}
						}
					}
					break;

				case 'S':
					if (currentPlayerClass == "Rogue")
					{
						int dashDistance = GetDashDistance();
						while (dashDistance > 0)
						{
							if (!theWorld.CheckWall(new Vector2(newPos.x, newPos.y + 1))) 
							{
								newPos.y++; 
								
								foreach (Entity entity in theWorld.entities)
								{
									if (entity is Enemy enemy && entity.position.Equals(newPos))
									{
										if (Player.AppliedAdventure >= 20)
										{
											TheAttacks.DealDamageToEnemies(theWorld, 5);
										}
									}
								}
								
								dashDistance--; 
							}
							else
							{
								break; 
							}
						}
					}
					break;
				
				case 'm':
				case 'M':
					Player.DisplayStats(opening); 
					Thread.Sleep(3000);
                	break;

				// Chest Interact Button
				case 'e':
				case 'E':
					foreach (Entity entity in theWorld.entities)
					{
						if (entity is Chest chest && Vector2.DistanceBetween(theWorld.playerPosition, entity.position) <= 1)
						{
							chest.Interact(); 
							break;
						}
					}
					break;

				// Basic Attack For All Classes
				case 'k':
				case 'K':
					TheAttacks.StandardAttack(theWorld);
					break;

				// Heavey Attack For All Classes
				case 'l':
				case 'L':
					TheAttacks.HeavyAttack(theWorld);
					break;
				
				// Frenzy Attack For Fighter Class
				case 'j':
				case 'J':
					if (currentPlayerClass == "Fighter")
					{
						TheAttacks.FrenzyAttack(theWorld);
					}
					break;

				// Ranged Attack For Wizard Class
				case 'r':
				case 'R':
					if (currentPlayerClass == "Wizard")
					{
						TheAttacks.RangedAttack(theWorld);
					}
					break;
			}
		}

		if (!theWorld.CheckWall(newPos)) 
		{
			theWorld.playerPosition = newPos;

			if (theWorld.cells[newPos.x, newPos.y] == '+') 
			{
				Player.TakeDamage(10);
			}
		}
		return false;
}

	public static void update(World theWorld)
	{
		List<Entity> toDelete = new List<Entity>();
		Attacks TheAttacks = new Attacks();

		// update the entities
		foreach (Entity theEntity in theWorld.entities)
		{
			theEntity.Update(theWorld);

			if (theEntity is Enemy enemy && enemy.health <= 0)
			{
				toDelete.Add(theEntity);
				Player.EnemiesSlain++;

				Player.GainRandomStatPoint();
			}

			if (theWorld.playerPosition.x == theEntity.position.x && theWorld.playerPosition.y == theEntity.position.y && !(theEntity is Enemy))
			{
				if (theEntity is HealthPickup)
				{
					Player.CurrentPlayerHealth = Player.CurrentPlayerHealth + 10;
					Player.DisplayHealth();
					
					toDelete.Add(theEntity); 
				}

				else if (theEntity is StrengthPickup)
				{
					TheAttacks.ApplyStrengthBonus();
					toDelete.Add(theEntity); 
				}
			}
		}

		foreach (Entity theEntity in toDelete)
		{
			if (theEntity is Pickup)
			{
				theWorld.entities.Remove(theEntity);
			}
			else if (theEntity is Enemy) 
			{
				theWorld.entities.Remove(theEntity);
			}
		}

		if (theWorld.entities.Count(entity => entity is Enemy) == 0)
		{
			Player.ApplyStatPoints();
		}

		TheAttacks.UpdateCooldowns(tick);

		tick++;

		if (Player.CurrentPlayerHealth <= 0)
		{
			Player.ApplyStatPoints();

			GameOver(opening);
			return; 
		}
	}
}