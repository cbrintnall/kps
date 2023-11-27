using UnityEngine;

public class DestroyOnRoundEnd : MonoBehaviour
{
    RoundManager roundManager;

    void Start()
    {
        roundManager = FindObjectOfType<RoundManager>();
    }

    void Update()
    {
        if (!roundManager.Active)
        {
            Destroy(gameObject);
        }
    }
}
