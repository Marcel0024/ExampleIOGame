namespace IOGameServer.Application.Models.Components
{
    public interface IComponent<out T> where T : IGameObject
    {
        T GameObject { get; }

        void Start();
        void Update(double distance);
    }
}
