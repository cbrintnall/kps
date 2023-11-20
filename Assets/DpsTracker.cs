using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DpsTracker : MonoBehaviour
{
    const int BUFFER_SIZE = 10;

    public TextMeshPro Text;
    public double DPS => buffer.Average();

    int[] buffer = new int[BUFFER_SIZE];
    int zone = 0;
    TimeSince ts;

    void Awake()
    {
        ts = 0;
        GetComponent<Health>().OnDamaged += (payload) =>
        {
            buffer[zone] += payload.Amount;
        };
    }

    void Update()
    {
        if (ts > 1)
        {
            ts = 0;
            buffer[zone] = buffer[zone] > 0 ? buffer[zone] : 0;
            zone = (zone + 1) % BUFFER_SIZE;
        }

        if (Text)
        {
            Text.text = DPS.ToString();
        }
    }
}
