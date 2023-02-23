namespace IOGameServer.Hubs.ClientModels
{
    public readonly struct UpdateModel
    {
        public double T { get; init; }
        public Player Me { get; init; }
        public IEnumerable<Player> P { get; init; }
        public IEnumerable<Bullet> B { get; init; }
        public IEnumerable<LeaderBoard> L { get; init; }

        public readonly struct Player
        {
            public string Id { get; init; }
            public int X { get; init; }
            public int Y { get; init; }
            public double Dir { get; init; }
            public double Hp { get; init; }
        }

        public readonly struct Bullet
        {
            public string Id { get; init; }
            public int X { get; init; }
            public int Y { get; init; }
        }

        public readonly struct LeaderBoard
        {
            public string Name { get; init; }
            public int Score { get; init; }
        }
    }

}