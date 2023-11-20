using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.SmartFormat;

public class VendingMachineUI : MonoBehaviour
{
    private string TEMPLATE =
        @"<size=""16"">{Name}</size>
<size=""8""><color=#{Color}>({Rarity})
${Cost}</color></size>
-------
<size=""12"">{Description}</size>
-------<size=""6"">
</size>
";

    private string OOS = @"<size=""16"">Out of Stock!</size>";
    private FlowManager flowManager;

    public TextMeshProUGUI Text;

    public UpgradeData Upgrade
    {
        set { Sync(value); }
    }

    void Awake()
    {
        flowManager = SingletonLoader.Get<FlowManager>();
    }

    void Update() { }

    void Sync(UpgradeData upgrade)
    {
        if (upgrade != null)
        {
            Text.text = Smart.Format(TEMPLATE, upgrade);
        }
        else
        {
            Text.text = OOS;
        }
    }
}
