using TMPro;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    RoundManager roundManager;
    TextMeshProUGUI txt;

    void Awake()
    {
        roundManager = FindObjectOfType<RoundManager>();
        txt = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (roundManager.Active)
        {
            txt.text = roundManager.TimerTag;
        }
    }
}
