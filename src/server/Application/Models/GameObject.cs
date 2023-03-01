﻿using IOGameServer.Application.Models.Components;
using IOGameServer.Application.Models.Components.Collision;
using IOGameServer.Application.Models.Inputs;

namespace IOGameServer.Application.Models
{
    public abstract class GameObject : IGameObject
    {
        public Game Game { get; init; }
        public required string Id { get; init; }
        public int X { get; set; }
        public int Y { get; set; }

        public IDictionary<Type, IComponent<IGameObject>> Components { get; set; }
            = new Dictionary<Type, IComponent<IGameObject>>(15);

        public GameObject(Game game)
        {
            Game = game;
        }

        public virtual void Start()
        {
            foreach (var component in Components.Values)
            {
                component.Start();
            }
        }

        public virtual void Update(double distance)
        {
            foreach (var component in Components.Values)
            {
                component.Update(distance);
            }
        }

        public double DistanceTo(IGameObject gameObject)
        {
            var dx = X - gameObject.X;
            var dy = Y - gameObject.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public bool Collided(IGameObject gameObject)
        {
            return GetComponent<CollisionObject>().Collided(gameObject);
        }

        public void RemoveMe()
        {
            Game.MarkToRemoveGameObjects.Add(this);
        }

        public void AddItemToGame(IGameObject gameObject)
        {
            Game.MarkToAddGameObjects.Add(gameObject);
        }

        public void HandleCollision(IGameObject gameObject)
        {
            GetComponent<CollisionObject>()?
                .HandleCollision(gameObject);
        }

        public abstract void HandleInput(IInput input);

        public T GetComponent<T>() where T : IComponent<IGameObject>
        {
            if (Components.TryGetValue(typeof(T), out IComponent<IGameObject> component))
            {
                return (T)component;
            }
            else
            {
                return default;
            }
        }

        public void AddComponent(IComponent<IGameObject> component)
        {
            Components.Add(component.GetType(), component);
        }
    }
}
