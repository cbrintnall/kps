using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public static Crosshair Instance;

    public TextMeshProUGUI Hint;
    public Sprite DefaultCrosshair;
    public Sprite OverrideCrosshair;

    [SerializeField]
    private Image _crosshair;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        _crosshair.sprite = OverrideCrosshair ?? DefaultCrosshair;
    }
}
