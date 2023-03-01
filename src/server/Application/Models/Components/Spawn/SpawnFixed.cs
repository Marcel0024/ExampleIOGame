namespace IOGameServer.Application.Models.Components.Spawn
{
    public sealed class SpawnFixed : SpawnComponent
    {
        public SpawnFixed(IGameObject gameObject, int x, int y) : base(gameObject)
        {
            XSpawn = x;
            YSpawn = y;
        }

        public int XSpawn { get; set; }
        public int YSpawn { get; set; }

        public override void Spawn()
        {
            GameObject.X = XSpawn;
            GameObject.Y = YSpawn;
        }
    }
}
