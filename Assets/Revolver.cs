using UnityEngine;

public class Revolver : Gun
{
    const int MAX_SHOT = 4;

    int shot = -1;
    int pattern = 0;

    public override Bullet Shoot(
        Vector3 variance,
        Bullet overridebullet = null,
        ShootPayload payload = null
    )
    {
        var bullet = base.Shoot(variance, overridebullet, payload);

        shot = (shot + 1) % MAX_SHOT;
        // Animator.SetInteger("BarrelIdx", shot);

        return bullet;
    }
}
