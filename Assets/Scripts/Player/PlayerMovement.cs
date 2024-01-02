using Cinemachine;
using UnityEngine;

public enum PlayerMoveState
{
    GROUNDED,
    IN_AIR,
    SLIDING
}

// Taken and modified from: https://github.com/IsaiahKelly/quake3-movement-for-unity/tree/master
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    const float DUTCH_AMT = 5.0f;
    const float DUTCH_SPEED = 3.5f;
    const float FALL_SCALAR = 100.0f;

    public static PlayerMovement Instance;

    [Header("Components")]
    public Animator Animator;
    public CinemachineVirtualCamera Camera;

    [Header("Stats")]
    public float MoveSpeed = 1.0f;

    [Header("Audio")]
    public AudioClip WalkSound;
    public AudioClip FallSound;
    public AudioClip LandSound;

    private AudioManager audioManager;

    [System.Serializable]
    public class MovementSettings
    {
        public float MaxSpeed;
        public float Acceleration;
        public float Deceleration;
        public float Gravity;

        public MovementSettings(float maxSpeed, float accel, float decel)
        {
            MaxSpeed = maxSpeed;
            Acceleration = accel;
            Deceleration = decel;
        }
    }

    [Header("Movement")]
    [SerializeField]
    private float m_Friction = 6;

    [SerializeField]
    private float m_JumpForce = 8;

    [Tooltip("Automatically jump when holding jump button")]
    [SerializeField]
    private bool m_AutoBunnyHop = false;

    [Tooltip("How precise air control is")]
    [SerializeField]
    private float m_AirControl = 0.3f;

    [SerializeField]
    private MovementSettings m_GroundSettings = new MovementSettings(7, 14, 10);

    [SerializeField]
    private MovementSettings m_AirSettings = new MovementSettings(7, 2, 2);

    [SerializeField]
    private MovementSettings m_SlidingSettings = new MovementSettings(7, 2, 2);

    [SerializeField]
    private float m_walkCadence = 0.25f;

    [SerializeField]
    private float m_wallTouchGravityMultiplier = 0.3f;

    [SerializeField]
    float m_wallJumpMultiplier = 1.5f;

    /// <summary>
    /// Returns player's current speed.
    /// </summary>
    public float Speed
    {
        get { return m_Character.velocity.magnitude; }
    }

    public Vector3 Velocity
    {
        get { return m_Character.velocity; }
    }

    private CharacterController m_Character;
    public Vector3 m_PlayerVelocity = Vector3.zero;

    // Used to queue the next jump just before hitting the ground.
    private bool m_JumpQueued = false;

    private Vector3 m_MoveInput;
    private Transform m_Tran;
    private float m_TimeOffGround = 0.0f;
    private FovController fovController;
    private PlayerInputManager playerInputManager;

    [SerializeField]
    private WallJumpDetector wallData;
    private TimeSince m_walkTs;
    private Vector3 m_lastUsedNormal;
    private Vector3 m_wallNormal => wallData.Data.Normal;
    private bool m_isTouchingWall => wallData.Data.Hitting;

    public void DoJump(float force)
    {
        m_PlayerVelocity = Vector3.up * force;
        m_Character.Move(m_PlayerVelocity * Time.deltaTime);
    }

    void Awake()
    {
        Instance = this;
        audioManager = SingletonLoader.Get<AudioManager>();
        DebugManager debugManager = SingletonLoader.Get<DebugManager>();

        debugManager.AddDrawVar(
            new DrawVar() { Name = "Dot with wall", Callback = () => CanWallJump(m_wallNormal) }
        );

        debugManager.AddDrawVar(
            new DrawVar() { Name = "Player touching wall", Callback = () => m_isTouchingWall }
        );
    }

    bool CanWallJump(Vector3 normal) =>
        Mathf.Max(
            Mathf.Abs(Vector3.Dot(Vector3.forward, normal)),
            Mathf.Abs(Vector3.Dot(Vector3.right, normal))
        ) >= 0.9f;

    private void Start()
    {
        m_Tran = transform;
        m_Character = GetComponent<CharacterController>();
        fovController = Camera.GetComponent<FovController>();
        playerInputManager = SingletonLoader.Get<PlayerInputManager>();
    }

    private PlayerMoveState DetermineMoveState()
    {
        if (playerInputManager.Sliding)
        {
            return PlayerMoveState.SLIDING;
        }

        return m_Character.isGrounded ? PlayerMoveState.GROUNDED : PlayerMoveState.IN_AIR;
    }

    private void Update()
    {
        m_MoveInput = new Vector3(playerInputManager.MoveDir.x, 0, playerInputManager.MoveDir.y);

        QueueJump();

        // Set movement state.
        switch (DetermineMoveState())
        {
            case PlayerMoveState.GROUNDED:
                DoGroundState();
                break;
            case PlayerMoveState.IN_AIR:
                DoAirState();
                break;
            case PlayerMoveState.SLIDING:
                DoSlideState();
                break;
        }

        ApplyJump();

        // Move the character.
        m_Character.Move(m_PlayerVelocity * Time.deltaTime);

        Camera.m_Lens.Dutch = Mathf.Lerp(
            Camera.m_Lens.Dutch,
            -m_MoveInput.x * DUTCH_AMT,
            Time.deltaTime * DUTCH_SPEED
        );
    }

    private void DoSlideState()
    {
        Animator.SetBool("Moving", false);
        ApplyFriction(0, m_SlidingSettings);

        var wishdir = new Vector3(
            UnityEngine.Camera.main.transform.forward.x,
            0.0f,
            UnityEngine.Camera.main.transform.forward.z
        );
        wishdir.Normalize();

        var wishspeed = wishdir.magnitude;
        wishspeed *= m_SlidingSettings.MaxSpeed;

        Accelerate(wishdir, wishspeed, m_SlidingSettings.Acceleration);

        float grav = Mathf.Max(
            m_SlidingSettings.Gravity,
            Mathf.Min(m_TimeOffGround * FALL_SCALAR, m_JumpForce * m_SlidingSettings.Gravity)
        );

        // Reset the gravity velocity
        m_PlayerVelocity.y = -grav * Time.deltaTime;
    }

    private void DoGroundState()
    {
        GroundMove();
        m_TimeOffGround = 0.0f;
        if (m_walkTs > m_walkCadence)
        {
            audioManager.Play(
                new AudioPayload()
                {
                    Clip = WalkSound,
                    Is2D = true,
                    Volume = 0.2f
                }
            );
            m_walkTs = 0.0f;
        }
        Animator.SetBool("Moving", m_MoveInput.sqrMagnitude > 0);

        fovController.AdditionalFov =
            Mathf.InverseLerp(0.0f, m_GroundSettings.MaxSpeed, Speed * Time.deltaTime) * 10.0f;
    }

    private void DoAirState()
    {
        AirMove();
        m_lastUsedNormal = Vector3.zero;
        m_TimeOffGround += Time.deltaTime;
        Animator.SetBool("Moving", false);
        m_walkTs = 0.0f;
    }

    // Queues the next jump.
    private void QueueJump()
    {
        if (m_AutoBunnyHop)
        {
            m_JumpQueued = playerInputManager.Jumping;
            return;
        }

        if (Input.GetButtonDown("Jump") && !m_JumpQueued)
        {
            m_JumpQueued = true;
        }

        if (Input.GetButtonUp("Jump"))
        {
            m_JumpQueued = false;
        }
    }

    // Handle air movement.
    private void AirMove()
    {
        float accel;

        var wishdir = new Vector3(m_MoveInput.x, 0, m_MoveInput.z);
        wishdir = m_Tran.TransformDirection(wishdir);

        float wishspeed = wishdir.magnitude;
        wishspeed *= m_AirSettings.MaxSpeed;

        wishdir.Normalize();
        // CPM Air control.
        float wishspeed2 = wishspeed;
        //test
        if (Vector3.Dot(m_PlayerVelocity, wishdir) < 0)
        {
            accel = m_AirSettings.Deceleration;
        }
        else
        {
            accel = m_AirSettings.Acceleration;
        }

        Accelerate(wishdir, wishspeed, accel);
        if (m_AirControl > 0)
        {
            AirControl(wishdir, wishspeed2);
        }

        float grav = Mathf.Max(
            m_AirSettings.Gravity,
            Mathf.Min(m_TimeOffGround * FALL_SCALAR, m_JumpForce * 5.0f)
        );

        if (m_isTouchingWall && !m_Character.isGrounded)
        {
            grav *= m_wallTouchGravityMultiplier;
        }

        if (Physics.Raycast(Camera.transform.position, Vector3.up, 1.0f))
        {
            grav *= 2.0f;
        }

        // Apply gravity
        m_PlayerVelocity.y -= grav * Time.deltaTime;
    }

    // Air control occurs when the player is in the air, it allows players to move side
    // to side much faster rather than being 'sluggish' when it comes to cornering.
    private void AirControl(Vector3 targetDir, float targetSpeed)
    {
        // Only control air movement when moving forward or backward.
        if (Mathf.Abs(m_MoveInput.z) < 0.001 || Mathf.Abs(targetSpeed) < 0.001)
        {
            return;
        }

        float zSpeed = m_PlayerVelocity.y;
        m_PlayerVelocity.y = 0;
        /* Next two lines are equivalent to idTech's VectorNormalize() */
        float speed = m_PlayerVelocity.magnitude;
        m_PlayerVelocity.Normalize();

        float dot = Vector3.Dot(m_PlayerVelocity, targetDir);
        float k = 32;
        k *= m_AirControl * dot * dot * Time.deltaTime;

        // Change direction while slowing down.
        if (dot > 0)
        {
            m_PlayerVelocity.x *= speed + targetDir.x * k;
            m_PlayerVelocity.y *= speed + targetDir.y * k;
            m_PlayerVelocity.z *= speed + targetDir.z * k;

            m_PlayerVelocity.Normalize();
        }

        m_PlayerVelocity.x *= speed;
        m_PlayerVelocity.y = zSpeed; // Note this line
        m_PlayerVelocity.z *= speed;
    }

    // Handle ground movement.
    private void GroundMove()
    {
        // Do not apply friction if the player is queueing up the next jump
        if (!m_JumpQueued)
        {
            ApplyFriction(1.0f, m_GroundSettings);
        }
        else
        {
            ApplyFriction(0, m_GroundSettings);
        }

        var wishdir = Vector3.zero;
        wishdir += playerInputManager.MoveDir.x * transform.right;
        wishdir +=
            playerInputManager.MoveDir.y
            * new Vector3(
                UnityEngine.Camera.main.transform.forward.x,
                0.0f,
                UnityEngine.Camera.main.transform.forward.z
            );
        wishdir.Normalize();

        var wishspeed = wishdir.magnitude;
        wishspeed *= m_GroundSettings.MaxSpeed;

        Accelerate(wishdir, wishspeed, m_GroundSettings.Acceleration);

        // Reset the gravity velocity
        m_PlayerVelocity.y = -m_GroundSettings.Gravity * Time.deltaTime;
    }

    void ApplyJump()
    {
        if (m_JumpQueued)
        {
            bool wallJumping = false;
            Vector3 jumpDirection = Vector3.up * m_JumpForce;
            if (m_isTouchingWall && CanWallJump(m_wallNormal) && !m_Character.isGrounded)
            {
                if (m_wallNormal != m_lastUsedNormal)
                {
                    m_lastUsedNormal = m_wallNormal;
                    jumpDirection =
                        ((Vector3.up + m_wallNormal) / 2.0f).normalized
                        * m_JumpForce
                        * m_wallJumpMultiplier;

                    m_PlayerVelocity = jumpDirection;
                }
            }
            else if (!m_Character.isGrounded)
            {
                // If not touching wall and we're in the air, no double jump fucko
                jumpDirection = Vector3.zero;
            }

            if (!wallJumping)
                m_PlayerVelocity += jumpDirection;

            m_JumpQueued = false;
        }
    }

    private void ApplyFriction(float t, MovementSettings movementSettings)
    {
        // Equivalent to VectorCopy();
        Vector3 vec = m_PlayerVelocity;
        vec.y = 0;
        float speed = vec.magnitude;
        float drop = 0;

        // Only apply friction when grounded.
        if (m_Character.isGrounded)
        {
            float control =
                speed < movementSettings.Deceleration ? movementSettings.Deceleration : speed;
            drop = control * m_Friction * Time.deltaTime * t;
        }

        float newSpeed = speed - drop;
        if (newSpeed < 0)
        {
            newSpeed = 0;
        }

        if (speed > 0)
        {
            newSpeed /= speed;
        }

        m_PlayerVelocity.x *= newSpeed;
        // playerVelocity.y *= newSpeed;
        m_PlayerVelocity.z *= newSpeed;
    }

    // Calculates acceleration based on desired speed and direction.
    private void Accelerate(Vector3 targetDir, float targetSpeed, float accel)
    {
        float currentspeed = Vector3.Dot(m_PlayerVelocity, targetDir);
        float addspeed = targetSpeed - currentspeed;
        if (addspeed <= 0)
        {
            return;
        }

        float accelspeed = accel * Time.deltaTime * targetSpeed;
        if (accelspeed > addspeed)
        {
            accelspeed = addspeed;
        }

        m_PlayerVelocity.x += accelspeed * targetDir.x;
        m_PlayerVelocity.z += accelspeed * targetDir.z;
    }
}
