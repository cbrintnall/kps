using System;
using UnityEngine;
using UnityEngine.Pool;

public class HitPayload
{
    public GameObject Owner;
    public int Amount;
    public int Dealt;
    public int Remaining;
    public bool Crit;
}

public class DamagePayload
{
    public GameObject Owner;
    public int Amount;
    public bool WasCrit;
}

public class HealPayload
{
    public int Amount;
}

public class Health : MonoBehaviour
{
    public event Action<HitPayload> OnDamaged;

    public StatInt Data;
    public bool Dead => Data.Current <= 0;
    public bool Invincible;

    public void Damage(int amount)
    {
        if (Invincible)
            return;
        Data.Incr(-amount, StatOperation.Value);
        OnDamaged?.Invoke(new HitPayload() { Amount = amount, Remaining = Data.Current });
    }

    public void Damage(DamagePayload payload)
    {
        if (Invincible)
            return;
        Data.Incr(-payload.Amount, StatOperation.Value);
        OnDamaged?.Invoke(
            new HitPayload()
            {
                Amount = payload.Amount,
                Owner = payload.Owner,
                Remaining = Data.Current,
                Crit = payload.WasCrit
            }
        );
    }

    public void Heal(HealPayload payload)
    {
        Data.Incr(payload.Amount, StatOperation.Value);
    }

    public void InstaKill()
    {
        Damage(Data);
    }
}
