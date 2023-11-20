using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI Souls;
    public TextMeshProUGUI KPS;

    private EnemyMaster master;

    void Start()
    {
        master = FindObjectOfType<EnemyMaster>();
    }

    void Update()
    {
        Souls.text = $"Souls: {PlayerEquipmentController.Instance.Money.Current}";
        // KPS.text = $"KPS: {master.killsPerSecond}/s";
    }
}
