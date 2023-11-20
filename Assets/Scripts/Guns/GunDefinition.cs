using System;
using System.Collections.Generic;
using UnityEngine;

public class GunDefinition
{
    public Dictionary<string, object> Stats;
    public string Name;
    public Type Class;
    public string[] Upgrades;
    public GameObject Base;

    public Gun Create()
    {
        GameObject gunBase = GameObject.Instantiate(Base);
        Gun gun = gunBase.AddComponent(Class) as Gun;
        UpgradesManager upgrades = SingletonLoader.Get<UpgradesManager>();

        foreach (var upgrade in Upgrades)
        {
            if (upgrades.TryGetUpgrade(upgrade, out var data))
            {
                PlayerEquipmentController.Instance.AddUpgrade(data);
            }
        }

        return gun;
    }
}
