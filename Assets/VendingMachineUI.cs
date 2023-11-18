using TMPro;
using UnityEngine;

public class VendingMachineUI : MonoBehaviour
{
    private string TEMPLATE =
        @"<size=""16"">{0}</size>
<size=""8"">({1})
${2}</size>
-------
<size=""12"">{3}</size>
-------<size=""6"">
{4}
</size>
";

    private string OOS = @"<size=""16"">Out of Stock!</size>";

    public TextMeshProUGUI Text;

    public UpgradeData Upgrade
    {
        set { Sync(value); }
    }

    void Update() { }

    void Sync(UpgradeData upgrade)
    {
        if (upgrade != null)
        {
            Text.text = string.Format(
                TEMPLATE,
                upgrade.Name,
                upgrade.Rarity,
                upgrade.Cost,
                upgrade.Description,
                "TODO!"
            );
        }
        else
        {
            Text.text = OOS;
        }
    }
}
