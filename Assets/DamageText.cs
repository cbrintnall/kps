using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Health))]
public class DamageText : MonoBehaviour
{
    const int PANIC = 150;
    public TextMeshPro Text;

    void Awake()
    {
        GetComponent<Health>().OnDamaged += (payload) =>
        {
            float xtreme = (float)payload.Dealt / PANIC;
            Text.color = Color.Lerp(Color.white, Color.red, Mathf.Min(xtreme, 1.0f));
            if (payload.Amount > 0)
            {
                var txt = Instantiate(Text, transform.position, Quaternion.identity);
                var scalar = Mathf.Max(5.0f * xtreme, 1.0f);
                txt.text = Mathf.Abs(payload.Amount).ToString();
                txt.transform.localScale = Vector3.one * (payload.Crit ? 3.0f : 1.0f);
                txt.transform.DOPunchScale(txt.transform.localScale * 1.1f, 0.5f);
                txt.transform
                    .DOMove(
                        transform.position
                            + new Vector3(
                                Random.Range(-scalar, scalar),
                                Random.Range(1.0f, scalar),
                                Random.Range(-scalar, scalar)
                            ),
                        1.0f
                    )
                    .OnComplete(() =>
                    {
                        Destroy(txt.gameObject);
                    });
            }
        };
    }
}
