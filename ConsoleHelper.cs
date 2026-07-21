using System;

namespace _userdatabase.UserDatabase;

public enum Color
{
    Default, White, Black, Yellow, Green, Cyan, Blue, DarkYellow, Red, Gray, DarkGreen, DarkGray
}

public static class ConsoleHelper
{
    public static void ColorSwitch(Color color)
    {
        Console.ForegroundColor = color switch
        {
            Color.White => ConsoleColor.White,
            Color.Black => ConsoleColor.Black,
            Color.Yellow => ConsoleColor.Yellow,
            Color.Green => ConsoleColor.Green,
            Color.Cyan => ConsoleColor.Cyan,
            Color.Blue => ConsoleColor.Blue,
            Color.DarkYellow => ConsoleColor.DarkYellow,
            Color.Red => ConsoleColor.Red,
            Color.Gray => ConsoleColor.Gray,
            Color.DarkGreen => ConsoleColor.DarkGreen,
            Color.DarkGray => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };

        if (color == Color.Default) Console.ResetColor();
    }

    public static void DrawText(string text, Color color = Color.Default, bool insertNewLine = true)
    {
        ColorSwitch(color);
        Console.Write(text);
        if (insertNewLine) Console.Write(Environment.NewLine);
        Console.ResetColor();
    }

    public static string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return text.Length <= maxLength ? text : text.Substring(0, maxLength - 3) + "...";
    }

    public static string TakeInput(string prefix = " > ", Color prefixColor = Color.Green, Color textColor = Color.Default)
    {
        ColorSwitch(prefixColor);
        Console.Write(prefix);
        ColorSwitch(textColor);
        string input = Console.ReadLine() ?? "";
        Console.ResetColor();
        DrawText("");
        return input;
    }
}