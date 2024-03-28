using System;
using System.Collections.Generic;

class World 
{
	public Vector2 playerPosition;
	public int width, height;
	public char[,] cells;
	public List<Entity> entities;

	public World(int initWidth, int initHeight) 
	{
		playerPosition = new Vector2();
		width = initWidth;
		height = initHeight;

		entities = new List<Entity>();

		cells = new char[width, height];
		for(int y = 0; y < height; y++) 
        {
			for(int x = 0; x < width; x++) 
            {
				cells[x,y] = ' ';
			}
		}

		for(int y = 0; y < height; y++) 
        {
			cells[0,y] = '|';
			cells[width-1,y] = '|';
		}

		for(int x = 0; x < width; x++) 
        {
			cells[x,0] = '-';
			cells[x,height-1] = '-';
		}

        for (int x = 1; x <= 7; x++)
        {
    		cells[x, 8] = '-';
        }

        for (int y = 1; y < 8; y++)
        {
            cells[8, y] = '|';
        }

        cells[8, 5] = 'D';
		cells[8, 4] = 'D';
 		cells[4, 7] = 'C';

		RandomGenerator.AddRandomPillars(this);
        RandomGenerator.AddRandomChests(this);
        RandomGenerator.AddRandomTrapStrips(this);
		RandomGenerator.AddRandomHealthPickups(this);
		RandomGenerator.AddRandomStrengthPickups(this);

		entities.Add(new Enemy(24, 2));
	}

	public void Render() 
	{
		Frame nextFrame = new Frame(70, 30);

		for(int y = 0; y < height; y++) 
		{
			for(int x = 0; x < width; x++) 
			{
				bool visible = CanSee(playerPosition, new Vector2(x, y));
				if(visible || (cells[x,y] == '@'))
					nextFrame.Put(x, y, cells[x,y]);
				else
					nextFrame.Put(x, y, '.');
			}
		}

		nextFrame.Put(playerPosition.x, playerPosition.y, 'X');

		foreach(Entity theEntity in entities) 
		{
			if(CanSee(playerPosition, theEntity.position))
				theEntity.Render(nextFrame);
		}

		nextFrame.Render();
	}
	
	public int CountRemainingEnemies()
	{
		int count = 0;
		foreach (Entity entity in entities)
		{
			if (entity is Enemy)
			{
				count++;
			}
		}
		return count;
	}

	public bool CheckWall(Vector2 v) {
		if((v.x < 0) || (v.x >= width) || (v.y < 0) || (v.y >= height))
			return true;
		
		return (cells[v.x,v.y] == '|') || (cells[v.x,v.y] == '-') || (cells[v.x, v.y] == 'C') || (cells[v.x, v.y] == '#');
	}

	public bool CanSee(Vector2 playerPosition, Vector2 targetPosition) 
	{
		return CanSee(playerPosition, targetPosition, 8.0f);
	}

	public bool CanSee(Vector2 playerPosition, Vector2 targetPosition, float range) 
	{
		
		if(Vector2.DistanceBetween(playerPosition, targetPosition) > range)
			return false;

		bool visible = true;
		List<Vector2> points = Extra.LineBetween(playerPosition, targetPosition);
		
		while(CheckWall(points[points.Count - 1]))
			points.RemoveAt(points.Count - 1);

		foreach(Vector2 point in points) 
		{
			if(CheckWall(point))
				visible = false;
		}

		return visible;
	}
}

class RandomGenerator {
    private const int maxChests = 4; 
    private const int maxPillarBlocks = 20;
    private const int maxTrapStrips = 5; 
    private const int maxHealthPickups = 3; 
	private const int maxStrengthPickups = 3; 

    public static void AddRandomPillars(World map)
    {
        Random rand = new Random();

        for (int i = 0; i < maxPillarBlocks; i++)
        {
            bool placed = false;
            while (!placed)
            {
                int startX = rand.Next(1, map.width - 2); 
                int startY = rand.Next(1, map.height - 2);
                if (CheckAvailability(map, startX, startY, 2, 2))
                {
                    for (int x = startX; x < startX + 2; x++)
                    {
                        for (int y = startY; y < startY + 2; y++)
                        {
                            map.cells[x, y] = '#';
                        }
                    }
                    placed = true;
                }
            }
        }

        for (int i = 0; i < maxPillarBlocks; i++)
        {
            bool placed = false;
            while (!placed)
            {
                int centerX = rand.Next(2, map.width - 3); 
                int centerY = rand.Next(2, map.height - 3);
                if (CheckAvailability(map, centerX - 1, centerY - 1, 3, 3))
                {
                    for (int x = centerX - 1; x <= centerX + 1; x++)
                    {
                        for (int y = centerY - 1; y <= centerY + 1; y++)
                        {
                            double distance = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                            if (distance <= 1.5)
                            {
                                map.cells[x, y] = '#';
                            }
                        }
                    }
                    placed = true;
                }
            }
        }
    }

