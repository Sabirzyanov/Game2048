// Insaf Sabirzyanov 220P
// 2048 Game 
// 22.12

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Game2048
{
    internal class Program
    {
        const int BoardWidth = 4;
        const int BoardHeight = 4;

        static int[,] board = new int[BoardWidth, BoardHeight];

        static string playerName;
        static int playerPoints;
        
        static int previousHighscore; 
        static string previousHighscorePlayerName;

        static bool isGameOver;

        static string fileName = "scores.txt";
        
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome 2048 game");
            Console.WriteLine("Your turns: Up/Right/Down/Left");
            Console.Write("You name: ");
            playerName = Console.ReadLine();

            FillCells();
            
            GetHightscoreFromFile();
            DisplayBoard();
            
            while (!isGameOver)
            {
                Console.Write("Turn: ");
                string input = Console.ReadLine();
                bool isBoardChange = false;
                switch (input.ToLower())
                {
                    case "up":
                        isBoardChange = MoveBoardHandler(0, -1);
                        break;
                    case "right":
                        isBoardChange = MoveBoardHandler(1, 0);
                        break;
                    case "down":
                        isBoardChange = MoveBoardHandler(0, 1);
                        break;
                    case "left":
                        isBoardChange = MoveBoardHandler(-1, 0);
                        break;
                    default:
                        Console.WriteLine("Wrong input");
                        continue;
                }

                Console.Clear();
                
                if (!isBoardChange)
                {
                    FillCells();
                }

                DisplayBoard();
                if (IsGameOver())
                {
                    Console.WriteLine("You lose!");
                    SaveScore();
                    break;
                }
            }
        }

        public static void SaveScore()
        {
            string textFromFile = "";
            try
            {
                using (FileStream fstream = File.OpenRead(fileName))
                {
                    byte[] array = new byte[fstream.Length];
                    fstream.Read(array, 0, array.Length); 
                    textFromFile = System.Text.Encoding.Default.GetString(array);
                }
            }
            catch (Exception e)
            {

            }
            
            using (FileStream fstream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                string scoreText = $"{playerName} {playerPoints} {DateTime.Now}";
                byte[] array = System.Text.Encoding.Default.GetBytes(scoreText + "\n" + textFromFile);
                fstream.Write(array, 0, array.Length);
            }
        }

        public static void GetHightscoreFromFile()
        {
            try
            {
                using (FileStream fstream = File.OpenRead(fileName))
                {
                    byte[] array = new byte[fstream.Length];
                    fstream.Read(array, 0, array.Length);
                    string textFromFile = System.Text.Encoding.Default.GetString(array);
                    string[][] info = textFromFile.Split('\n').Select(x => x.Split()).ToArray();
                    previousHighscore = int.Parse(info[0][1]);
                    previousHighscorePlayerName = info[0][0];
                    for (int i = 1; i < info.GetLength(0); ++i)
                    {
                        if (previousHighscore < int.Parse(info[i][1]))
                        {
                            previousHighscore = int.Parse(info[i][1]);
                            previousHighscorePlayerName = info[i][0];
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            
        }

        public static void FillCells()
        {
            Random rand = new Random();
            List<int[]> emptyCellsCoords = GetEmptyCellsCoords();
            int cellsToFill = Math.Min(emptyCellsCoords.Count, 2);
            for (int i = 0; i < cellsToFill; ++i)
            {
                int cellToFillIndex = rand.Next(emptyCellsCoords.Count);
                int[] cellToFillCoords = emptyCellsCoords[cellToFillIndex];
                board[cellToFillCoords[0], cellToFillCoords[1]] = 2;
                emptyCellsCoords.RemoveAt(cellToFillIndex);
            }
        }

        public static bool IsCellEmpty(int row, int col)
        {
            if (!IsCellOnBoard(row, col))
                return false;
            return board[row, col] == 0;
        }

        public static bool IsCellOnBoard(int row, int col)
        {
            return row >= 0 && row < BoardWidth &&
                   col >= 0 && col < BoardHeight;
        }

        public static void CellsSwap(int row, int col, int value)
        {
            if (IsCellOnBoard(row, col))
                board[row, col] = value;
        }

        public static List<int[]> GetEmptyCellsCoords()
        {
            List<int[]> emptyCellsCoords = new List<int[]>();
            for (int i = 0; i < board.GetLength(0); ++i)
            {
                for (int j = 0; j < board.GetLength(1); ++j)
                {
                    if (IsCellEmpty(i, j))
                    {
                        emptyCellsCoords.Add(new int[] { i, j });
                    }
                }
            }

            return emptyCellsCoords;
        }

        public static bool MoveBoardHandler(int rowDirection, int colDirection)
        {
            bool isBoardChange = false;
            switch (rowDirection)
            {
                case 1:
                    for (int i = 0; i < BoardWidth; ++i)
                    {
                        for (int j = BoardWidth - 2; j >= 0; --j)
                        {
                            MoveBoard(j, i, rowDirection, colDirection);
                        }

                        for (int j = BoardWidth - 2; j >= 0; --j)
                        {
                            isBoardChange = SumBoardCells(j, i, rowDirection, colDirection);
                            if (isBoardChange)
                                break;
                        }
                    }

                    break;
                case -1:
                    for (int i = 0; i < BoardWidth; ++i)
                    {
                        for (int j = 1; j < BoardWidth; ++j)
                        {
                            MoveBoard(j, i, rowDirection, colDirection);
                        }

                        for (int j = 1; j < BoardWidth; ++j)
                        {
                            isBoardChange = SumBoardCells(j, i, rowDirection, colDirection);
                            if (isBoardChange)
                                break;
                        }
                    }

                    break;
            }

            switch (colDirection)
            {
                case 1:
                    for (int i = 0; i < BoardWidth; ++i)
                    {
                        for (int j = BoardWidth - 2; j >= 0; --j)
                        {
                            MoveBoard(i, j, rowDirection, colDirection);
                        }

                        for (int j = BoardWidth - 2; j >= 0; --j)
                        {
                            isBoardChange = SumBoardCells(i, j, rowDirection, colDirection);
                            if (isBoardChange)
                                break;
                        }
                    }

                    break;
                case -1:
                    for (int i = 0; i < BoardWidth; ++i)
                    {
                        for (int j = 1; j < BoardHeight; ++j)
                        {
                            MoveBoard(i, j, rowDirection, colDirection);
                        }

                        for (int j = 1; j < BoardHeight; ++j)
                        {
                            isBoardChange = SumBoardCells(i, j, rowDirection, colDirection);
                            if (isBoardChange)
                                break;
                        }
                    }

                    break;
            }

            return isBoardChange;
        }


        public static void MoveBoard(int col, int row, int colDir, int rowDir)
        {
            if (IsCellOnBoard(row, col))
            {
                if (board[row, col] > 0)
                {
                    while (IsCellOnBoard(row + rowDir, col + colDir) &&
                           board[row + rowDir, col + colDir] == 0)
                    {
                        CellsSwap(row + rowDir, col + colDir, board[row, col]);
                        CellsSwap(row, col, 0);
                        row += rowDir;
                        col += colDir;
                    }
                }
            }
        }


        public static bool SumBoardCells(int col, int row, int colDir, int rowDir)
        {
            bool isBoardChange = false;
            if (!IsCellEmpty(row, col) && board[row, col] == board[row + rowDir, col + colDir])
            {
                playerPoints += board[row, col] * 2;
                isBoardChange = true;
                CellsSwap(row + rowDir, col + colDir, board[row, col] * 2);
                while (!IsCellEmpty(row - rowDir, col - colDir) &&
                       IsCellOnBoard(row - rowDir, col - colDir))
                {
                    CellsSwap(row, col, board[row - rowDir, col - colDir]);
                    col -= colDir;
                    row -= rowDir;
                }

                board[row, col] = 0;
            }
            return isBoardChange;
        }

        public static bool IsGameOver()
        {
            if (isGameOver)
                return isGameOver;
            for (int i = 0; i < BoardWidth; ++i)
            {
                for (int j = 0; j < BoardHeight; ++j)
                {
                    if (IsCellEmpty(i, j))
                        return false;
                    try
                    {
                        if (board[i, j] == board[i, j + 1])
                            return false;
                    }
                    catch
                    {
                    }

                    try
                    {
                        if (board[i, j] == board[i + 1, j])
                            return false;
                    }
                    catch
                    {
                    }
                }
            }

            isGameOver = true;
            return isGameOver;
        }

        public static void DisplayBoard()
        {
            Console.WriteLine($"You points: {playerPoints}");
            Console.WriteLine($"Highscore: {previousHighscorePlayerName} : {previousHighscore}");
            for (int i = 0; i < board.GetLength(0); ++i)
            {
                for (int j = 0; j < board.GetLength(1); ++j)
                {
                    Console.Write($" [{board[i, j]}] ");
                }

                Console.WriteLine();
            }
        }
    }
}