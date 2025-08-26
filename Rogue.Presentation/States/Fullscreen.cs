using Rogue.Domain;

namespace Rogue.Presentation.States;

public class Fullscreen(string s, Func<IState?> nextState) : IState
{
    public const string StartString = "ROGUE";
    public const string GameOver = "GAME OVER";
    public const string Win = "You won! Congratulations!";
    private const string PressAnyKey = "Press any key to continue...";

    public IState? Update(char key)
    {
        return nextState();
    }

    public void Render()
    {
        int xStart = (Constants.MapWidth - s.Length) / 2;
        int y = Constants.MapHeight / 2;
        for (int x = 0; x < s.Length; x++)
        {
            Terminal.Instance.Put(x + xStart, y - 1, new MapChar(s[x], Font.White));
        }

        int xStart2 = (Constants.MapWidth - PressAnyKey.Length) / 2;
        for (int x = 0; x < PressAnyKey.Length; x++)
        {
            Terminal.Instance.Put(x + xStart2, y + 1, new MapChar(PressAnyKey[x], Font.White));
        }
    }
}
