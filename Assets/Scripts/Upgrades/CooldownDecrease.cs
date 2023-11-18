using UnityEngine;

public class CooldownDecrease : Upgrade
{
    public StatFloat ValueDecrease = 0.01f;

    public override void OnPickup(PlayerEquipmentController controller)
    {
        controller.Stats.PistolCooldown.Incr(-ValueDecrease, StatOperation.Value);
    }
}
