using System.Collections.Generic;
using UnityEngine;

public class DropTurret : Upgrade
{
    public StatFloat DropTime = 50.0f;
    public StatInt Number = 1;
    public string Path;
    public ShootingTurret Prefab;

    TimeSince ts;
    Stack<ShootingTurret> turrets = new();

    void Start()
    {
        Prefab = Resources.Load<ShootingTurret>(Path);
        ts = 0;
    }

    void Update()
    {
        if (ts > DropTime)
        {
            if (turrets.Count >= Number)
            {
                if (turrets.TryPop(out ShootingTurret turret))
                {
                    Destroy(turret.gameObject);
                }
            }

            turrets.Push(Instantiate(Prefab, controller.GetGroundPosition(), Quaternion.identity));

            ts = 0;
        }
    }
}
