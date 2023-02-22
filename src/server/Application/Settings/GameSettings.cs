namespace IOGameServer.Application.Settings
{
    public sealed class GameSettings
    {
        public int TotalPlayersPerGame { get; init; }
        public int MapSize { get; init; }
        public int PlayerRadius { get; init; }
        public int PlayerMaxHP { get; init; }
        public int PlayerSpeed { get; init; }
        public double PlayerFireCooldown { get; init; }
        public int BulletRadius { get; init; }
        public int BulletSpeed { get; init; }
        public int BulletDamage { get; init; }
        public int ScoreBulletHit { get; init; }
        public int ScorePerSecond { get; init; }
    }
}
