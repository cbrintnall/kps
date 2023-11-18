using UnityEngine;

public class Status : MonoBehaviour
{
    public StatFloat Time = 3.0f;

    protected TimeSince remaining;
    protected GameObject target;

    public static T Apply<T>(GameObject gameObject)
        where T : Status
    {
        if (gameObject.TryGetComponent<T>(out var comp))
        {
            comp.OnStack();
            return comp;
        }
        else
        {
            var status = gameObject.AddComponent<T>();
            status.target = gameObject;
            status.OnApply();
            return status;
        }
    }

    void Awake()
    {
        remaining = 0;
    }

    protected virtual void Update()
    {
        if (remaining > Time)
        {
            OnFinish();
            Destroy(this);
        }
    }

    protected virtual void OnApply() { }

    protected virtual void OnFinish() { }

    protected virtual void OnStack()
    {
        remaining = 0;
    }
}
