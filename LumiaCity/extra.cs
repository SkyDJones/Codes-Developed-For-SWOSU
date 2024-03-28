using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

class Extra {
	[DllImport("kernel32.dll")]
	private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

	[DllImport("kernel32.dll")]
	private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr GetStdHandle(int nStdHandle);

	static public void EnableANSIEscape() 
	{
		unsafe {
			var stdout = GetStdHandle(-11);
			GetConsoleMode(stdout, out uint mode);
			SetConsoleMode(stdout, mode | 0x4);
		}
	}

	// clear the entire screen
	static public void ClearScreen() 
	{
		Console.Write("\x1B[H\x1B[J");
	}

	// hide the cursor
	static public void HideCursor() 
	{
		Console.Write("\x1B[?25l");
	}

	// show the cursor
	static public void ShowCursor() 
	{
		Console.Write("\x1B[?25h");
	}

	// move the cursor to position (x,y)
	static public void MoveTo(int x, int y) 
	{
		Console.Write("\x1B[" + (y + 1) + ";" + (x + 1) + "H");
	}

	static public List<Vector2> LineBetween(Vector2 a, Vector2 b) 
	{
		List<Vector2> list = new List<Vector2>();

		a = new Vector2(a.x, a.y);
		b = new Vector2(b.x, b.y);

		int dx = Math.Abs(b.x - a.x);
    		int sx = a.x < b.x ? 1 : -1;
    		int dy = -Math.Abs(b.y - a.y);
    		int sy = a.y < b.y ? 1 : -1;
    		int error = dx + dy;

		while(true) 
		{
			list.Add(new Vector2(a.x, a.y));
        		if(a.x == b.x && a.y == b.y)
				break;

        		int e2 = 2 * error;

			if(e2 >= dy) 
			{
				if(a.x == b.x)
					break;

				error += dy;
				a.x += sx;
			}

			if(e2 <= dx) 
			{
				if(a.y == b.y)
					break;

				error += dx;
				a.y += sy;
			}
		}

		return list;
	}
}

class Vector2 
{
	public int x, y;

	public Vector2() 
	{
		x = 0;
		y = 0;
	}

	public Vector2(int initX, int initY) 
	{
		x = initX;
		y = initY;
	}

	public Vector2(Vector2 other) 
	{
		x = other.x;
		y = other.y;
	}

	public static Vector2 operator-(Vector2 a)  
	{
		return new Vector2(-a.x, -a.y);
	}

	public static Vector2 operator+(Vector2 a, Vector2 b)  
	{
		return new Vector2(a.x + b.x, a.y + b.y);
	}

	public static Vector2 operator-(Vector2 a, Vector2 b)  
	{
		return new Vector2(a.x - b.x, a.y - b.y);
	}

/*
	public static bool operator==(Vector2 a, Vector2 b)  {
		return a.x == b.x && a.y == b.y;
	}

	public static bool operator!=(Vector2 a, Vector2 b)  {
		return !(a == b);
	}
	*/

	public double Length() 
	{
		return Math.Sqrt(x*x + y*y);
	}

	public static double DistanceBetween(Vector2 a, Vector2 b) 
	{
		double dx = a.x - b.x;
		double dy = a.y - b.y;

		return Math.Sqrt(dx*dx + dy*dy);
	}
}