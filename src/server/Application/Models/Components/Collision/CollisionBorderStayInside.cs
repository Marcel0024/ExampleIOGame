﻿namespace IOGameServer.Application.Models.Components.Collision
{
    public sealed class CollisionBorderStayInside : CollisionBorder
    {
        public CollisionBorderStayInside(IGameObject gameObject) : base(gameObject) { }

        protected override void HandleReachedBorder()
        {
            GameObject.X = Math.Max(5, Math.Min(GameObject.Game.Settings.MapSize, GameObject.X));
            GameObject.Y = Math.Max(5, Math.Min(GameObject.Game.Settings.MapSize, GameObject.Y));
        }
    }
}
