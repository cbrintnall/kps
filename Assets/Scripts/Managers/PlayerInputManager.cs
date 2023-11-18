using System;
using Unity.VisualScripting;
using UnityEngine;

// TODO: finish this class
public class PlayerAction<T>
    where T : unmanaged
{
    public event Action<T> Changed;
    public T Value;
    public bool Started;
    public bool Stopped;

    public static implicit operator T(PlayerAction<T> pa) => pa.Value;

    public virtual void UpdateValue(T value)
    {
        Value = value;
    }

    public void Reset() => Value = default;
}

public class PlayerBinaryAction : PlayerAction<bool>
{
    public float HoldTime
    {
        get => ts;
    }

    TimeSince ts;

    public override void UpdateValue(bool value)
    {
        Stopped = Value && !value;
        Started = !Value && value;

        if (Started)
            ts = 0;

        if (!Value)
            ts = 0;

        base.UpdateValue(value);
    }
}

[@Singleton]
public class PlayerInputManager : MonoBehaviour
{
    public PlayerBinaryAction OnInteract = new();
    public PlayerBinaryAction OnPrimaryAction = new();
    public PlayerBinaryAction Jumping = new();
    public PlayerBinaryAction StartRound = new();
    public Vector2 MoveDir;
    public Vector3 LookDir;

    private System.Collections.Generic.Stack<CursorLockMode> CursorStack = new();
    private bool debug;

    void Awake()
    {
        // FindObjectOfType<IngameDebugConsole.DebugLogManager>().gameObject.SetActive(false);
    }

    public void PushCursor(CursorLockMode mode)
    {
        CursorStack.Push(mode);
        Cursor.lockState = mode;
    }

    public void PopCursor() { }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            debug = !debug;

            if (debug)
            {
                OnInteract.Reset();
                OnPrimaryAction.Reset();
                Jumping.Reset();
                StartRound.Reset();
            }
        }

        if (debug)
            return;

        LookDir = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        MoveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        OnPrimaryAction.UpdateValue(Input.GetMouseButton(0));
        OnInteract.UpdateValue(Input.GetKeyDown(KeyCode.F));
        Jumping.UpdateValue(Input.GetKey(KeyCode.Space));
        StartRound.UpdateValue(Input.GetKeyDown(KeyCode.U));

        // if (Input.GetKey(KeyCode.F12) && Input.GetKey(KeyCode.RightControl))
        //     FindObjectOfType<IngameDebugConsole.DebugLogManager>().gameObject.SetActive(true);
    }
}
