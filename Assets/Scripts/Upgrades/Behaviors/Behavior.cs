using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;

public class UpgradeBehavior
{
    // TODO: we may want to decouple this from here, although it
    // just exists for ease of use
    public readonly UpgradeData Data;

    public UpgradeBehavior(UpgradeData data)
    {
        this.Data = data;
    }

    /// <summary>
    /// Creates a new version of the upgrade
    /// </summary>
    public virtual Upgrade CreateForOwner(GameObject owner)
    {
        var upgrade = owner.AddComponent(Data.Class) as Upgrade;
        Assert.IsNotNull(upgrade);

        var reserialized = JsonConvert.SerializeObject(Data.Stats);
        JsonConvert.PopulateObject(reserialized, upgrade);
        // foreach (var kv in Data.Stats)
        // {
        //     var field = upgrade.GetType().GetField(kv.Key);
        //     var value = field.GetValue(upgrade);

        //     if (kv.Value is double f)
        //     {
        //         var statint = value as StatFloat;
        //         field.SetValue(
        //             upgrade,
        //             new StatFloat(Convert.ToSingle(f), statint.Max, statint.Min)
        //         );
        //     }
        //     else if (kv.Value is long i)
        //     {
        //         var statint = value as StatInt;
        //         field.SetValue(upgrade, new StatInt(Convert.ToInt32(i), statint.Max, statint.Min));
        //     }
        //     else
        //     {
        //         try
        //         {
        //             field.SetValue(upgrade, kv.Value);
        //         }
        //         catch (Exception)
        //         {
        //             Debug.LogWarning(
        //                 $"Failed to set value for field '{upgrade.GetType()}:{field.GetType().Name}' with type '{value.GetType().Name}'"
        //             );
        //         }
        //     }
        // }

        return upgrade;
    }

    public virtual void OnExisting(Upgrade upgrade)
    {
        foreach (var kv in Data.Stats)
        {
            var field = upgrade.GetType().GetField(kv.Key);
            var value = field.GetValue(upgrade);

            if (kv.Value is double f)
            {
                (value as StatFloat).Incr(Convert.ToSingle(f), StatOperation.Value);
            }
            else if (kv.Value is long i)
            {
                (value as StatInt).Incr(Convert.ToInt32(i), StatOperation.Value);
            }
            else
            {
                field.SetValue(upgrade, kv.Value);
            }
        }
    }

    public virtual bool CanBeSold()
    {
        return true;
    }

    public virtual void OnPurchase(TransactionPayload payload) { }
}
