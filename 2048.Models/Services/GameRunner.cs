using _2048.Models;
using _2048.Options;

namespace _2048.Services;

public class GameRunner
{
    private GameBoard _gameBoard;
    private GameOptions _gameOptions;
    private bool _gameDone;
    private readonly HashSet<ConsoleKey> _validMoves = [ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.UpArrow, ConsoleKey.DownArrow];
    
    public GameRunner(GameOptions gameOptions)
    {
        _gameOptions = gameOptions;
    }

    public void Run()
    {
        while (true)
        {
            InstantiateBoard();

            while (!_gameDone)
            {
                Console.Clear();

                _gameBoard.PrintBoard();

                //if (MoveCells(AwaitMove()))
                if (Move(AwaitMove()))
                    SpawnRandomCells(amountToCreate: 1);
                    
                _gameDone = GameWon() || GameLost();
            }

            Console.Clear();
            _gameBoard.PrintBoard();

            Console.WriteLine("Press any key to play again. Press 'esc' to exit");

            var inputKey = Console.ReadKey(true).Key;

            if (inputKey == ConsoleKey.Escape) Environment.Exit(0);  // non-error exit code 0
        }
    }

    /// <summary>
    /// Generates starting cells randomly on the starting game board.
    /// </summary>
    private void InstantiateBoard()
    {
        _gameDone = false;
        _gameBoard = new(_gameOptions.BoardRows, _gameOptions.BoardColumns);

        SpawnRandomCells(_gameOptions.NumberOfStartingCells);
    }

    private void SpawnRandomCells(int amountToCreate)
    {
        var random = new Random();

        for (int i = 0; i < amountToCreate; i++)
        {
            List<Cell> emptyCells = GetEmptyCells();

            int cellValue = random.Next(100) < 90 ? 2 : 4;
            Cell? chosenCell = null;

            if (emptyCells.Any())
            {
                chosenCell = emptyCells[random.Next(emptyCells.Count)];
                _gameBoard.GameState[chosenCell.Row][chosenCell.Column] = new Cell(cellValue, chosenCell.Row, chosenCell.Column);
            }           
        }
    }

    private List<Cell> GetEmptyCells()
    {
        var emptycells = new List<Cell>();

        for(int i = 0; i < _gameOptions.BoardRows; i++)
        {
            for(int j = 0; j < _gameOptions.BoardColumns; j++)
            {
                if (_gameBoard.GameState[i][j].Value == 0)
                    emptycells.Add(_gameBoard.GameState[i][j]);
            }
        }

        return emptycells;
    }

    private ConsoleKey AwaitMove()
    {
        ConsoleKey move = ConsoleKey.None;

        while(!_validMoves.Contains(move))
        {
            move = Console.ReadKey(true).Key;
        }

        return move;
    }

    /// <summary>
    /// Legacy method that moves cells to the left
    /// </summary>
    /// <returns></returns>
    [Obsolete]
    private bool MoveLeft()
    {
        bool moved = false;

        for(int row = 0; row < _gameBoard.GameState.Length; row++)
        {
            var rowLength = _gameBoard.GameState[row].Length;
            int lastMergeColumn = -1; // track column where last merge occurred 

            for (int column = 1; column < rowLength; column++)
            {
                var currentCell = _gameBoard.GameState[row][column];
               
                if (currentCell.Value == 0) continue;

                bool edgeReached = false;

                // keep moving current cell left until we hit the edge
                while (!edgeReached)
                {
                    if (currentCell.Column - 1 < 0) break; // we should not go out of bounds, if past edge, break

                    var leftNeighbor = _gameBoard.GameState[row][currentCell.Column - 1];

                    // if both cells 0, no need to perform a merge
                    if (currentCell.Value != 0 &&
                        currentCell.Value == leftNeighbor.Value &&
                        lastMergeColumn != leftNeighbor.Column) // <-- only merge if not already merged
                    {
                        leftNeighbor.SetValue(leftNeighbor.Value + currentCell.Value);
                        currentCell.SetValue(0);

                        lastMergeColumn = leftNeighbor.Column; // mark this position as merged

                        moved = true;
                        edgeReached = true; // no need to keep sliding after a merge
                    }                      
                    // if left neighbor is 0, set value to right neighbors' set right neighbor to 0
                    else if (leftNeighbor.Value == 0 && currentCell.Value != 0)
                    {
                        leftNeighbor.SetValue(currentCell.Value);
                        currentCell.SetValue(0);
                        moved = true;
                    }
                    else
                    {
                        edgeReached = true;
                    }
                                      
                    currentCell = leftNeighbor;

                    if (currentCell.Column == 0) edgeReached = true;
                }
            }
        }

        return moved;
    }

    /// <summary>
    /// Game is won when there is at least one cell with value of 2048.
    /// </summary>
    /// <returns></returns>
    private bool GameWon() => CellWithValueExists(2048);

