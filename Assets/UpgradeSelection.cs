using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.SmartFormat;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UpgradeChosenEvent : BaseEvent
{
    public UpgradeData Data;
}

public class UpgradeSelection : MonoBehaviour
{
    private string TEMPLATE =
        @"<size=""72"">{Name}</size>
<size=""48""><color=#{Color}>({Rarity})
${Cost}</color></size>
-------
<size=""32"">{Description}</size>
";

    public AudioClip onChoose;

    [SerializeField]
    private TextMeshProUGUI Text;

    [SerializeField]
    private UnityEngine.UI.Button button;
    private UpgradesManager upgradesManager;
    private UpgradeData upgrade;

    // Start is called before the first frame update
    void Start()
    {
        upgradesManager = SingletonLoader.Get<UpgradesManager>();

        if (upgradesManager.TryGetViableUpgrade(out UpgradeData data))
        {
            upgrade = data;
        }

        Text.text = Smart.Format(TEMPLATE, upgrade);
        button.onClick.AddListener(() =>
        {
            PlayerEquipmentController.Instance.AddUpgrade(upgrade);
            SingletonLoader
                .Get<EventManager>()
                .Publish(new UpgradeChosenEvent() { Data = upgrade });
            SingletonLoader.Get<AudioManager>().Play(onChoose);
            Destroy(gameObject);
        });
    }
}
