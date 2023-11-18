public class ApplyStatus : Upgrade
{
    public StatFloat ApplyChance = 0.0f;

    private float time = 3.0f;

    public override void OnWillShootBullet(UpgradePipelineData pipelineData, Bullet bullet)
    {
        base.OnWillShootBullet(pipelineData, bullet);

        if (Utilities.Randf() < ApplyChance)
        {
            bullet.Hit += (hit) =>
            {
                if (hit.Health)
                {
                    var burnstatus = Status.Apply<Burning>(hit.Health.gameObject);
                    burnstatus.Time = time;
                }
            };
        }
    }
}
