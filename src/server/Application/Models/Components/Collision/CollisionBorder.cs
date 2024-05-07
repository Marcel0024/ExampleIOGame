namespace IOGameServer.Application.Models.Components.Collision;

public abstract class CollisionBorder(IGameObject gameObject) : Component(gameObject)
{
    protected abstract void HandleReachedBorder();

    public override void Start() { }

    public override void Update(double _)
    {
        if (HasReachedBorder())
        {
            HandleReachedBorder();
        }
    }

    private bool HasReachedBorder()
    {
        return GameObject.X <= 0
            || GameObject.Y <= 0
            || GameObject.X >= GameObject.Game.Settings.MapSize
            || GameObject.Y >= GameObject.Game.Settings.MapSize;
    }
}
