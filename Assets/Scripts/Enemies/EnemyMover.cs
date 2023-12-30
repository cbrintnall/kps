using UnityEngine;

public abstract class EnemyMover : MonoBehaviour
{
    public bool AtTarget { get; protected set; }
    public abstract void Move();
}
