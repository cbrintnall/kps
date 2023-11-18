using Cinemachine;
using UnityEngine;

public struct LookData
{
    public Vector3 EndPoint;
    public Vector3 Direction;
    public Vector3 StartPoint;
    public Transform StartTransform;
}

public class MouseLook : MonoBehaviour
{
    const float AIM_DISTANCE = 1000.0f;

    public static MouseLook Instance;

    public float Sensitivity = 1.0f;
    public CinemachineVirtualCamera Camera;
    public LookData LookData;

    private float yView = 0.0f;
    private PlayerInputManager playerInputManager;

    void Awake()
    {
        // Bad idea, but in the name of time we'll use it
        Instance = this;
        playerInputManager = SingletonLoader.Get<PlayerInputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        var lookDir = playerInputManager.LookDir;

        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y + lookDir.x * Sensitivity,
            transform.rotation.eulerAngles.z
        );

        yView = Mathf.Clamp(yView - lookDir.y * Sensitivity, -89.0f, 89.0f);

        Camera.transform.rotation = Quaternion.Euler(
            yView,
            Camera.transform.rotation.eulerAngles.y,
            Camera.transform.rotation.eulerAngles.z
        );
    }

    void FixedUpdate()
    {
        var baseCam = UnityEngine.Camera.main;
        LookData.EndPoint = baseCam.transform.forward * AIM_DISTANCE;
        LookData.StartPoint = baseCam.transform.position;
        LookData.StartTransform = baseCam.transform;
        LookData.Direction = baseCam.transform.forward.normalized;

        if (
            Physics.Raycast(
                baseCam.transform.position,
                baseCam.transform.forward,
                out RaycastHit hit,
                AIM_DISTANCE,
                ~LayerMask.GetMask("Player"),
                QueryTriggerInteraction.Collide
            )
        )
        {
            LookData.EndPoint = hit.point;
            LookData.Direction = (LookData.EndPoint - LookData.StartPoint).normalized;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Ray(LookData.StartPoint, LookData.Direction));
        Gizmos.DrawSphere(LookData.EndPoint, 0.25f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(LookData.StartPoint, LookData.EndPoint);
    }
}
