using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : Gun
{
    const int MAX_SHOT = 4;

    int shot = -1;

    public override Bullet Shoot(Vector3 variance, Bullet overridebullet = null)
    {
        var bullet = base.Shoot(variance, overridebullet);

        shot = (shot + 1) % MAX_SHOT;
        Animator.SetInteger("BarrelIdx", shot);

        return bullet;
    }
}
