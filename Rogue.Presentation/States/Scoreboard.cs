using Rogue.Domain;
using System.Text;

namespace Rogue.Presentation.States;

internal class Scoreboard : IState
{
    private readonly string _data;

    public Scoreboard()
    {

        List<Statistics> stats = Data.Data.LoadStatistics();
        _data = FormatStatistics(stats);
    }

    public IState Update(char key) => new MainMenu();

    public void Render()
    {
        string[] lines = _data.Split('\n');
        for (int y = 0; y < lines.Length; y++)
        {
            Terminal.Instance.PutString(0, y, Font.White, lines[y]);
        }
    }

    private static string FormatStatistics(List<Statistics> stats)
    {
        if (stats.Count == 0)
        {
            return "No scoreboard available.";
        }
        var sb = new StringBuilder();
        sb.AppendLine("Scoreboard:");
        foreach (var stat in stats.OrderByDescending(s => s.Treasures).Take(5))
        {
            sb.AppendLine($"Level: {stat.Level:D2} Treasures: {stat.Treasures:D4} Enemies: {stat.Enemies:D2} Food: {stat.Food:D2} Elixirs: {stat.Elixirs:D2}");
            sb.AppendLine($"Scrolls: {stat.Scrolls:D2} Attacks: {stat.Attacks:D2} Missed: {stat.Missed:D2} Moves: {stat.Moves:D3}");
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
