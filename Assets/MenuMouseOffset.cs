using UnityEngine;

public class MenuMouseOffset : MonoBehaviour
{
    public float Offset = 4.0f;
    public float OffsetSpeed = 1.0f;
    public RectTransform OptionsRoot;

    private Vector3 basePosition;

    // Start is called before the first frame update
    void Start()
    {
        basePosition = OptionsRoot.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = Vector3.zero;

        if (Vector3.Distance(Input.mousePosition, OptionsRoot.transform.position) > 2.0f)
        {
            var dir = (Input.mousePosition - OptionsRoot.transform.position).normalized;
            offset = dir * Offset;
        }

        OptionsRoot.transform.position = Vector3.Lerp(
            OptionsRoot.transform.position,
            basePosition + offset,
            Time.deltaTime * OffsetSpeed
        );
    }
}
