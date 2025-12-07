using _2048.Extensions;

namespace _2048.Models;

public  class GameBoard
{
    public Cell[][] GameState { get; private set; }
    private readonly int _minRows = 4;
    private readonly int _minColumns = 4;
    private readonly int _maxRows = 12;
    private readonly int _maxColumns = 12;

    /// <summary>
    /// Clamps grid if provided rows and columns count outside thresholds
    /// </summary>
    public GameBoard(int numRows, int numColumns) 
    {
        CreateGameBoard(numRows, numColumns);
    }

    public void PrintBoard()
    {
        for(int i = 0; i < GameState.Length; i++)
        {
            for(int j = 0; j < GameState[i].Length; j++) 
            {
                Console.Write($"|{(GameState[i][j].Value == 0 ? " " : GameState[i][j].Value)}");
            }

            Console.Write("|\n");
        }

        Console.Write("\n");
    }

    private void CreateGameBoard(int numRows, int numColumns)
    {
        numRows.Clamp(_minRows, _maxRows);
        numColumns.Clamp(_minColumns, _maxColumns);

        GameState = new Cell[numRows][];

        // ints default to 0 so should just have to instantiate each row array
        for (int i = 0; i < numRows; i++)
        {
            GameState[i] = new Cell[numColumns];

            for(int j = 0; j < numColumns; j++)
            {
                GameState[i][j] = new Cell(0, i, j);
            }
        }
    }
}
