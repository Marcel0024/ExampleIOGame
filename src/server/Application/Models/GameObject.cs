using IOGameServer.Application.Models.Components;
using IOGameServer.Application.Models.Components.Collision;
using IOGameServer.Application.Models.Inputs;

namespace IOGameServer.Application.Models;

public abstract class GameObject(Game game) : IGameObject
{
    public Game Game { get; init; } = game;
    public required string Id { get; init; }
    public int X { get; set; }
    public int Y { get; set; }

    public IDictionary<Type, IComponent> Components { get; set; } = new Dictionary<Type, IComponent>(15);

    public void Start()
    {
        foreach (var component in Components)
        {
            component.Value.Start();
        }
    }

    public void Update(double distance)
    {
        foreach (var component in Components)
        {
            component.Value.Update(distance);
        }
    }

    public double DistanceTo(IGameObject gameObject)
    {
        var dx = X - gameObject.X;
        var dy = Y - gameObject.Y;

        return Math.Sqrt(dx * dx + dy * dy);
    }

    public bool HasCollidedWith(IGameObject gameObject)
    {
        return GetComponent<CollisionObject>().Collided(gameObject);
    }

    public virtual void RemoveMe()
    {
        Game.QueueToRemoveGameObjects.Enqueue(this);
    }

    public void AddItemToGame(IGameObject gameObject)
    {
        Game.QueueToAddGameObjects.Enqueue(gameObject);
    }

    public void HandleCollisionImpact(IGameObject gameObject)
    {
        GetComponent<CollisionObject>()?
            .HandleCollision(gameObject);
    }

    public abstract void HandleInput(IInput input);

    public T GetComponent<T>() where T : IComponent
    {
        if (Components.TryGetValue(typeof(T), out IComponent component))
        {
            return (T)component;
        }
        else
        {
            return default;
        }
    }

    public void AddComponent(IComponent component)
    {
        Components.Add(component.GetType(), component);
    }
}
