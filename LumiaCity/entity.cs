using System;

class Enemy : Entity 
{
    public int health;
    private int attackCooldown = 0;

    public Enemy() : base() 
    {
        health = 10;
    }

    public Enemy(int initX, int initY) : base(initX, initY) 
    {
        health = 10;
    }

    public override void Render(Frame nextFrame) 
    {
        nextFrame.Put(position.x, position.y, 'e');
    }

    public override void Update(World theMap) 
    {
        if ((MainProgram.tick % 3) == 0)
        {
            if (attackCooldown > 0) {
                attackCooldown--;
                return; 
            }

            if (!theMap.CanSee(position, theMap.playerPosition, 12.0f))
                return;

            Vector2 direction = theMap.playerPosition - position;

            if (Math.Abs(direction.x) > Math.Abs(direction.y)) 
            {
                direction.x = Math.Clamp(direction.x, -1, 1);
                direction.y = 0;
            } 
            else 
            {
                direction.x = 0;
                direction.y = Math.Clamp(direction.y, -1, 1);
            }

            Vector2 newPosition = position + direction;

            if (theMap.cells[newPosition.x, newPosition.y] == '+') 
            {
                health -= 10; 
            } else if (!theMap.CheckWall(newPosition)) 
            {
                position = newPosition;
            }

            if (Vector2.DistanceBetween(position, theMap.playerPosition) <= 1) 
            {
                AttackPlayer(theMap);
            }
        }
    }
    private void AttackPlayer(World theMap)
    {
        Player.TakeDamage(5); 
        attackCooldown = 3; 
    }
}

class Projectile : Entity
{
    private Vector2 direction;

    public override void Render(Frame nextFrame) 
    {
        nextFrame.Put(position.x, position.y, '@');
    }

    public Projectile(Vector2 position, Vector2 direction)
    {
        this.position = position;
        this.direction = direction;
    }

}

class HeavyEnemy : Enemy
{
    public HeavyEnemy(int x, int y) : base(x, y)
    {
        health = 50;
    }

    public override void Render(Frame nextFrame)
    {
        nextFrame.Put(position.x, position.y, '&'); 
    }

    public override void Update(World map)
    {
        base.Update(map);
    }
}

class RangedEnemy : Enemy
{
    public int health;
    private int attackCooldown = 0;
    
    public RangedEnemy(int x, int y) : base(x, y)
    {
        health = 20; 
    }

    public override void Render(Frame nextFrame)
    {
        nextFrame.Put(position.x, position.y, 'R'); 
    }

    public override void Update(World theMap) 
    {
        if ((MainProgram.tick % 3) == 0)
        {
            if (attackCooldown > 0) {
                attackCooldown--;
                return; 
            }

            if (!theMap.CanSee(position, theMap.playerPosition, 6.0f))
                return;

            Vector2 direction = theMap.playerPosition - position;

            if (Math.Abs(direction.x) > Math.Abs(direction.y)) 
            {
                direction.x = Math.Clamp(direction.x, -1, 1);
                direction.y = 0;
            } 
            else 
            {
                direction.x = 0;
                direction.y = Math.Clamp(direction.y, -1, 1);
            }

            Vector2 newPosition = position + direction;

            if (theMap.cells[newPosition.x, newPosition.y] == '+') 
            {
                health -= 10;
            } else if (!theMap.CheckWall(newPosition)) 
            {
                position = newPosition;
            }

            if (Vector2.DistanceBetween(position, theMap.playerPosition) <= 5) 
            {
                AttackPlayer(theMap);
            }
        }
    }
    private void AttackPlayer(World theMap)
    {
        Player.TakeDamage(5); 
        attackCooldown = 3; 
    }
}

class Pickup : Entity 
{
	public Pickup() : base() {
	}

	public Pickup(int initX, int initY) : base(initX, initY) {
	}

	public override void Render(Frame nextFrame) {
		nextFrame.Put(position.x, position.y, 'p');
	}
}

class HealthPickup : Pickup
{
    public HealthPickup() : base() { }

    public HealthPickup(int initX, int initY) : base(initX, initY) { }

    public override void Render(Frame nextFrame)
    {
        nextFrame.Put(position.x, position.y, 'H');
    }
}

class StrengthPickup : Pickup
{
    public StrengthPickup() : base() { }

    public StrengthPickup(int initX, int initY) : base(initX, initY) { }

    public override void Render(Frame nextFrame)
    {
        nextFrame.Put(position.x, position.y, 'S');
    }
}

class Chest : Entity
{
    private bool isOpen = false;

    public Chest(int initX, int initY) : base(initX, initY)
    {
    }

    public override void Render(Frame nextFrame)
    {
        if (isOpen)
        {
            nextFrame.Put(position.x, position.y, 'O'); // Render open chest
        }
        else
        {
            nextFrame.Put(position.x, position.y, 'C'); // Render closed chest
        }
    }

    public void Interact()
    {
        if (!isOpen)
        {
            int randomStatPointsCount = new Random().Next(3, 6);
            for (int i = 0; i < randomStatPointsCount; i++)
            {
                Player.GainRandomStatPoint();
            }

            isOpen = true; 
        }
    }
}

abstract class Entity 
{
	public Vector2 position;

	public Entity() {
		position = new Vector2(0, 0);
	}

	public Entity(int initX, int initY) {
		position = new Vector2(initX, initY);
	}

	public abstract void Render(Frame nextFrame);

	public virtual void Update(World theMap) 
	{
	}
}
