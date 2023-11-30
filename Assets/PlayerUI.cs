using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    const string UPLOAD_TXT = "Press 'F12' to upload logs";

    public DeathPanel DeathPanel;
    public AudioClip LogsSuccessSound;
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
            logs.Add($"\n[{DateTime.Now} | {type}]: {condition}\n---\n{stackTrace}\n");
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
    }

    void UploadLogs(string message)
    {
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
                this.WaitThen(3.0f, () => LogText.text = UPLOAD_TXT);
            }

            SingletonLoader
                .Get<AudioManager>()
                .Play(new AudioPayload() { Clip = LogsSuccessSound });
        }
    }
}
