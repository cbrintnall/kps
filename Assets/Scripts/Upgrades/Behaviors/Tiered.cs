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
                $"Player has previous tier ({upgrade.Data.GenerateId()}), allowing {Data.GenerateId()}"
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
    }

    public override void OnPurchase(TransactionPayload payload)
    {
        base.OnPurchase(payload);

        // Can only purchase one tier at a time.
        // payload.Machine.ClearStock();
    }
}
