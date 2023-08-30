using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeperGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hi Dev, Welcome to Minesweeper!");

            int gridSize = GetGridSize();
            int mineCount = GetMineCount(gridSize);

            MinesweeperGame game = new MinesweeperGame(gridSize, mineCount);

            while (!game.GameOver && !game.GameWon)
            {
                Console.Clear();
                game.DisplayGrid();

                string input = GetValidInput(gridSize);
                int row = input[0] - 'A';
                int col = int.Parse(input.Substring(1)) - 1;

                game.UncoverCell(row, col);
            }

            Console.Clear();
            game.DisplayGrid();

            if (game.GameWon)
            {
                Console.WriteLine("Congratulations Dev, you have won the game!");
            }
            else
            {
                Console.WriteLine("Oh no, you detonated a mine! Game over.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        static int GetGridSize()
        {
            int gridSize;
            while (true)
            {
                Console.Write("Enter the size of the grid (e.g. 4 for a 4x4 grid): ");
                if (int.TryParse(Console.ReadLine(), out gridSize) && gridSize >= 2 && gridSize <= 10)
                {
                    return gridSize;
                }
                Console.WriteLine("Invalid input. Minimum size is 2, maximum size is 10.");
            }
        }

        static int GetMineCount(int gridSize)
        {
            int maxMines = (int)(gridSize * gridSize * 0.35);
            int mineCount;
            while (true)
            {
                Console.Write($"Enter the number of mines to place on the grid (maximum is {maxMines}): ");
                if (int.TryParse(Console.ReadLine(), out mineCount) && mineCount >= 1 && mineCount <= maxMines)
                {
                    return mineCount;
                }
                Console.WriteLine($"Invalid input. Must be between 1 and {maxMines} mines.");
            }
        }

        static string GetValidInput(int gridSize)
        {
            while (true)
            {
                Console.Write("Select a square to reveal (e.g. A1): ");
                string input = Console.ReadLine().ToUpper();

                if (input.Length == 2 &&
                    input[0] >= 'A' && input[0] < 'A' + gridSize &&
                    int.TryParse(input[1].ToString(), out int col) && col >= 1 && col <= gridSize)
                {
                    return input;
                }

                Console.WriteLine("Incorrect input. Please try again.");
            }
        }
    }

    class MinesweeperGame
    {
        private char[,] grid;
        private char[,] solution;
        private bool[,] isMine;
        private bool[,] isUncovered;
        private int gridSize;
        private int mineCount;
        private int uncoveredCount;

        public bool GameOver { get; private set; }
        public bool GameWon => uncoveredCount == gridSize * gridSize - mineCount;

        public MinesweeperGame(int gridSize, int mineCount)
        {
            this.gridSize = gridSize;
            this.mineCount = mineCount;
            this.grid = new char[gridSize, gridSize];
            this.solution = new char[gridSize, gridSize];
            this.isMine = new bool[gridSize, gridSize];
            this.isUncovered = new bool[gridSize, gridSize];

            InitializeGrid();
            PlaceMines();
        }

        private void InitializeGrid()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    grid[i, j] = '_';
                    solution[i, j] = '_';
                }
            }
        }

        private void PlaceMines()
        {
            Random random = new Random();

            for (int i = 0; i < mineCount;)
            {
                int row = random.Next(gridSize);
                int col = random.Next(gridSize);

                if (!isMine[row, col])
                {
                    isMine[row, col] = true;
                    i++;
                }
            }
        }

        public void DisplayGrid()
        {
            Console.WriteLine("Here is your minefield:");
            Console.Write("  ");
            for (int i = 0; i < gridSize; i++)
            {
                Console.Write($"{i + 1} ");
            }
            Console.WriteLine();

            for (int i = 0; i < gridSize; i++)
            {
                Console.Write((char)('A' + i) + " ");
                for (int j = 0; j < gridSize; j++)
                {
                    char cell = isUncovered[i, j] ? grid[i, j] : '_';
                    Console.Write(cell + " ");
                }
                Console.WriteLine();
            }
        }

        public void UncoverCell(int row, int col)
        {
            if (isUncovered[row, col] || GameOver || GameWon)
            {
                return;
            }

            isUncovered[row, col] = true;

            if (isMine[row, col])
            {
                GameOver = true;
                return;
            }

            int adjacentMines = CountAdjacentMines(row, col);
            grid[row, col] = adjacentMines.ToString()[0];

            uncoveredCount++;

            if (adjacentMines == 0)
            {
                UncoverAdjacentCells(row, col);
            }
        }

        private int CountAdjacentMines(int row, int col)
        {
            int count = 0;

            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    int newRow = row + dr;
                    int newCol = col + dc;

                    if (newRow >= 0 && newRow < gridSize && newCol >= 0 && newCol < gridSize && isMine[newRow, newCol])
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private void UncoverAdjacentCells(int row, int col)
        {
            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    int newRow = row + dr;
                    int newCol = col + dc;

                    if (newRow >= 0 && newRow < gridSize && newCol >= 0 && newCol < gridSize && !isUncovered[newRow, newCol])
                    {
                        UncoverCell(newRow, newCol);
                    }
                }
            }
        }
    }
}
