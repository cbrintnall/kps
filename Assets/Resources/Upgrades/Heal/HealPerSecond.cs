public class HealPerSecond : Upgrade
{
    public StatInt Amount;
    public float Time;

    TimeSince ts;

    void Update()
    {
        if (ts > Time)
        {
            controller.Health.Heal(new HealPayload() { Amount = Amount });
            ts = 0;
        }
    }
}
