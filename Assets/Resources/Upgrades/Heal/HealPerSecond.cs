public class HealPerSecond : Upgrade
{
    public StatInt Amount = 1;
    public StatFloat Time = 1.0f;

    TimeSince ts;

    void Update()
    {
        if (ts > Time)
        {
            PlayerEquipmentController.Instance.Health.Heal(new HealPayload() { Amount = Amount });
            ts = 0;
        }
    }
}
