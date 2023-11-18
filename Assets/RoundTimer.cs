using TMPro;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    RoundManager roundManager;
    TextMeshProUGUI txt;

    void Awake()
    {
        roundManager = SingletonLoader.Get<RoundManager>();
        txt = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (roundManager.Active)
        {
            txt.text = roundManager.TimerTag;
        }
        else
        {
            txt.text = "Press U to start round";
        }
    }
}
