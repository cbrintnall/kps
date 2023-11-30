using System.Diagnostics;
using System.Reflection;

/// <summary>
/// A dummy upgrade, since all upgrades inherit from a certain class
///
/// The behavior applies the fields to this and then immediately applies
/// the stat upgrades. This is used to encapsulate all stat upgrades, like
/// health, damage, explosive size etc.
/// </summary>
public class StatChanger : Upgrade
{
    public string FieldName;
    public int Amount;
    public bool IsPercentage;

    public override void OnExisting(PlayerEquipmentController controller, UpgradeData data)
    {
        base.OnExisting(controller, data);
        HandleUpgrade(controller);
    }

    public override void OnPickup(PlayerEquipmentController controller)
    {
        base.OnPickup(controller);
        HandleUpgrade(controller);
    }

    void HandleUpgrade(PlayerEquipmentController controller)
    {
        FieldInfo field = controller.Stats.GetType().GetField(FieldName);
        object value = field.GetValue(controller.Stats);

        if (value is StatFloat sf)
        {
            sf.Incr(
                (float)Amount / (IsPercentage ? 100.0f : 1.0f),
                IsPercentage ? StatOperation.Percent : StatOperation.Value
            );
        }
        else if (value is StatInt si)
        {
            Debug.Assert(!IsPercentage);
            si.Incr(Amount, StatOperation.Value);
        }
    }
}
