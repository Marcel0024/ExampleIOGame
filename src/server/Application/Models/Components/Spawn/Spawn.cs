namespace IOGameServer.Application.Models.Components.Spawn;

public abstract class SpawnComponent(IGameObject gameObject) : Component(gameObject)
{
    public override void Start()
    {
        Spawn();
    }

    public override void Update(double _) { }

    public abstract void Spawn();
}
