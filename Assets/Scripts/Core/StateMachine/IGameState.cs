namespace Core.StateMachine
{
    /// <summary>
    /// Interface for all game states
    /// Each state should implement Enter, Update, and Exit methods
    /// </summary>
    public interface IGameState
    {
        void Enter();
        void Update();
        void Exit();
    }
}