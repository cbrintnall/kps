using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinningWeapons : Upgrade
{
    public StatFloat RespawnDelay = 5.0f;
    public StatInt WeaponCount = new(1, 99, 0);
    public StatInt DamageEach = 1;
    public StatFloat Distance = 3.0f;
    public DamageOnTouch Prefab;
    public string Path = "";

    TimeSince lastSpawn;
    List<DamageOnTouch> children = new();
    Stack<Action> removal = new();

    void Awake()
    {
        lastSpawn = 0;
    }

    void Start()
    {
        Prefab = Resources.Load<DamageOnTouch>(Path);
    }

    void Update()
    {
        if (children.Count < WeaponCount && lastSpawn > RespawnDelay)
        {
            lastSpawn = 0;
            var child = Instantiate(Prefab, transform);
            child.transform.localPosition = Vector3.zero;
            children.Add(child);
            child.Destroyed += () => removal.Push(() => children.Remove(child));
        }

        while (removal.TryPop(out Action res))
        {
            res.Invoke();
        }

        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];
            var val =
                (Mathf.PI * 2.0f * ((float)i / (float)children.Count)) + Time.timeSinceLevelLoad;
            child.transform.localPosition =
                new Vector3(Mathf.Cos(val), 0.0f, Mathf.Sin(val)) * Distance;
        }
    }
}
