namespace IOGameServer.Application.Models.Components
{
    public abstract class Component : IComponent
    {
        public IGameObject GameObject { get; protected set; }

        public Component(IGameObject gameObject)
        {
            GameObject = gameObject;
        }

        public abstract void Start();

        public abstract void Update(double distance);
    }
}
