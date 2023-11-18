using System;
using System.Collections;
using UnityEngine;

public class BulletHitCountBuff : Upgrade
{
    private static StatFloat BuffTime = 5.0f;
    private static StatFloat Chance = 0.02f;
    private static StatInt IncreaseAmount = 1;
    private Coroutine buff;
    private int queue = 0;

    public override void OnBulletHit(UpgradePipelineData pipelineData, BulletHitData data)
    {
        if (Utilities.Randf() < Chance)
        {
            if (buff == null)
            {
                buff = StartCoroutine(HandleBuff());
            }
            else
            {
                queue++;
            }
        }
    }

    public override void OnPickup(PlayerEquipmentController controller) { }

    protected override void OnCountIncrease(int count)
    {
        base.OnCountIncrease(count);

        if (count % 5 == 0)
        {
            IncreaseAmount.Incr(1, StatOperation.Value);
        }
        else
        {
            Chance.Incr(0.02f, StatOperation.Value);
        }
    }

    IEnumerator HandleBuff()
    {
        AudioPayload payload = new AudioPayload()
        {
            Clip = Resources.Load<AudioClip>("Audio/Drink Potion 4"),
            Transform = transform
        };

        SingletonLoader.Get<AudioManager>().Play(payload);

        int amountChanged = IncreaseAmount;

        // Gun.ShootPattern.PerShot.Incr(amountChanged, StatOperation.Value);

        yield return new WaitForSeconds(BuffTime);

        // Gun.ShootPattern.PerShot.Incr(-amountChanged, StatOperation.Value);

        if (queue > 0)
        {
            buff = StartCoroutine(HandleBuff());
            queue--;
        }
    }
}
