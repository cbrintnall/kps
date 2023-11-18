using UnityEngine;

public class Burning : Status
{
    public StatFloat TickTime = 0.25f;
    public StatInt DamagePerTick = 1;
    public GameObject BurningPrefab;

    TimeSince tick;
    Health targetHealth;
    GameObject burnfx;

    protected override void OnApply()
    {
        BurningPrefab = Resources.Load<GameObject>("FX/Fire");
        targetHealth = target.GetComponent<Health>();
        burnfx = Instantiate(BurningPrefab);
        burnfx.transform.SetParent(targetHealth.transform);
        burnfx.transform.localPosition = Vector3.zero;
    }

    protected override void OnFinish()
    {
        base.OnFinish();

        Destroy(burnfx);
    }

    protected override void OnStack()
    {
        base.OnStack();

        DamagePerTick.Incr(1);
    }

    protected override void Update()
    {
        base.Update();

        if (tick > TickTime)
        {
            targetHealth.Damage(DamagePerTick);
            tick = 0;
        }
    }
}
