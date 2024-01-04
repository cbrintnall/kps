using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public DeathPanel DeathPanel;
    public AudioClip LogsSuccessSound;
    public TextMeshProUGUI UpgradeText;
    public TextMeshProUGUI LogText;
    public TextMeshProUGUI LevelText;

    FlowManager flowManager;

    List<string> logs = new();
    int errorCount = 0;
    int autoErrorCapture = 15;
    TimeSince ts;

    void Awake()
    {
        flowManager = SingletonLoader.Get<FlowManager>();
        var eventManager = SingletonLoader.Get<EventManager>();

        eventManager.Subscribe<PlayerSpawnedEvent>(data =>
        {
            gameObject.SetActive(true);
        });

        eventManager.Subscribe<PlayerDeathEvent>(data =>
        {
            Instantiate(DeathPanel, transform);
        });

        eventManager.Subscribe<PlayerLeveledEvent>(data =>
        {
            LevelText.text = "Level " + data.ToLevel;
        });

        Application.logMessageReceived += (string condition, string stackTrace, LogType type) =>
        {
            string line = $"\n[{DateTime.Now} | {type}]: {condition}\n---";
            if (type == LogType.Error || type == LogType.Assert)
            {
                line += $"\n{stackTrace}";
            }

            if (type == LogType.Error)
            {
                errorCount++;
                if (errorCount >= autoErrorCapture && ts > 120)
                {
                    UploadLogs($"Auto-captured, errors=#{errorCount}, seconds since last={ts}");
                    ts = 0;
                }
            }
        };

        gameObject.SetActive(false);
    }

    void Start()
    {
        LevelText.text = "Level " + PlayerEquipmentController.Instance.level;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            UploadLogs("Uploaded, thank you!");
        }

        var upgrades = PlayerUpgradeManager.Instance;
        if (upgrades.Upgrades > 0)
        {
            UpgradeText.gameObject.SetActive(true);
            if (upgrades.Upgrades > 1)
            {
                UpgradeText.text = $"{upgrades.Upgrades} Upgrades Remaining, press \"T\"";
            }
            else
            {
                UpgradeText.text = $"{upgrades.Upgrades} Upgrade Remaining, press \"T\"";
            }
        }
        else
        {
            UpgradeText.gameObject.SetActive(false);
        }
    }

    void UploadLogs(string message)
    {
        return;

        using (var client = new WebClient())
        {
            var form = new NameValueCollection
            {
                { "api_dev_key", flowManager.GameData.Debug.PasteBinKey },
                { "api_option", "paste" },
                { "api_user_key", flowManager.GameData.Debug.PasteBinUserKey },
                { "api_paste_code", $"'{string.Join("", logs)}'" }
            };

            var resp = Encoding.UTF8.GetString(
                client.UploadValues(flowManager.GameData.Debug.PasteBinUrl, form)
            );

            if (resp.Contains("pastebin"))
            {
                GUIUtility.systemCopyBuffer = resp;
                LogText.text = message;
                this.WaitThen(3.0f, () => LogText.text = "");
            }

            SingletonLoader
                .Get<AudioManager>()
                .Play(new AudioPayload() { Clip = LogsSuccessSound });
        }
    }
}
