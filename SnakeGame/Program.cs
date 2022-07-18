﻿using System;
using System.Threading;
using System.Diagnostics;
using static System.Console;

namespace SnakeGame
{
    class Program
    {
        private const int MapWidht = 30;
        private const int MapHeight = 20;

        private const int ScreenWidht = MapWidht*3;
        private const int ScreenHeight = MapHeight*3;

        private const int FrameMs = 200;

        private const ConsoleColor BorderColor = ConsoleColor.Gray;
        private const ConsoleColor HeadColor = ConsoleColor.Red;
        private const ConsoleColor BodyColor = ConsoleColor.Yellow;
        private const ConsoleColor FoodColor = ConsoleColor.Green;

        private static readonly Random Random = new Random();   

        static void Main()
        {
            SetWindowSize(ScreenWidht, ScreenHeight);
            SetBufferSize(ScreenWidht, ScreenHeight);
            CursorVisible = false;

            while (true)
            {
                StartGame();
                Thread.Sleep(1000);
                ReadKey();
            }

            

        }

        static void StartGame()
        {
            Clear();

            DrawBorder();

            int score = 0;

            int LagMs = 0;

            Direction currentMovement = Direction.Right;

            var snake = new Snake(15, 10, HeadColor, BodyColor);

            Pixel food=GenFood(snake);
            food.Draw();

            Stopwatch sw = new Stopwatch();

            while (true)
            {
                sw.Restart();

                Direction oldMovement = currentMovement;


                while (sw.ElapsedMilliseconds <= FrameMs-  LagMs)
                {
                    if (currentMovement == oldMovement)
                    {
                        currentMovement = ReadMovement(currentMovement);
                    }

                }

                sw.Restart();

                if (snake.Head.X == food.X && snake.Head.Y == food.Y)
                {
                    snake.Move(currentMovement, true);
                    food = GenFood(snake);
                    food.Draw();
                    score++;
                    Task.Run(() => Beep(1200, 200));
                }
                else
                {
                    snake.Move(currentMovement);
                }

                if (snake.Head.X == MapWidht - 1 || snake.Head.X == 0 || snake.Head.Y == MapHeight - 1 || snake.Head.Y == 0 || snake.Body.Any(b => b.X == snake.Head.X && b.Y == snake.Head.Y)||score==10)
                    break;

                LagMs=(int)sw.ElapsedMilliseconds;
            }

            snake.Clear();
            food.Clear();

            if (score == 10)
            {
                SetCursorPosition(ScreenWidht / 3, ScreenHeight / 2);
                WriteLine($"You Win,score:{score}");
            }
            else
            {
                SetCursorPosition(ScreenWidht / 3, ScreenHeight / 2);
                WriteLine($"Game over, score:{score}");
            }
            Task.Run(() => Beep(200, 600));
        }


        static Pixel GenFood(Snake snake)
        {
            Pixel food;
            
            do
            {
                food = new Pixel(Random.Next(1, MapWidht - 2), Random.Next(1, MapHeight - 2), FoodColor);
            }while(snake.Head.X==food.X && snake.Head.Y==food.Y || snake.Body.Any(b=>b.X==food.X && b.Y==food.Y));

            return food;
        }

        static Direction ReadMovement(Direction currentDirection)
        {
            if(!KeyAvailable)
                return currentDirection;

            ConsoleKey key= ReadKey(true).Key;

            currentDirection = key switch
            {
                ConsoleKey.UpArrow when currentDirection != Direction.Down=>Direction.Up,
                ConsoleKey.DownArrow when currentDirection != Direction.Up => Direction.Down,
                ConsoleKey.LeftArrow when currentDirection != Direction.Right => Direction.Left,
                ConsoleKey.RightArrow when currentDirection != Direction.Left => Direction.Right,
                _ => currentDirection
            };

            return currentDirection;
        }

        static void DrawBorder()
        {
            for(int i = 0; i < MapWidht; i++)
            {
                new Pixel(i, 0, BorderColor).Draw();
                new Pixel(i, MapHeight-1, BorderColor).Draw();
            }

            for (int i = 0; i < MapHeight; i++)
            {
                new Pixel(0, i, BorderColor).Draw();
                new Pixel(MapWidht-1, i, BorderColor).Draw();
            }
        }
    }

}
