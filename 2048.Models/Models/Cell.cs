namespace _2048.Models;

public class Cell
{
    public Cell(int value, int row, int column)
    {
        Value = value;
        Row = row;
        Column = column;
    }

    public int Value { get;  private set; }
    public int Row { get; private set; }
    public int Column { get; private set; }

    public void SetValue(int value) => Value = value;
}
