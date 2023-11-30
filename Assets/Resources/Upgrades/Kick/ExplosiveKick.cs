using UnityEngine;

public class ExplosiveKick : Upgrade
{
    public override void OnKick(KickPipelineData data)
    {
        base.OnKick(data);

        if (data.Health.Length == 0)
            return;

        if (Random.Range(1, 100) > PlayerEquipmentController.Instance.Stats.KickExplosion)
            return;

        var explosion = Instantiate(
            Resources.Load<Explosion>("Upgrades/Explosion"),
            transform.position,
            Quaternion.identity
        );
        explosion.Owner = gameObject;
        explosion.Damage = PlayerEquipmentController.Instance.Stats.ExplosionDamage;
        explosion.Layers = LayerMask.GetMask("Enemy");
        explosion.Size = PlayerEquipmentController.Instance.Stats.ExplosionSize;
    }
}