    /// <summary>
    /// Game is lost when no empty cells are left and no existing cells can merge with each other.
    /// </summary>
    /// <returns></returns>
    private bool GameLost()
    {
        for(int row = 0; row < _gameBoard.GameState.Length; row++)
        {
            for (int column = 0; column < _gameBoard.GameState.Length; column++)
            {
                var currentCellValue = _gameBoard.GameState[row][column].Value;

                if (currentCellValue == 0) return false; // short-circuit if a 0-cell exists, game is not over

                var rightNeighborValue = column + 1 < _gameBoard.GameState[row].Length ? _gameBoard.GameState[row][column + 1].Value : 0;
                var belowNeighborValue = row + 1 < _gameBoard.GameState.Length ? _gameBoard.GameState[row + 1][column].Value : 0;

                if (currentCellValue == rightNeighborValue || currentCellValue == belowNeighborValue) return false;
            }
        }

        return true;
    }

    private bool CellWithValueExists(int value)
    {
        for(int row = 0; row < _gameBoard.GameState.Length; row++)
            for (int column = 0; column < _gameBoard.GameState.Length; column++)
                if (_gameBoard.GameState[row][column].Value == value) return true;
       
        return false;
    }

    private bool Move(ConsoleKey direction)
    {
        bool moved = false;

        int rows = _gameBoard.GameState.Length;   // O(1)
        int cols = _gameBoard.GameState[0].Length; // O(1)

        bool isHorizontal = direction == ConsoleKey.LeftArrow || direction == ConsoleKey.RightArrow; // O(1)
        bool reverse = direction == ConsoleKey.RightArrow || direction == ConsoleKey.DownArrow;      // O(1)

        if (isHorizontal)
        {
            for (int r = 0; r < rows; r++)  // O(rows)
            {
                var line = GetRowValues(r);  // O(cols)

                if (reverse) line.Reverse();  // O(cols)

                var result = SlideAndMergeLeft(line); // O(cols)

                var newLine = result.Line;  // O(1) (reference copy)
                if (reverse) newLine.Reverse();  // O(cols)

                if (result.Changed)         // O(1)
                {
                    SetRowValues(r, newLine); // O(cols)
                    moved = true;
                }
            }
        }
        else // vertical
        {
            for (int c = 0; c < cols; c++)  // O(cols)
            {
                var line = GetColumnValues(c);  // O(rows)

                if (reverse) line.Reverse();  // O(rows)

                var result = SlideAndMergeLeft(line); // O(rows)

                var newLine = result.Line;  // O(1)
                if (reverse) newLine.Reverse();  // O(rows)

                if (result.Changed)         // O(1)
                {
                    SetColumnValues(c, newLine); // O(rows)
                    moved = true;
                }
            }
        }

        return moved;
    }

    private MergeResult SlideAndMergeLeft(List<int> line)
    {
        bool changed = false;
        int length = line.Count;
        int lastMergeIndex = -1; // the last position that was merged this turn

        for (int currentIndex = 1; currentIndex < length; currentIndex++)
        {
            if (line[currentIndex] == 0) continue;

            int targetIndex = currentIndex;

            // slide left through empty spots
            while (targetIndex > 0 && line[targetIndex - 1] == 0)
            {
                line[targetIndex - 1] = line[targetIndex];
                line[targetIndex] = 0;
                targetIndex--;
                changed = true;
            }

            // attempt to merge with left neighbor
            if (targetIndex > 0 &&
                line[targetIndex] != 0 &&
                line[targetIndex - 1] == line[targetIndex] &&
                lastMergeIndex != targetIndex - 1)
            {
                line[targetIndex - 1] *= 2;
                line[targetIndex] = 0;
                lastMergeIndex = targetIndex - 1;
                changed = true;
            }
        }

        return new MergeResult(line, changed);
    }

    // --- helpers: read/write just the values from the board ---

    private List<int> GetRowValues(int rowIndex)
    {
        int colCount = _gameBoard.GameState[rowIndex].Length;
        var rowValues = new List<int>(colCount);

        for (int colIndex = 0; colIndex < colCount; colIndex++)
            rowValues.Add(_gameBoard.GameState[rowIndex][colIndex].Value);

        return rowValues;
    }

    private void SetRowValues(int rowIndex, List<int> values)
    {
        for (int colIndex = 0; colIndex < values.Count; colIndex++)
            _gameBoard.GameState[rowIndex][colIndex].SetValue(values[colIndex]);
    }

    private List<int> GetColumnValues(int colIndex)
    {
        int rowCount = _gameBoard.GameState.Length;
        var colValues = new List<int>(rowCount);

        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            colValues.Add(_gameBoard.GameState[rowIndex][colIndex].Value);

        return colValues;
    }

    private void SetColumnValues(int colIndex, List<int> values)
    {
        for (int rowIndex = 0; rowIndex < values.Count; rowIndex++)
            _gameBoard.GameState[rowIndex][colIndex].SetValue(values[rowIndex]);
    }
}
