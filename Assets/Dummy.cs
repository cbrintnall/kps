using UnityEngine;

public class Dummy : MonoBehaviour
{
    private Animator animator;
    private Health health;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        health.Data.ValueChanged += OnDamaged;
    }

    void OnDamaged(int current, int delta)
    {
        animator.SetTrigger("Hit");
    }
}
