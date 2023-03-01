namespace IOGameServer.Application.Models.Components.Collision
{
    public sealed class CollisionBorderRemoveOnTouch : CollisionBorder
    {
        public CollisionBorderRemoveOnTouch(IGameObject gameObject) : base(gameObject) { }

        protected override void HandleReachedBorder()
        {
            GameObject.RemoveMe();
        }
    }
}
