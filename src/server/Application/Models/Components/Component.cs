namespace IOGameServer.Application.Models.Components
{
    public abstract class Component<T> : IComponent<T> where T : IGameObject
    {
        public T GameObject { get; protected set; }

        public Component(T gameObject)
        {
            GameObject = gameObject;
        }

        public abstract void Start();

        public abstract void Update(double distance);
    }
}
