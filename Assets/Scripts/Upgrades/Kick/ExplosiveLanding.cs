using UnityEngine;

public class ExplosiveLanding : Upgrade
{
    public StatFloat ExplosionMultiplier = 0.25f;

    public override void OnKick(KickPipelineData data)
    {
        base.OnKick(data);

        var explosion = Instantiate(
            Resources.Load<Explosion>("Upgrades/Explosion"),
            transform.position,
            Quaternion.identity
        );

        explosion.Layers = LayerMask.GetMask("Enemy");
        explosion.Damage = controller.Stats.GrenadeExplosiveDamage;
        explosion.Size = controller.Stats.ExplosionSize;
    }
}
