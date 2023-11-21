using System;
using System.Collections.Generic;
using Cinemachine;

public class UpgradePipelineData
{
    public StatBlock PlayerStats;
    public CinemachineImpulseSource CameraShake;
    public Gun ShotFrom;
    private Dictionary<Type, object> custom = new();

    public T GetCustom<T>()
        where T : class, new()
    {
        if (custom.TryGetValue(typeof(T), out object val))
        {
            return val as T;
        }
        else
        {
            var newCustomVal = new T();
            custom[typeof(T)] = newCustomVal;
            return newCustomVal;
        }
    }
}

public class KickPipelineData : UpgradePipelineData
{
    public IInteractable LookingAt;
    public Health[] Health;
    public LookData LookData;
    public PlayerMovement PlayerMovement;
}
