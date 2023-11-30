using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UpgradePanelListener : MonoBehaviour
{
    public static UpgradePanelListener Instance;

    public GameObject UpgradesBase;
    public TextMeshProUGUI RemainingText;

    public int CreateCount = 4;
    public UpgradeSelection PanelPrefab;
    public bool IsOpen => panels.Count > 0;
    public event Action ClosedPanels;
    private CanvasGroup canvasGroup;

    List<UpgradeSelection> panels = new();

    void Awake()
    {
        Instance = this;
        // canvasGroup = GetComponent<CanvasGroup>();
        ClosePanels();
    }

    public void OpenPanels()
    {
        SingletonLoader.Get<UpgradesManager>().UpdateUpgradeList();
        gameObject.SetActive(true);
        // canvasGroup.blocksRaycasts = true;
        RemainingText.text = "Choices Remaining\n" + PlayerUpgradeManager.Instance.Upgrades;
        for (int i = 0; i < CreateCount; i++)
        {
            var panel = Instantiate(PanelPrefab, transform);
            panel.transform.SetParent(UpgradesBase.transform);
            panel.OnChoose += ClosePanels;
            panels.Add(panel);
        }

        SingletonLoader.Get<PlayerInputManager>().PushCursor(CursorLockMode.Confined);

        DOTween.To(() => Time.timeScale, scale => Time.timeScale = scale, 0.01f, 0.25f);
    }

    public void ClosePanels()
    {
        gameObject.SetActive(false);
        foreach (var panel in panels)
        {
            Destroy(panel.gameObject);
        }

        panels = new();
        SingletonLoader.Get<PlayerInputManager>().PopCursor();
        DOTween.To(() => Time.timeScale, scale => Time.timeScale = scale, 1.0f, 0.75f);
        // canvasGroup.blocksRaycasts = false;
        ClosedPanels?.Invoke();
    }
}
