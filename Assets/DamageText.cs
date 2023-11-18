using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Health))]
public class DamageText : MonoBehaviour
{
    public TextMeshPro Text;

    void Awake()
    {
        GetComponent<Health>().OnDamaged += (payload) =>
        {
            if (payload.Amount > 0)
            {
                var txt = Instantiate(Text, transform.position, Quaternion.identity);
                var scalar = 1.0f;
                txt.text = Mathf.Abs(payload.Amount).ToString();
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
