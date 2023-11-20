using UnityEngine;

public class Revolver : Gun
{
    const int MAX_SHOT = 4;

    int shot = -1;
    int pattern = 0;

    public override Bullet Shoot(Vector3 variance, Bullet overridebullet = null)
    {
        var bullet = base.Shoot(variance, overridebullet);

        shot = (shot + 1) % MAX_SHOT;
        Animator.SetInteger("BarrelIdx", shot);

        return bullet;
    }

    protected override void OnShot()
    {
        if (ShootPatterns.Count == 0)
            return;

        pattern = (pattern + 1) % ShootPatterns.Count;
        ShootPatterns[pattern].Shoot((variance) => Shoot(variance, null), ShootFX);
    }
}
