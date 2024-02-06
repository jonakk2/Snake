using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.VisualBasic;
///█ ■
////https://www.youtube.com/watch?v=SGZgvMwjq2U
namespace Snake
{
    class Program
    {
        //size of program
        private const int HEIGHT = 16;
        private const int WIDTH = 32;

        static void Main(string[] args)
        {
            //size of console
            Console.WindowHeight = 16;
            Console.WindowWidth = 32;


            Random random = new Random();
            int score = 5;
            bool gameover = false;


            SnakeHead snakeHead = new SnakeHead(WIDTH/2, HEIGHT/2, ConsoleColor.Red);
            
            //starting movement
            Direction direction = Direction.Right;


            List<int> xTail = new List<int>();
            List<int> yTail = new List<int>();

            Berry berry = new Berry(random.Next(0, WIDTH), random.Next(0, HEIGHT));

            DateTime stepStart = DateTime.Now;
            DateTime stepEnd = DateTime.Now;

            bool isButtonPressed = false;

            //main game loop
            while (true)
            {
                Console.Clear();

                //snake in boundary
                if (snakeHead.x == WIDTH - 1 || snakeHead.x == 0 || snakeHead.y == HEIGHT - 1 || snakeHead.y == 0)
                {
                    gameover = true;
                }
                for (int i = 0; i < WIDTH; i++)
                {
                    Console.SetCursorPosition(i, 0);
                    Console.Write("■");
                }
                for (int i = 0; i < WIDTH; i++)
                {
                    Console.SetCursorPosition(i, HEIGHT - 1);
                    Console.Write("■");
                }
                for (int i = 0; i < HEIGHT; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write("■");
                }
                for (int i = 0; i < HEIGHT; i++)
                {
                    Console.SetCursorPosition(WIDTH - 1, i);
                    Console.Write("■");
                }
                Console.ForegroundColor = ConsoleColor.Green;

                //eat berry
                if (berry.x == snakeHead.x && berry.y == snakeHead.y)
                {
                    score++;
                    berry.x = random.Next(1, WIDTH - 2);
                    berry.y = random.Next(1, HEIGHT - 2);
                }
                for (int i = 0; i < xTail.Count(); i++)
                {
                    Console.SetCursorPosition(xTail[i], yTail[i]);
                    Console.Write("■");
                    //self kill
                    if (xTail[i] == snakeHead.x && yTail[i] == snakeHead.y)
                    {
                        gameover = true;
                    }
                }
                if (gameover)
                {
                    break;
                }
                Console.SetCursorPosition(snakeHead.x, snakeHead.y);
                Console.ForegroundColor = snakeHead.screenColor;
                Console.Write("■");
                Console.SetCursorPosition(berry.x, berry.y);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("■");
                stepStart = DateTime.Now;
                isButtonPressed = false;
                while (true)
                {
                    stepEnd = DateTime.Now;
                    if (stepEnd.Subtract(stepStart).TotalMilliseconds > 100) { break; }
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo toets = Console.ReadKey(true);
                        //Console.WriteLine(toets.Key.ToString());
                        if (toets.Key.Equals(ConsoleKey.UpArrow) && direction != Direction.Down && !isButtonPressed)
                        {
                            direction = Direction.Up;
                            isButtonPressed = true;
                        }
                        if (toets.Key.Equals(ConsoleKey.DownArrow) && direction != Direction.Up && !isButtonPressed)
                        {
                            direction = Direction.Down;
                            isButtonPressed = true;
                        }
                        if (toets.Key.Equals(ConsoleKey.LeftArrow) && direction != Direction.Right && !isButtonPressed)
                        {
                            direction = Direction.Left;
                            isButtonPressed = true;
                        }
                        if (toets.Key.Equals(ConsoleKey.RightArrow) && direction != Direction.Left && !isButtonPressed)
                        {
                            direction = Direction.Right;
                            isButtonPressed = true;
                        }
                    }
                }
                xTail.Add(snakeHead.x);
                yTail.Add(snakeHead.y);
                switch (direction)
                {
                    case Direction.Up:
                        snakeHead.y--;
                        break;
                    case Direction.Down:
                        snakeHead.y++;
                        break;
                    case Direction.Left:
                        snakeHead.x--;
                        break;
                    case Direction.Right:
                        snakeHead.x++;
                        break;
                }
                if (xTail.Count() > score)
                {
                    xTail.RemoveAt(0);
                    yTail.RemoveAt(0);
                }
            }
            Console.SetCursorPosition(WIDTH / 5, HEIGHT / 2);
            Console.WriteLine("Game over, Score: " + score);
            Console.SetCursorPosition(WIDTH / 5, HEIGHT / 2 + 1);
        }
        class SnakeHead
        {
            public int x { get; set; }
            public int y { get; set; }
            public ConsoleColor screenColor { get; set; }

            public SnakeHead(int x, int y, ConsoleColor screenColor)
            {
                this.x = x;
                this.y = y;
                this.screenColor = screenColor;
            }
        }

        class SnakeTail
        {
            private List<int> xTail;
            private List<int> yTail;

            public SnakeTail()
            {
                this.xTail = new List<int>();
                this.yTail = new List<int>();
            }

            public void grow(SnakeHead snakeHead)
            {
                xTail.Add(snakeHead.x);
                yTail.Add(snakeHead.y);
            }

            public void writeTail()
            {
                for (int i = 0; i < xTail.Count(); i++)
                {
                    Console.SetCursorPosition(xTail[i], yTail[i]);
                    Console.Write("■");
                }
            }

            public bool didHeadJustBiteMe(SnakeHead snakeHead)
            {
                for (int i = 0; i < xTail.Count(); i++)
                {
                    if (xTail[i] == snakeHead.x && yTail[i] == snakeHead.y)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        class Berry
        {
            public int x;
            public int y;

            public Berry(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        enum Direction
        {
            Left,
            Right,
            Up,
            Down

        }
    }
}
//¦