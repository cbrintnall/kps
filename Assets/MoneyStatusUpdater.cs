using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MoneyChangedEvent : BaseEvent
{
    public int Gained;
}

public class MoneyStatusUpdater : MonoBehaviour
{
    public TextMeshProUGUI TMP;
    public float ResetTime = 3.5f;

    int textCount;
    int aggregate;
    TimeSince ts;

    void Awake()
    {
        TMP.color = Color.clear;
        ts = 0;
        SingletonLoader.Get<EventManager>().Subscribe<MoneyChangedEvent>(OnMoney);
    }

    void OnMoney(MoneyChangedEvent ev)
    {
        if (ev.Gained < 0)
            return;

        aggregate += ev.Gained;
        ts = 0;

        if (!TMP.gameObject.activeInHierarchy)
        {
            TMP.color = Color.clear;
            TMP.gameObject.SetActive(true);
            TMP.DOColor(Color.white, 0.25f);
        }
    }

    void Update()
    {
        textCount = Mathf.RoundToInt(Mathf.Lerp(textCount, aggregate, Time.deltaTime * 3.0f));
        TMP.text = $"+{textCount}";

        if (ts > ResetTime)
        {
            aggregate = 0;
            ts = 0;

            if (TMP.gameObject.activeInHierarchy)
            {
                TMP.DOColor(Color.clear, 0.25f).OnComplete(() => TMP.gameObject.SetActive(false));
            }
        }
    }
}
