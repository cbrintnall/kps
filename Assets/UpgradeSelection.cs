using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.SmartFormat;
using UnityEngine.UI;

public class UpgradeChosenEvent : BaseEvent
{
    public UpgradeData Data;
}

public class UpgradeSelection : MonoBehaviour
{
    private string TEMPLATE =
        @"<color=#{Color}><size=""72"">{Name}</size></color>
-------
<size=""24"">{Description}</size>
";

    public AudioClip onChoose;
    public Image Header;
    public event Action OnChoose;

    [SerializeField]
    private TextMeshProUGUI Text;

    [SerializeField]
    private TextMeshProUGUI Description;

    [SerializeField]
    private Image HeaderPanel;

    [SerializeField]
    private Button button;
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

        var color = SingletonLoader.Get<FlowManager>().GameData.UpgradeColors[upgrade.Rarity];
        HeaderPanel.color = color;
        Header.color = color;
        Text.text = upgrade.Name;
        Description.text = Smart.Format(upgrade.Description, upgrade.Stats);
        button.onClick.AddListener(() =>
        {
            PlayerEquipmentController.Instance.AddUpgrade(upgrade);
            SingletonLoader
                .Get<EventManager>()
                .Publish(new UpgradeChosenEvent() { Data = upgrade });
            OnChoose?.Invoke();
            SingletonLoader.Get<AudioManager>().Play(onChoose);
            Destroy(gameObject);
        });
    }
}
