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

        broken = true;
        var rb = GetComponent<Rigidbody>();

        gameObject.layer = LayerMask.NameToLayer("Nothing");
        foreach (Door door in Others)
        {
            var otherrb = door.GetComponent<Rigidbody>();
            otherrb.isKinematic = false;
            otherrb.AddForce(payload.LookDir * HitForce, ForceMode.Impulse);
            otherrb.gameObject.layer = LayerMask.NameToLayer("Nothing");
        }
        rb.isKinematic = false;
        rb.AddForce(payload.LookDir.normalized * HitForce, ForceMode.Impulse);
    }
}
