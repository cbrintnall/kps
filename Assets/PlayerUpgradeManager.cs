using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelPayload
{
    public Action OnCancel;
    public Action OnConfirm;
}

/// <summary>
/// Came a little late, a lot of responsibility is deferred
/// to the equipment manager, but this is here to stop that.
/// </summary>
public class PlayerUpgradeManager : MonoBehaviour
{
    public static PlayerUpgradeManager Instance;
    public int Upgrades = 0;
    PlayerInputManager inputManager;

    void Awake()
    {
        Instance = this;
        var eventManager = SingletonLoader.Get<EventManager>();
        inputManager = SingletonLoader.Get<PlayerInputManager>();
        eventManager.Subscribe<PlayerLeveledEvent>(data => Upgrades++);
        SingletonLoader.Get<EventManager>().Subscribe<UpgradeChosenEvent>(data => Upgrades--);
    }

    void Start()
    {
        UpgradePanelListener.Instance.ClosedPanels += () =>
        {
            if (Upgrades > 0)
            {
                UpgradePanelListener.Instance.OpenPanels();
            }
        };
    }

    void Update()
    {
        if (inputManager.OpenUpgrades && !UpgradePanelListener.Instance.IsOpen && Upgrades > 0)
        {
            UpgradePanelListener.Instance.OpenPanels();
        }
    }
}
