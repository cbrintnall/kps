using UnityEngine;

public class SpeedKick : Upgrade
{
    public StatFloat Force = 25.0f;

    public override void OnKick(KickPipelineData data)
    {
        base.OnKick(data);

        data.PlayerMovement.m_PlayerVelocity +=
            data.PlayerMovement.Camera.transform.forward * Force;
    }
}
