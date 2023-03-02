namespace IOGameServer.Application.Models.Components.Movement
{
    public sealed class MovementNormal : Component
    {
        public required int Speed { get; init; }
        public required double Direction { get; set; }

        public MovementNormal(IGameObject gameObject) : base(gameObject) { }

        public override void Start() { }

        public override void Update(double distance)
        {
            GameObject.X = (int)(GameObject.X + Speed * distance * Math.Sin(Direction));
            GameObject.Y = (int)(GameObject.Y - Speed * distance * Math.Cos(Direction));
        }

        public void SetDirection(double direction)
        {
            Direction = direction;
        }
    }
}
