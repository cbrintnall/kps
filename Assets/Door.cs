using UnityEngine;

public class DoorKickedEvent : BaseEvent { }

public class Door : MonoBehaviour, IInteractable
{
    public float HitForce = 500.0f;
    public AudioClip HitSound;
    public AudioClip Chime;
    public Door[] Others;

    bool broken = false;

    public InteractionData GetData()
    {
        return new InteractionData();
    }

    public void Interact(InteractionPayload payload)
    {
        if (broken)
            return;

        this.PlayAtMe(HitSound);

        SingletonLoader.Get<AudioManager>().Play(new AudioPayload() { Clip = Chime, Is2D = true });
        SingletonLoader.Get<EventManager>().Publish(new DoorKickedEvent());

        Break(payload);

        foreach (Door door in Others)
        {
            door.Break(payload);
        }
    }

    protected void Break(InteractionPayload payload)
    {
        broken = true;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(payload.LookDir.normalized * HitForce, ForceMode.Impulse);
        this.WaitThen(15.0f, () => Destroy(gameObject));
    }
}
