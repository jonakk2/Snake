using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.VisualBasic;
using static System.Formats.Asn1.AsnWriter;
////https://www.youtube.com/watch?v=SGZgvMwjq2U
namespace Snake
{
    class Program
    {
        // Global constants
        private const int HEIGHT = 16;
        private const int WIDTH = 32;
        private const int START_SCORE = 5;
        private const int SPEED = 300;
        private const int MAX_POSSIBLE_SCORE = (HEIGHT-2) * (WIDTH-2) - 4;

        private const String PIXEL = "■";

        static void Main(string[] args)
        {
            // Init variables
            Console.SetWindowSize(WIDTH, HEIGHT);
            Console.CursorVisible = false;
            int score = START_SCORE;

            SnakeHead snakeHead = new SnakeHead(WIDTH / 2, HEIGHT / 2, ConsoleColor.Blue);
            SnakeTail snakeTail = new SnakeTail();
            Berry berry = new Berry();
            Direction direction = Direction.Right;

            DateTime stepStart = DateTime.Now;
            DateTime stepEnd = DateTime.Now;

            // Main game loop
            while (true)
            {
                Console.Clear();

                // Checks if game over
                if (snakeHead.amIOutOfBundary() || snakeTail.didHeadJustBiteMe(snakeHead) || score >= MAX_POSSIBLE_SCORE)
                {
                    break;
                }

                // Checks if berry was eaten
                if (berry.didSnakeJustAteMe(snakeHead))
                {
                    score++;
                    berry.changePosition(snakeHead, snakeTail);
                }

                // Updating graphics
                Boundary.write();
                snakeHead.write();
                snakeTail.write();
                berry.write();

                // Reading pressed key and switching direction
                stepStart = DateTime.Now;
                while (stepEnd.Subtract(stepStart).TotalMilliseconds < SPEED)
                {
                    stepEnd = DateTime.Now;
                    if (Console.KeyAvailable)
                    {
                        direction = switchDirection(Console.ReadKey(true), direction);
                       
                    }
                }

                // Growing and moving snake
                snakeTail.grow(snakeHead);        
                snakeTail.shrinkIfHungry(score);
                snakeHead.move(direction);
            }
            writeGameOver(score);
        }

        class SnakeHead
        {
            public int x { get; set; }
            public int y { get; set; }
            public ConsoleColor headColor { get; set; }

            public SnakeHead(int x, int y, ConsoleColor screenColor)
            {
                this.x = x;
                this.y = y;
                this.headColor = screenColor;
            }

            public bool amIOutOfBundary()
            {
                return this.x == WIDTH - 1 || this.x == 0 || this.y == HEIGHT - 1 || this.y == 0;
            }

            public void move(Direction direction)
            {
                switch (direction)
                {
                    case Direction.Up:
                        y--;
                        break;
                    case Direction.Down:
                        y++;
                        break;
                    case Direction.Left:
                        x--;
                        break;
                    case Direction.Right:
                        x++;
                        break;
                }
            }

            public void write()
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = headColor;
                Console.Write(PIXEL);
            }
        }

        class SnakeTail
        {
            public List<int> xTail { get; set; }
            public List<int> yTail { get; set; }

            public SnakeTail()
            {
                xTail = new List<int>();
                yTail = new List<int>();
            }

            public void grow(SnakeHead snakeHead)
            {
                xTail.Add(snakeHead.x);
                yTail.Add(snakeHead.y);
            }

            public void shrinkIfHungry(int score)
            {
                if (length() > score)
                {
                    xTail.RemoveAt(0);
                    yTail.RemoveAt(0);
                }
            }

            public void write()
            {
                for (int i = 0; i < xTail.Count(); i++)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition(xTail[i], yTail[i]);
                    Console.Write(PIXEL);
                }
            }

            public bool didHeadJustBiteMe(SnakeHead snakeHead)
            {
                for (int i = 0; i < length(); i++)
                {
                    if (xTail[i] == snakeHead.x && yTail[i] == snakeHead.y)
                    {
                        return true;
                    }
                }
                return false;
            }

            public int length()
            {
                return xTail.Count();
            }
        }

        class Berry
        {
            private Random random = new Random();
            public int x;
            public int y;

            public Berry()
            {
                this.x = random.Next(1, WIDTH - 2); ;
                this.y = random.Next(1, HEIGHT - 2); ;
            }

            public bool didSnakeJustAteMe(SnakeHead snakeHead)
            {
                return x == snakeHead.x && y == snakeHead.y;
            }

            public void changePosition(SnakeHead snakeHead, SnakeTail snakeTail)
            {
                this.x = random.Next(1, WIDTH - 2);
                this.y = random.Next(1, HEIGHT - 2);
                if (doIColideWithSnake(snakeHead, snakeTail))
                {
                    changePosition(snakeHead, snakeTail);
                }
            }

            private bool doIColideWithSnake(SnakeHead snakeHead, SnakeTail snakeTail)
            {
                if (x == snakeHead.x && y == snakeHead.y)
                {
                    return true;
                }
                for (int i = 0; i < snakeTail.length(); i++)
                {
                    if (x == snakeTail.xTail[i] && y == snakeTail.yTail[i])
                    {
                        return true;
                    }
                }
                return false;
            }

            public void write()
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(PIXEL);
            }
        }

        static class Boundary
        {
            public static void write()
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                for (int i = 0; i < WIDTH; i++)
                {
                    Console.SetCursorPosition(i, 0);
                    Console.Write(PIXEL);
                }
                for (int i = 0; i < WIDTH; i++)
                {
                    Console.SetCursorPosition(i, HEIGHT - 1);
                    Console.Write(PIXEL);
                }
                for (int i = 0; i < HEIGHT; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write(PIXEL);
                }
                for (int i = 0; i < HEIGHT; i++)
                {
                    Console.SetCursorPosition(WIDTH - 1, i);
                    Console.Write(PIXEL);
                }
            }
        }

        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }

        public static Direction switchDirection(ConsoleKeyInfo pressedKey, Direction currentDirection)
        {
            if (pressedKey.Key.Equals(ConsoleKey.UpArrow) && currentDirection != Direction.Down)
            {
                return Direction.Up;
            }
            else if (pressedKey.Key.Equals(ConsoleKey.DownArrow) && currentDirection != Direction.Up)
            {
                return Direction.Down;
            }
            else if (pressedKey.Key.Equals(ConsoleKey.LeftArrow) && currentDirection != Direction.Right)
            {
                return Direction.Left;
            }
            else if (pressedKey.Key.Equals(ConsoleKey.RightArrow) && currentDirection != Direction.Left)
            {
                return Direction.Right;
            }
            return currentDirection;
        }

        public static void writeGameOver(int score)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.SetCursorPosition(WIDTH / 5, HEIGHT / 2);
            Console.WriteLine("Game over, Score: " + score);
            Console.SetCursorPosition(WIDTH / 5 - 2, HEIGHT / 2 + 1);
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}
