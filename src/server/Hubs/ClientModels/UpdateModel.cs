namespace IOGameServer.Hubs.ClientModels
{
    public class UpdateModel
    {
        public double T { get; init; }
        public Player Me { get; init; }
        public IEnumerable<Player> P { get; init; }
        public IEnumerable<GameObject> B { get; init; }
        public IEnumerable<LeaderBoard> L { get; init; }

        public class Player : GameObject
        {
            public double Direction { get; init; }
            public double Hp { get; init; }
        }

        public class GameObject
        {
            public string Id { get; init; }
            public int X { get; init; }
            public int Y { get; init; }
        }

        public class LeaderBoard
        {
            public string Username { get; init; }
            public int Score { get; init; }
        }
    }

}