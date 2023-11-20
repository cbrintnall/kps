using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

public delegate Bullet ShootCallback(Vector3 variance);

[JsonObject(MemberSerialization.OptIn)]
public class ShootPattern : MonoBehaviour
{
    [JsonProperty]
    public SprayData Data;

    Coroutine activeBurst;
    int PerShot => Data.BulletPattern.Length;

    [Button("Copy Output")]
    void SyncOutput()
    {
        var output = JsonConvert.SerializeObject(
            this,
            Formatting.Indented,
            new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new Vector3Converter(),
                    new StatFloatConverter(),
                    new StatIntConverter()
                }
            }
        );

#if UNITY_EDITOR
        Clipboard.Copy(output);
#endif
    }

    public void Shoot(ShootCallback cb, Action fx)
    {
        if (activeBurst == null)
        {
            activeBurst = StartCoroutine(DoShot(cb, fx));
        }
    }

    IEnumerator DoShot(ShootCallback cb, Action fx)
    {
        yield return new WaitForSeconds(Data.Delay);
        var delay = new WaitForSeconds(Data.BurstCooldown);
        var variance = new Vector3(
            PerShot > 1
                ? Mathf.Max(0.01f, Data.PerBulletDirectionVariance[0])
                : Data.PerBulletDirectionVariance[0],
            PerShot > 1
                ? Mathf.Max(0.01f, Data.PerBulletDirectionVariance[1])
                : Data.PerBulletDirectionVariance[1],
            PerShot > 1
                ? Mathf.Max(0.01f, Data.PerBulletDirectionVariance[2])
                : Data.PerBulletDirectionVariance[2]
        );

        for (int i = 0; i < Data.PerBurst; i++)
        {
            fx.Invoke();
            for (int j = 0; j < Data.BulletPattern.Length; j++)
            {
                var vec =
                    Data.BulletPattern[j]
                    + new Vector3(
                        UnityEngine.Random.Range(-variance.x, variance.x),
                        UnityEngine.Random.Range(-variance.y, variance.y),
                        UnityEngine.Random.Range(-variance.z, variance.z)
                    );

                cb.Invoke(vec);
            }

            yield return delay;
        }
        activeBurst = null;
    }
}
