using Rogue.Domain;
using Rogue.Domain.Characters;

namespace Rogue.Presentation.States;

public class MainMenu : IState
{
    private const int OptionLength = 10;
    private int _selected;

    private readonly (string Name, Func<IState?> Action)[] _options;

    public MainMenu()
    {
        _options =
        [
            ("NEW   GAME", NewGame),
            ("LOAD  GAME", LoadGame),
            ("SCOREBOARD", Scoreboard),
            ("EXIT  GAME", () => null),
        ];
    }

    public IState? Update(char key)
    {
        if (Controls.Up.Contains(key))
        {
            _selected = Math.Max(0, _selected - 1);
        }
        else if (Controls.Down.Contains(key))
        {
            _selected = Math.Min(_options.Length - 1, _selected + 1);
        }
        else if (Controls.Submit.Contains(key))
        {
            return _options[_selected].Action();
        }

        return this;
    }

    public void Render()
    {
        int height = _options.Length + 2;
        int yStart = (Constants.MapHeight - height) / 2;
        int xStart = (Constants.MapWidth - OptionLength) / 2;

        Terminal.Instance.PutString(xStart, yStart, Font.White, "GAME  MENU");

        for (int i = 0; i < _options.Length; i++)
        {
            Font font = i == _selected ? Font.Black : Font.White;
            Terminal.Instance.PutString(xStart, yStart + 2 + i, font, _options[i].Name);
        }
    }

    private static IState NewGame()
    {
        Player player = new(new Vector());
        Level level = Generation.GenerateLevel(1, player);
        Rogue.Domain.Game game = new(level, player);
        return new Game(game);
    }

    private IState LoadGame()
    {
        try
        {
            Rogue.Domain.Game game = Data.Data.LoadData();
            return new Game(game);
        }
        catch (Exception ex)
        {
            Terminal.Instance.PutMessage(ex.Message);
            return this;
        }
    }

    private static IState Scoreboard()
    {
        try
        {
            return new Scoreboard();
        }
        catch (Exception ex)
        {
            Terminal.Instance.PutMessage($"Error loading statistics: {ex.Message}");
            return new MainMenu();
        }
    }
}
