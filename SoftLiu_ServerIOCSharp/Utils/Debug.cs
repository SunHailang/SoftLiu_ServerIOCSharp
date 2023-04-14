using System;

public static class Debug
{

    public static void Log(string msg)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"[{DateTime.Now.ToString("G")}] {msg}");
    }

    public static void LogWarning(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{DateTime.Now.ToString("G")}] {msg}");
    }

    public static void LogError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{DateTime.Now.ToString("G")}] {msg}");
    }

}
