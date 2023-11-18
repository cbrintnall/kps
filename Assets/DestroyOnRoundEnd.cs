using UnityEngine;

public class DestroyOnRoundEnd : MonoBehaviour
{
    RoundManager roundManager;

    void Start()
    {
        roundManager = SingletonLoader.Get<RoundManager>();
    }

    void Update()
    {
        if (!roundManager.Active)
        {
            Destroy(gameObject);
        }
    }
}
