namespace IOGameServer.Application.Models.Components
{
    public interface IComponent
    {
        IGameObject GameObject { get; }

        void Start();
        void Update(double distance);
    }
}
