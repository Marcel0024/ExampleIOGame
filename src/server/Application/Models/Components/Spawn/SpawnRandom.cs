namespace IOGameServer.Application.Models.Components.Spawn
{
    public sealed class SpawnRandom : SpawnComponent
    {
        public SpawnRandom(IGameObject gameObject) : base(gameObject) { }

        public override void Spawn()
        {
            GameObject.X = Random.Shared.Next(0, GameObject.Game.Settings.MapSize);
            GameObject.Y = Random.Shared.Next(0, GameObject.Game.Settings.MapSize);
        }
    }
}
