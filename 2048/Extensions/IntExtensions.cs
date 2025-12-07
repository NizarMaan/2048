namespace _2048.Extensions;

public static class IntExtensions
{
    public static void Clamp(this ref int value, int min, int max)
    {
        if (value < min) value = min;
        else if (value > max) value = max;
    }
}
