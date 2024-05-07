namespace IOGameServer.Application.Models.Components.Spawn;

public sealed class SpawnFixed(IGameObject gameObject, int x, int y) : SpawnComponent(gameObject)
{
    public int XSpawn { get; set; } = x;
    public int YSpawn { get; set; } = y;

    public override void Spawn()
    {
        GameObject.X = XSpawn;
        GameObject.Y = YSpawn;
    }
}
