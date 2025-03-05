using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Movement Settings")]
    public float maxSpeed = 4f;
    public float blockSpeed = 1.5f; // Movement speed when blocking
    public float acceleration = 10f; // How quickly the player reaches max speed
    public float deceleration = 25f; // How quickly the player stops when no input
    public float airAcceleration = 5f;
    public float airBrake = 5f; // How quickly the player can stop mid-air
    public float groundFriction = 15f; // Stop quickly on ground

    [Header("Jump Settings")]
    public float jumpForce = 9f;
    public float maxFallSpeed = -10f;
    public float lowJumpMultiplier = 2f; // Apply extra gravity for lower jumps
    public float fallGravityMultiplier = 1f;
    public float coyoteTime = 0f; // Small "grace" period after leaving ground
    private float coyoteTimer;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform wallCheck;
    public float groundCheckRadius = 0.3f;
    public float wallCheckRadius = 0.2f;
    private bool isGrounded;
    private bool isTouchingWall;

    [Header("Health System")]
    private readonly int maxHealth = 3;
    private int currentHealth;
    private bool canTakeDamage = true;
    public Image[] healthImages;
    public Sprite[] healthSprites;

    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite runSprite;
    public Sprite jumpSprite;
    public Sprite fallSprite;
    public Sprite blockSprite;

    private float horizontalInput;
    private bool isJumping;
    private bool hasJumped = false;
    private bool jumpPressed;
    private bool jumpReleased;
    public bool isBlocking;

    public AudioClip jumpSound;
    public AudioSource sfx; //Still required with AudioClip to output audio

    //public AK.Wwise.Event Level1Music;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();

        currentHealth = maxHealth;
        // Add logic to save health and take health from the saves


        // Audio stuff
        //AkUnitySoundEngine.PostEvent("Level1Music", 447814771);
    }

    void Update()
    {
        HandleInput();
        CheckCollisionStatus();
        CheckGroundStatus();
        HandleCoyoteTime();
        HandleJumping();
        MovePlayer();
        UpdateSprite();
    }

    void FixedUpdate()
    {
        ApplyGravityModifiers();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isBlocking = Input.GetMouseButton(1);

        if (Input.GetButtonDown("Jump")) jumpPressed = true;
        if (Input.GetButtonUp("Jump")) jumpReleased = true;
    }

    private void CheckCollisionStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, groundLayer);

        if (isGrounded) hasJumped = false; // Prevent double jumping
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded) hasJumped = false; // Prevent double jumps
    }

    private void HandleCoyoteTime()
    {
        if (isGrounded) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;
    }

    private void HandleJumping()
    {
        // Jump only if player is on ground OR if going off the ground "slightly", let them still jump
        if (jumpPressed && (isGrounded || coyoteTimer > 0) && !hasJumped && !isBlocking && !isTouchingWall)
        {
            Jump();
            jumpPressed = false;
        }
    }

    private void Jump()
    {
        sfx.PlayOneShot(jumpSound);
        rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        hasJumped = true;
    }

    private void MovePlayer()
    {
        float currentMaxSpeed = isBlocking ? blockSpeed : maxSpeed;
        float targetSpeed = horizontalInput * currentMaxSpeed;
        float speedDifference = targetSpeed - rb.linearVelocityX;
        float accelerationRate = isGrounded ? acceleration : airAcceleration;
        float movement = speedDifference * accelerationRate * Time.deltaTime;

        rb.linearVelocity = new Vector2(rb.linearVelocityX + movement, rb.linearVelocityY);

        ApplyFriction(); // Ground friction while stopping - might remove
        HandleDirectionFlip();
    }

    private void ApplyFriction()
    {
        if (isGrounded && horizontalInput == 0)
        {
            float slowDown = Mathf.Min(Mathf.Abs(rb.linearVelocityX), groundFriction * Time.deltaTime);
            rb.linearVelocity = new Vector2(rb.linearVelocityX - Mathf.Sign(rb.linearVelocityX) * slowDown, rb.linearVelocityY);
        }
    }

    private void HandleDirectionFlip()
    {
        if (!isBlocking)
        {
            if (horizontalInput > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (horizontalInput < 0) transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void ApplyGravityModifiers()
    {
        // If player is jumping and releases button, apply low jump multiplier
        if (rb.linearVelocityY > 0 && jumpReleased)
        {
            rb.linearVelocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up;
            jumpReleased = false;
        }

        // Fall gravity multiplier - FIX
        else if (rb.linearVelocityY < 0) rb.linearVelocity += Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up; // (fallGravityMultiplier - 1) * 

        // Limit max fall speed
        if (rb.linearVelocityY < maxFallSpeed) rb.linearVelocity = new Vector2(rb.linearVelocityX, maxFallSpeed);
    }

    private void UpdateSprite()
    {
        if (isBlocking)
        {
            animator.enabled = false;
            spriteRenderer.sprite = blockSprite;
        }
        else if (!isGrounded)
        {
            animator.enabled = false;
            spriteRenderer.sprite = (rb.linearVelocityY > 0) ? jumpSprite : fallSprite;
        }
        else if (Mathf.Abs(rb.linearVelocityX) > 0.1f)
        {
            animator.enabled = true;
            animator.SetBool("isRunning", true);
            animator.SetBool("isIdle", false);
        }
        else
        {
            animator.enabled = true;
            animator.SetBool("isIdle", true);
            animator.SetBool("isRunning", false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If enemy collides with player and enemy is not frozen, take damage
        EnemyBehaviour enemy = collision.gameObject.GetComponent<EnemyBehaviour>();

        if (enemy != null && !enemy.isFrozen) HandleEnemyCollision(collision);

        // Reduce fall impact by damping velocity upon landing
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            rb.linearVelocity = new Vector2(rb.linearVelocityX * 0.1f, Mathf.Max(rb.linearVelocityY, -2f));
    }

    private void HandleEnemyCollision(Collision2D collision)
    {
        if (!canTakeDamage) return; // If blocking

        Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
        float knockbackForce = isBlocking ? 7f : 15f;

        rb.linearVelocity = new Vector2(knockbackDirection.x * knockbackForce, rb.linearVelocityY);

        if (!isBlocking) TakeDamage();
    }

    private void TakeDamage()
    {
        if (currentHealth > 0)
        {
            currentHealth--;
            UpdateHealthUI(-1);
            canTakeDamage = false;
            StartCoroutine(DamageCooldown());
        }

        // Respawn at last saved checkpoint
        if (currentHealth <= 0)
        {
            GameManager gameManager = FindAnyObjectByType<GameManager>();

            if (gameManager != null)
            {
                transform.position = gameManager.respawnPoint.transform.position;
                currentHealth = maxHealth;
                UpdateHealthUI(1);
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(3f);
        canTakeDamage = true;
    }

    private void UpdateHealthUI(int i)
    {
        if (currentHealth < 0 || currentHealth > healthImages.Length) return;

        if (i == -1) healthImages[currentHealth].sprite = healthSprites[1];
        else if (i == 1) for (int j = 0; j < healthImages.Length; j++) healthImages[j].sprite = healthSprites[0];
    }
}