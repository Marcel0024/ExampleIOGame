namespace IOGameServer.Hubs
{
    public interface IGameHub
    {
        Task GameOver();
        Task GameUpdate(string json);
    }
}
