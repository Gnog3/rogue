namespace Rogue.Presentation.States;

public interface IState
{
    IState? Update(char key);
    void Render();
}
