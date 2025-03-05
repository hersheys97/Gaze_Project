using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Camera Movement Settings")]
    public float smoothSpeed = 1f;
    public float damping = 0.05f;
    public float stopThreshold = 0.05f; // Min velocity before stopping camera movement

    [Header("Look-Ahead Settings")]
    public float lookAheadDistance = 1.5f;
    public float defaultOffset = 0.8f;

    [Header("Vertical Offset Settings")]
    public float verticalOffset = 0.5f;
    public float jumpLookUp = 0f;
    public float fallLookDown = 2f;
    public float fallSpeedMultiplier = 3f;

    private Vector3 velocity = Vector3.zero;
    private float targetLookAhead = 0f;
    private float currentLookAhead = 0f;
    private float targetVerticalOffset;
    private float currentVerticalOffset;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private PlayerMovement playerScript;
    private float lastMoveDirection = 1f; // Store last movement direction for smooth transition

    private AudioSource music;

    void Start()
    {
        // Get player's components
        spriteRenderer = player.GetComponent<SpriteRenderer>();
        rb = player.GetComponent<Rigidbody2D>();
        playerScript = player.GetComponent<PlayerMovement>();
        music = GetComponent<AudioSource>();

        music.Play();
        Debug.Log("Music: " + music);

        // Set default offsets when starting the game
        targetVerticalOffset = verticalOffset;
        currentVerticalOffset = verticalOffset;
    }

    void LateUpdate()
    {
        UpdateLookAhead();
        UpdateVerticalOffset();
        SmoothCameraMovement();
    }

    void UpdateLookAhead()
    {
        // If player is moving, then get whether moving left (nega-) or right (pos+)
        float moveDirection = Mathf.Abs(rb.linearVelocityX) > stopThreshold ? Mathf.Sign(rb.linearVelocityX) : 0;

        // If player is moving, then update last movement to moving left or right
        if (moveDirection != 0) lastMoveDirection = moveDirection;

        // Adjust look-ahead and offset based on player's movement direction if the player is moving
        targetLookAhead = (moveDirection == 0) ? lastMoveDirection * defaultOffset : moveDirection * lookAheadDistance;

        // If player stops moving, return to default offset
        if (moveDirection == 0) targetLookAhead = lastMoveDirection * defaultOffset;
        else
        {
            targetLookAhead = 0f; // Look straight ahead when idle
            targetVerticalOffset = 0f; // Center player vertically
        }

        // Smoothly interpolate the look-ahead - Prevents abrupt camera movement
        currentLookAhead = Mathf.Lerp(currentLookAhead, targetLookAhead, Time.deltaTime * 3f);
    }

    void UpdateVerticalOffset()
    {
        // Adjust vertical offset based on if player is jumping or falling
        if (spriteRenderer.sprite == playerScript.jumpSprite) targetVerticalOffset = verticalOffset + jumpLookUp; // Jumping
        else if (spriteRenderer.sprite == playerScript.fallSprite) targetVerticalOffset = verticalOffset - fallLookDown; // Falling
        else targetVerticalOffset = verticalOffset; // Default position when on the ground

        // Smoothly interpolate vertical movement - Move faster when falling down. Else, use the default speed smoother.
        float verticalSpeed = (spriteRenderer.sprite == playerScript.fallSprite) ? fallSpeedMultiplier : smoothSpeed;
        currentVerticalOffset = Mathf.Lerp(currentVerticalOffset, targetVerticalOffset, Time.deltaTime * verticalSpeed);
    }

    void SmoothCameraMovement()
    {
        // Target position with smooth look-ahead and vertical movement
        Vector3 targetPosition = new(
            player.position.x + currentLookAhead,
            player.position.y + currentVerticalOffset,
            transform.position.z
        );

        // SmoothDamp - Current pos, Target pos, Current velocity, Smooth time
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, damping); // Damping smoothens it
    }
}