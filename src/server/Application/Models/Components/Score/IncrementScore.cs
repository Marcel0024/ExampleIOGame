﻿namespace IOGameServer.Application.Models.Components.Score
{
    public sealed class ScoreIncrementPerSecond : Component
    {
        public required int ScorePerSecond { get; init; }
        public double Score { get; set; } = 0;

        public ScoreIncrementPerSecond(IGameObject gameObject) : base(gameObject) { }

        public override void Start() { }

        public override void Update(double distance)
        {
            IncreaseScore(distance * ScorePerSecond);
        }

        public void IncreaseScore(double score)
        {
            Score += score;
        }
    }
}
