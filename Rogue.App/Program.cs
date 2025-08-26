namespace Rogue.App;

using Mindmagma.Curses;
using Presentation;
using Presentation.States;

internal class Program
{
    static void Main()
    {
        Terminal.Init();

        try
        {
            IState? state = new Fullscreen(Fullscreen.StartString, () => new MainMenu());
            while (state != null)
            {
                Terminal.Instance.Clear();
                state.Render();
                Terminal.Instance.Render();
                char key = GetCharAndRenderPeriodically();
                Terminal.Instance.ClearMessages();
                state = state.Update(key);
            }
        }
        finally
        {
            Terminal.Terminate();
        }
    }

    private static char GetCharAndRenderPeriodically()
    {
        Task<int> task = Task.Run(NCurses.GetChar);

        while (!task.Wait(750))
        {
            Terminal.Instance.Render();
        }

        return (char)task.Result;
    }
}
