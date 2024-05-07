namespace IOGameServer.Application.Models.Components;

public abstract class Component(IGameObject gameObject) : IComponent
{
    public IGameObject GameObject { get; protected set; } = gameObject;

    public abstract void Start();

    public abstract void Update(double distance);
}
