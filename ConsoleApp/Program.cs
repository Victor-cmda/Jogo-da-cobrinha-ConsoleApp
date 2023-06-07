using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConsoleSnakeGame
{
    class Program
    {
        static int screenWidth = 80;
        static int screenHeight = 25;
        static int score = 0;
        static int gameSpeed = 100;
        static Direction currentDirection = Direction.Right;
        static Snake snake = new Snake();
        static Food food = new Food();

        public static int GetScreenWidth()
        {
            return screenWidth;
        }

        public static int GetScreenHeight()
        {
            return screenHeight;
        }

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.WindowHeight = screenHeight;
            Console.BufferHeight = screenHeight;
            Console.WindowWidth = screenWidth;
            Console.BufferWidth = screenWidth;

            Console.SetCursorPosition(screenWidth / 2, screenHeight / 2);
            Console.Write("Press any key to start");
            Console.ReadKey();

            Console.Clear();
            DrawGameBorder();

            snake.Initialize();
            food.Generate(snake.Body);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    HandleKeyPress(key);
                }

                if (snake.HasCollidedWithWall() || snake.HasCollidedWithItself())
                {
                    GameOver();
                    break;
                }

                if (snake.Head.X == food.Position.X && snake.Head.Y == food.Position.Y)
                {
                    snake.Grow();
                    score++;
                    food.Generate(snake.Body);
                    IncreaseGameSpeed();
                }

                snake.Move(currentDirection);

                DrawScore();
                snake.Draw();
                food.Draw();

                Thread.Sleep(gameSpeed);
                Console.Clear();
            }
        }

        static void HandleKeyPress(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (currentDirection != Direction.Down)
                        currentDirection = Direction.Up;
                    break;
                case ConsoleKey.DownArrow:
                    if (currentDirection != Direction.Up)
                        currentDirection = Direction.Down;
                    break;
                case ConsoleKey.LeftArrow:
                    if (currentDirection != Direction.Right)
                        currentDirection = Direction.Left;
                    break;
                case ConsoleKey.RightArrow:
                    if (currentDirection != Direction.Left)
                        currentDirection = Direction.Right;
                    break;
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
            }
        }

        static void DrawGameBorder()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < screenWidth; i++)
                Console.Write("▒");

            Console.SetCursorPosition(0, screenHeight - 1);
            for (int i = 0; i < screenWidth; i++)
                Console.Write("▒");

            for (int i = 1; i < screenHeight - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("▒");
                Console.SetCursorPosition(screenWidth - 1, i);
                Console.Write("▒");
            }
        }

        static void DrawScore()
        {
            Console.SetCursorPosition(2, screenHeight - 1);
            Console.Write($"Score: {score}");
        }

        static void GameOver()
        {
            Console.Clear();
            Console.SetCursorPosition(screenWidth / 2 - 4, screenHeight / 2);
            Console.Write("Game Over");
            Console.SetCursorPosition(screenWidth / 2 - 8, screenHeight / 2 + 1);
            Console.Write($"Score: {score}");
            Console.SetCursorPosition(screenWidth / 2 - 12, screenHeight / 2 + 2);
            Console.Write("Press any key to exit");

            Console.ReadKey();
            Environment.Exit(0);
        }

        static void IncreaseGameSpeed()
        {
            if (score % 5 == 0 && gameSpeed > 50)
                gameSpeed -= 10;
        }
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !p1.Equals(p2);
        }
    }

    class Snake
    {
        private List<Point> body;
        private char headChar = 'O';
        private char bodyChar = '■';

        public List<Point> Body
        {
            get { return body; }
        }

        public Point Head
        {
            get { return body.First(); }
        }

        public Snake()
        {
            body = new List<Point>();
        }

        public void Initialize()
        {
            body.Clear();
            body.Add(new Point(10, 10));
            body.Add(new Point(9, 10));
            body.Add(new Point(8, 10));
        }

        public void Move(Direction direction)
        {
            Point newHead = new Point(Head.X, Head.Y);

            switch (direction)
            {
                case Direction.Up:
                    newHead.Y--;
                    break;
                case Direction.Down:
                    newHead.Y++;
                    break;
                case Direction.Left:
                    newHead.X--;
                    break;
                case Direction.Right:
                    newHead.X++;
                    break;
            }

            body.Insert(0, newHead);
            body.RemoveAt(body.Count - 1);
        }

        public void Grow()
        {
            body.Add(new Point(body.Last().X, body.Last().Y));
        }

        public bool HasCollidedWithWall()
        {
            if (Head.X <= 0 || Head.X >= Program.GetScreenWidth() - 1 || Head.Y <= 0 || Head.Y >= Program.GetScreenHeight() - 1)
                return true;

            return false;
        }

        public bool HasCollidedWithItself()
        {
            for (int i = 1; i < body.Count; i++)
            {
                if (Head.Equals(body[i]))
                    return true;
            }

            return false;
        }

        public void Draw()
        {
            Console.SetCursorPosition(Head.X, Head.Y);
            Console.Write(headChar);

            for (int i = 1; i < body.Count; i++)
            {
                Console.SetCursorPosition(body[i].X, body[i].Y);
                Console.Write(bodyChar);
            }
        }
    }

    class Food
    {
        private Point position;
        private char foodChar = '■';

        public Point Position
        {
            get { return position; }
        }

        public void Generate(List<Point> snakeBody)
        {
            Random random = new Random();

            while (true)
            {
                int x = random.Next(1, Program.GetScreenWidth() - 1);
                int y = random.Next(1, Program.GetScreenHeight() - 1);

                Point newPosition = new Point(x, y);

                if (!snakeBody.Contains(newPosition))
                {
                    position = newPosition;
                    break;
                }
            }
        }

        public void Draw()
        {
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write(foodChar);
        }
    }
}
