using System.Diagnostics;
using System.Reflection;

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
            sf.Incr(Amount, IsPercentage ? StatOperation.Percent : StatOperation.Value);
        }
        else if (value is StatInt si)
        {
            Debug.Assert(!IsPercentage);
            si.Incr(Amount);
        }
    }
}
