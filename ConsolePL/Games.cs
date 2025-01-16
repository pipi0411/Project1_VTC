using System;
using System.Collections.Generic;

namespace Games
{
    public class SnakeGame
    {
        private Exception? exception = null;
        private int speedInput;
        private int[] velocities = { 100, 70, 50 };
        private char[] DirectionChars = { '^', 'v', '<', '>' };
        private TimeSpan sleep;
        private int width;
        private int height;
        private Tile[,] map;
        private Direction? direction = null;
        private Queue<(int X, int Y)> snake = new();
        private (int X, int Y) position;
        private bool closeRequested = false;

        public void StartGame()
        {
            Console.CursorVisible = false;
            string prompt = $"Select speed [1], [2] (default), or [3]: ";
            string? input;
            Console.Write(prompt);
            while (!int.TryParse(input = Console.ReadLine(), out speedInput) || speedInput < 1 || 3 < speedInput)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    speedInput = 2;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Input. Try Again...");
                    Console.Write(prompt);
                }
            }
            int velocity = velocities[speedInput - 1];
            sleep = TimeSpan.FromMilliseconds(velocity);
            width = Console.WindowWidth;
            height = Console.WindowHeight;
            map = new Tile[width, height];
            position = (width / 2, height / 2);

            try
            {
                Console.CursorVisible = false;
                Console.Clear();
                snake.Enqueue(position);
                map[position.X, position.Y] = Tile.Snake;
                PositionFood();
                Console.SetCursorPosition(position.X, position.Y);
                Console.Write('@');
                while (!direction.HasValue && !closeRequested)
                {
                    GetDirection();
                }
                while (!closeRequested)
                {
                    if (Console.WindowWidth != width || Console.WindowHeight != height)
                    {
                        Console.Clear();
                        Console.Write("Console was resized. Snake game has ended.");
                        return;
                    }
                    switch (direction)
                    {
                        case Direction.Up: position.Y--; break;
                        case Direction.Down: position.Y++; break;
                        case Direction.Left: position.X--; break;
                        case Direction.Right: position.X++; break;
                    }
                    if (position.X < 0 || position.X >= width ||
                        position.Y < 0 || position.Y >= height ||
                        map[position.X, position.Y] is Tile.Snake)
                    {
                        Console.Clear();
                        Console.Write("Game Over. Score: " + (snake.Count - 1) + ".");
                        return;
                    }
                    Console.SetCursorPosition(position.X, position.Y);
                    Console.Write(DirectionChars[(int)direction!]);
                    snake.Enqueue(position);
                    if (map[position.X, position.Y] is Tile.Food)
                    {
                        PositionFood();
                    }
                    else
                    {
                        (int x, int y) = snake.Dequeue();
                        map[x, y] = Tile.Open;
                        Console.SetCursorPosition(x, y);
                        Console.Write(' ');
                    }
                    map[position.X, position.Y] = Tile.Snake;
                    if (Console.KeyAvailable)
                    {
                        GetDirection();
                    }
                    System.Threading.Thread.Sleep(sleep);
                }
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                Console.CursorVisible = true;
                Console.Clear();
                Console.WriteLine(exception?.ToString() ?? "Snake was closed.");
            }
        }

        private void GetDirection()
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow: direction = Direction.Up; break;
                case ConsoleKey.DownArrow: direction = Direction.Down; break;
                case ConsoleKey.LeftArrow: direction = Direction.Left; break;
                case ConsoleKey.RightArrow: direction = Direction.Right; break;
                case ConsoleKey.Escape: closeRequested = true; break;
            }
        }

        private void PositionFood()
        {
            List<(int X, int Y)> possibleCoordinates = new();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (map[i, j] is Tile.Open)
                    {
                        possibleCoordinates.Add((i, j));
                    }
                }
            }
            int index = Random.Shared.Next(possibleCoordinates.Count);
            (int X, int Y) = possibleCoordinates[index];
            map[X, Y] = Tile.Food;
            Console.SetCursorPosition(X, Y);
            Console.Write('+');
        }

        private enum Direction
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
        }

        private enum Tile
        {
            Open = 0,
            Snake,
            Food,
        }
    }
}

