using System;
using System.Linq;
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
    public virtual Upgrade CreateForOwner(PlayerEquipmentController owner)
    {
        var upgrade = owner.gameObject.AddComponent(Data.Class) as Upgrade;
        Assert.IsNotNull(upgrade);

        var reserialized = JsonConvert.SerializeObject(Data.Stats);
        JsonConvert.PopulateObject(reserialized, upgrade);

        upgrade.OnPickup(owner);
        return upgrade;
    }

    public virtual void OnExisting(Upgrade upgrade)
    {
        foreach (var kv in Data.Stats)
        {
            var field = upgrade.GetType().GetField(kv.Key);
            var value = field.GetValue(upgrade);

            if (kv.Value is double || kv.Value is float)
            {
                (value as StatFloat).Incr(Convert.ToSingle(kv.Value), StatOperation.Value);
            }
            else if (kv.Value is long || kv.Value is int)
            {
                (value as StatInt).Incr(Convert.ToInt32(kv.Value), StatOperation.Value);
            }
            else
            {
                field.SetValue(upgrade, kv.Value);
            }
        }
    }

    public virtual bool CanBeSold()
    {
        bool canSell = true;

        if (PlayerEquipmentController.Instance == null)
        {
            return false;
        }

        if (Data.Requires != null && Data.Requires.Length > 0)
        {
            canSell = Data.Requires.All(
                id => PlayerEquipmentController.Instance.TryGetUpgrade(id, out var _)
            );

            if (!canSell)
            {
                Debug.Log($"Not selling {Data.GenerateId()}, player doesn't have pre-reqs.");
            }
        }

        return canSell;
    }

    public virtual void OnPurchase(TransactionPayload payload)
    {
        payload.Data.Behavior.OnPurchase(payload);
    }
}