	public static void AddRandomChests(World map)
	{
		Random rand = new Random();

		for (int i = 0; i < maxChests; i++)
		{
			bool placed = false;
			while (!placed)
			{
				int chestX = rand.Next(1, map.width - 1);
				int chestY = rand.Next(1, map.height - 1); 
				if (CheckAvailability(map, chestX, chestY, 1, 1))
				{
					map.entities.Add(new Chest(chestX, chestY)); 
					placed = true;
				}
			}
		}
	}

	public static void AddRandomTrapStrips(World map)
    {
        Random rand = new Random();

        for (int i = 0; i < maxTrapStrips; i++)
        {
            bool placed = false;
            while (!placed)
            {
                int startX = rand.Next(1, map.width - 5); 
                int startY = rand.Next(1, map.height - 1); 
                int length = rand.Next(2, 5); 
                if (CheckAvailability(map, startX, startY, length, 1))
                {
                    for (int x = startX; x < startX + length; x++)
                    {
                        map.cells[x, startY] = '+';
                    }
                    placed = true;
                }
            }
        }
    }

	public static void AddRandomHealthPickups(World map)
	{
		Random rand = new Random();

		for (int i = 0; i < maxHealthPickups; i++)
		{
			bool placed = false;
			while (!placed)
			{
				int healthPickupX = rand.Next(1, map.width - 1); 
				int healthPickupY = rand.Next(1, map.height - 1); 
				if (CheckAvailability(map, healthPickupX, healthPickupY, 1, 1))
				{
					map.entities.Add(new HealthPickup(healthPickupX, healthPickupY));
					placed = true;
				}
			}
		}
	}

	public static void AddRandomStrengthPickups(World map)
	{
		Random rand = new Random();

		for (int i = 0; i < maxStrengthPickups; i++)
		{
			bool placed = false;
			while (!placed)
			{
				int strengthPickupX = rand.Next(1, map.width - 1);
				int strengthPickupY = rand.Next(1, map.height - 1); 
				if (CheckAvailability(map, strengthPickupX, strengthPickupY, 1, 1))
				{
					map.entities.Add(new StrengthPickup(strengthPickupX, strengthPickupY));
					placed = true;
				}
			}
		}
	}

    private static bool CheckAvailability(World map, int startX, int startY, int width, int height)
    {
		int safeZoneStartX = 1;
		int safeZoneEndX = 10;
		int safeZoneStartY = 1;
		int safeZoneEndY = 10;

		for (int x = startX; x < startX + width; x++)
		{
			for (int y = startY; y < startY + height; y++)
			{
				if (x >= safeZoneStartX && x < safeZoneEndX && y >= safeZoneStartY && y < safeZoneEndY)
				{
					return false; 
				}

				if (map.CheckWall(new Vector2(x, y)))
				{
					return false;
				}

				foreach (Entity entity in map.entities)
				{
					if (entity.position.x == x && entity.position.y == y)
					{
						return false; 
					}
				}
			}
		}
		return true;
	}
}

class Frame {
	private char[,] buffer;
	private uint width, height;

	public Frame(uint initWidth, uint initHeight) 
	{
		width = initWidth;
		height = initHeight;
		buffer = new char[width, height];
	}

	public void Put(int x, int y, char c) 
	{
		if((x >= 0) && (x < width) && (y >= 0) && (y < height))
			buffer[x,y] = c;
	}

	public void Render() 
	{
		Extra.MoveTo(0, 0);

		for(int y = 0; y < height; y++) 
		{
			for(int x = 0; x < width; x++) 
			{
				Console.Write(buffer[x, y]);
			}
			Console.Write("\n");
		}
	}

	public static void DisplayPlayerStats(Dictionary<string, string> stats) 
	{
		foreach (var stat in stats) 
		{
			Console.WriteLine($"{stat.Key}: {stat.Value}");
		}
	}
}
