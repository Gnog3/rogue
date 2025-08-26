using Mindmagma.Curses;
using Rogue.Domain;

namespace Rogue.Presentation;

public sealed class Terminal
{
    private const int MaxMessagesCount = 2;
    private readonly Map _map = new();
    private readonly List<string> _messages = [];

    public string StatusBar { get; set; } = "";

    internal void Put(int x, int y, MapChar c)
    {
        _map.Chars[x, y] = c;
    }

    internal void PutString(int x, int y, Font font, string s)
    {
        foreach (char ch in s)
        {
            Put(x, y, new MapChar(ch, font));
            x++;
        }
    }

    internal void PutMessage(string message)
    {
        while (_messages.Count >= MaxMessagesCount)
        {
            _messages.RemoveAt(0);
        }

        _messages.Add(message);
    }

    public void Render()
    {
        NCurses.Clear();
        for (int i = 0; i < _messages.Count; i++)
        {
            NCurses.AttributeSet(NCurses.ColorPair((int)Font.White));
            NCurses.MoveAddString(i, 0, _messages[i]);
        }

        for (int x = 0; x < Constants.MapWidth; x++)
        {
            for (int y = 0; y < Constants.MapHeight; y++)
            {
                MapChar c = _map.Chars[x, y];
                NCurses.AttributeSet(NCurses.ColorPair((int)c.Font));
                NCurses.MoveAddChar(y + MaxMessagesCount, x, c.Char);
            }
        }

        NCurses.AttributeSet(NCurses.ColorPair((int)Font.White));
        NCurses.MoveAddString(Constants.MapHeight + MaxMessagesCount, 0, StatusBar);
        NCurses.Refresh();
    }

    public void Clear()
    {
        _map.Clear();
        StatusBar = "";
    }

    public void ClearMessages()
    {
        _messages.Clear();
    }

    private static Terminal? _instance;

    public static Terminal Instance => _instance ?? throw new Exception("Terminal not initialized.");

    public static void Init()
    {
        nint screen = NCurses.InitScreen();
        NCurses.ResizeTerminal(MaxMessagesCount + Constants.MapHeight + 1, Constants.MapWidth);
        try
        {
            // fails on windows
            NCurses.SetCursor(CursesCursorState.INVISIBLE);
        }
        catch { }

        NCurses.NoEcho();
        NCurses.StartColor();
        NCurses.Keypad(screen, true);

        NCurses.InitPair((short)Font.White, CursesColor.WHITE, CursesColor.BLACK);
        NCurses.InitPair((short)Font.Red, CursesColor.RED, CursesColor.BLACK);
        NCurses.InitPair((short)Font.Green, CursesColor.GREEN, CursesColor.BLACK);
        NCurses.InitPair((short)Font.Blue, CursesColor.BLUE, CursesColor.BLACK);
        NCurses.InitPair((short)Font.Yellow, CursesColor.YELLOW, CursesColor.BLACK);
        NCurses.InitPair((short)Font.Cyan, CursesColor.CYAN, CursesColor.BLACK);
        NCurses.InitPair((short)Font.Black, CursesColor.BLACK, CursesColor.WHITE);

        _instance = new Terminal();
    }

    public static void Terminate()
    {
        NCurses.EndWin();
        _instance = null;
    }

    private Terminal() { }
}

internal enum Font
{
    White = 1,
    Red,
    Green,
    Blue,
    Yellow,
    Cyan,
    Black,
}

internal record struct MapChar(char Char, Font Font);

internal class Map
{
    public MapChar[,] Chars { get; } = new MapChar[Constants.MapWidth, Constants.MapHeight];

    public Map()
    {
        Clear();
    }

    public void Clear()
    {
        for (int x = 0; x < Constants.MapWidth; x++)
        {
            for (int y = 0; y < Constants.MapHeight; y++)
            {
                Chars[x, y] = new MapChar(' ', Font.White);
            }
        }
    }
}