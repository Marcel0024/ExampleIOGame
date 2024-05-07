using IOGameServer.Hubs.ClientModels;

namespace IOGameServer.Hubs;

public interface IGameHub
{
    Task GameOver();
    Task GameUpdate(UpdateModel update);
}
