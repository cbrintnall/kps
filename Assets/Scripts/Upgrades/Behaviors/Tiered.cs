using System;
using Newtonsoft.Json;
using UnityEngine;

public class TieredUpgradeBehavior : UpgradeBehavior
{
    public TieredUpgradeBehavior(UpgradeData data)
        : base(data)
    {
        Debug.Log($"Creating tiered behavior for {data}");
    }

    public override bool CanBeSold()
    {
        if (
            PlayerEquipmentController.Instance != null
            && PlayerEquipmentController.Instance.TryGetUpgrade(
                Data.Class,
                out UpgradeStorage upgrade
            )
        )
        {
            Debug.Log(
                $"Player has previous tier ({upgrade.Data.GenerateBaseId()}), allowing {Data.GenerateBaseId()}"
            );

            return upgrade.Data.Next == Data;
        }

        // only allow common if player hasn't gotten any
        return Data.Rarity == UpgradeRarity.COMMON;
    }

    public override void OnExisting(Upgrade upgrade)
    {
        var reserialized = JsonConvert.SerializeObject(Data.Stats);

        JsonConvert.PopulateObject(reserialized, upgrade);
        // foreach (var kv in Data.Stats)
        // {
        //     var field = upgrade.GetType().GetField(kv.Key);
        //     var value = field.GetValue(upgrade);

        //     if (kv.Value is double f)
        //     {
        //         (value as StatFloat).Set(Convert.ToSingle(f));
        //     }
        //     else if (kv.Value is long i)
        //     {
        //         (value as StatInt).Set(Convert.ToInt32(i));
        //     }
        //     else
        //     {
        //         field.SetValue(upgrade, kv.Value);
        //     }
        // }
    }

    public override void OnPurchase(TransactionPayload payload)
    {
        base.OnPurchase(payload);

        // Can only purchase one tier at a time.
        payload.Machine.ClearStock();
    }
}
