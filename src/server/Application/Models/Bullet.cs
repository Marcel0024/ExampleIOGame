using IOGameServer.Hubs.ClientModels;

namespace IOGameServer.Application.Models
{
    public sealed class Bullet : GameObject
    {
        public required string PlayerId { get; init; }
        
        public UpdateModel.Bullet GetClientModel() => new()
        {
            Id = Id,
            X = X,
            Y = Y
        };
    }
}
