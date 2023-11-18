using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PowerupType
{
    HEAL,
    SHOTSPEED
}

public class Powerup : MonoBehaviour
{
    public PowerupType Type;
    public AudioClip PickupSound;

    public GameObject Heal;
    public GameObject Speed;

    public int HealAmount = 1;
    public float SpeedDuration = 5.0f;
    public float SpeedMultiplier = 2.0f;

    void Start()
    {
        Type = Utilities.Randf() <= 0.5f ? PowerupType.HEAL : PowerupType.SHOTSPEED;

        switch (Type)
        {
            case PowerupType.SHOTSPEED:
                Heal.SetActive(false);
                Speed.SetActive(true);
                break;
            case PowerupType.HEAL:
                Heal.SetActive(true);
                Speed.SetActive(false);
                break;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        switch (Type)
        {
            case PowerupType.SHOTSPEED:
                break;
            case PowerupType.HEAL:
                break;
        }

        SingletonLoader
            .Get<AudioManager>()
            .Play(new AudioPayload() { Clip = PickupSound, Location = transform.position });

        Destroy(gameObject);
    }
}
