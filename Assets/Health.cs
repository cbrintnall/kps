using System;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.Pool;

public class HitPayload
{
    public GameObject Owner;
    public int Amount;
    public int Dealt;
    public int Remaining;
}

public class DamagePayload
{
    public GameObject Owner;
    public int Amount;
}

public class HealPayload
{
    public int Amount;
}

public class Health : MonoBehaviour
{
    public event Action<HitPayload> OnDamaged;

    public StatInt Data;

    public void Damage(int amount)
    {
        Data.Incr(-amount, StatOperation.Value);
        OnDamaged?.Invoke(new HitPayload() { Amount = amount, Remaining = Data.Current });
    }

    public void Damage(DamagePayload payload)
    {
        Data.Incr(-payload.Amount, StatOperation.Value);
        OnDamaged?.Invoke(
            new HitPayload()
            {
                Amount = payload.Amount,
                Owner = payload.Owner,
                Remaining = Data.Current
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
