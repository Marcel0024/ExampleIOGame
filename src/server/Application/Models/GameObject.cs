namespace IOGameServer.Application.Models
{
    public abstract class GameObject
    {
        public required string Id { get; init; }
        public required int X { get; set; }
        public required int Y { get; set; }
        public required double Direction { get; set; }
        public int Speed { get; set; }

        public virtual void Update(double distance)
        {
            X = (int)(X + Speed * distance * Math.Sin(Direction));
            Y = (int)(Y - Speed * distance * Math.Cos(Direction));
        }

        public double DistanceTo(GameObject @object)
        {
            var dx = X - @object.X;
            var dy = Y - @object.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public bool ReachedBorder(int mapSize)
        {
            return X <= 0 
                || Y <= 0
                || X >= mapSize 
                || Y >= mapSize;
        }

        public void SetDirection(double direction)
        {
            Direction = direction;
        }
    }
}
