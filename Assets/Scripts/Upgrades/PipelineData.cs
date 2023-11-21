using Cinemachine;

public class UpgradePipelineData
{
    public StatBlock PlayerStats;
    public CinemachineImpulseSource CameraShake;
    public Gun ShotFrom;
}

public class KickPipelineData : UpgradePipelineData
{
    public IInteractable LookingAt;
    public Health[] Health;
    public LookData LookData;
    public PlayerMovement PlayerMovement;
}
