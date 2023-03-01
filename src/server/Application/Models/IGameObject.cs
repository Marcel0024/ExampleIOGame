using IOGameServer.Application.Models.Components;
using IOGameServer.Application.Models.Inputs;

namespace IOGameServer.Application.Models
{
    public interface IGameObject
    {
        Game Game { get; }
        string Id { get; init; }
        int X { get; set; }
        int Y { get; set; }

        IDictionary<Type, IComponent<IGameObject>> Components { get; }

        void Start();
        void Update(double distance);
        double DistanceTo(IGameObject @object);
        bool HasCollidedWith(IGameObject @object);
        void HandleCollisionImpact(IGameObject @object);
        void HandleInput(IInput input);
        void RemoveMe();
        void AddItemToGame(IGameObject @object);

        T GetComponent<T>() where T : IComponent<IGameObject>;
        void AddComponent(IComponent<IGameObject> component);
    }
}
