using IOGameServer.Application.Models.GameObjects;

namespace IOGameServer.Application.Models.Components.Health;

public sealed class Health(Player gameObject) : Component(gameObject)
{
    public required int HP { get; set; }

    public override void Start() { }

    public override void Update(double _) { }

    public void Heal(int amount)
    {
        HP += amount;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            GameObject.RemoveMe();
        }
    }
}
