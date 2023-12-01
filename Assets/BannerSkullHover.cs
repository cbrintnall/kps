using DG.Tweening;
using UnityEngine;

public class BannerSkullHover : MonoBehaviour
{
    public float TimePer = 0.75f;
    public float Amount = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        var s = DOTween.Sequence();

        s.Append(transform.DOLocalMoveY(Amount, TimePer).SetEase(Ease.InCubic));
        // s.Append(transform.DOLocalMoveY(-Amount, TimePer).SetEase(Ease.OutCubic));

        s.SetLoops(-1, LoopType.Yoyo);
    }
}
