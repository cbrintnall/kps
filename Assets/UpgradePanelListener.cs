using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UpgradePanelListener : MonoBehaviour
{
    public int CreateCount = 4;
    public GameObject PanelPrefab;
    private CanvasGroup canvasGroup;

    List<GameObject> panels = new();
    int remainingChoices = 0;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        SingletonLoader
            .Get<EventManager>()
            .Subscribe<PlayerLeveledEvent>(data =>
            {
                remainingChoices++;
            });

        SingletonLoader
            .Get<EventManager>()
            .Subscribe<UpgradeChosenEvent>(data =>
            {
                SingletonLoader.Get<UpgradesManager>().UpdateUpgradeList();
                foreach (var panel in panels)
                {
                    Destroy(panel.gameObject);
                }

                panels = new();
                remainingChoices--;
                SingletonLoader.Get<PlayerInputManager>().PopCursor();
                DOTween.To(() => Time.timeScale, scale => Time.timeScale = scale, 1.0f, 0.75f);
                canvasGroup.blocksRaycasts = false;
            });
    }

    void Update()
    {
        if (remainingChoices > 0 && panels.Count == 0)
        {
            canvasGroup.blocksRaycasts = true;
            for (int i = 0; i < CreateCount; i++)
            {
                var panel = Instantiate(PanelPrefab, transform);

                panels.Add(panel);
            }

            SingletonLoader.Get<PlayerInputManager>().PushCursor(CursorLockMode.Confined);

            DOTween.To(() => Time.timeScale, scale => Time.timeScale = scale, 0.1f, 0.25f);
        }
    }
}
