namespace IOGameServer.Application.Models.Components.Collision;

public sealed class CollisionBorderRemoveOnTouch(IGameObject gameObject) : CollisionBorder(gameObject)
{
    protected override void HandleReachedBorder()
    {
        GameObject.RemoveMe();
    }
}
