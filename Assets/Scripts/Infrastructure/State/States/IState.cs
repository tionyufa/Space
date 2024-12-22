namespace Infrastructure.State
{
    public interface IState
    {
        void Enter();
        void Exit();
    }
}