using System.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Enemy Movement")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 4f;
    public float triggerRange = 3f;
    public float chaseRange = 7f;

    [Header("Patrol & Checkpoints")]
    public Transform patrolCheck;
    public Transform checkpoint;
    public float checkRadius = 0.2f;
    public LayerMask patrolLayer;

    [Header("Sprites & Animation")]
    public Sprite normalSprite;
    public Sprite frozenSprite;

    public bool isFrozen = false;

    private enum State { Patrol, Chase, Wait, Frozen, Returning };
    private State aiState = State.Patrol;

    private bool facingRight = false;
    private bool canFlip = true;

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private GameObject player;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        if (isFrozen) return;

        switch (aiState)
        {
            case State.Patrol:
                Patrol();
                SetAnimationState(true, false, false, false);
                break;
            case State.Chase:
                ChasePlayer();
                SetAnimationState(false, true, false, false);
                break;
            case State.Wait:
                WaitState();
                SetAnimationState(false, false, true, false);
                break;
            case State.Frozen:
                SetFrozenState();
                SetAnimationState(false, false, false, true);
                break;
            case State.Returning:
                MoveToCheckpoint();
                SetAnimationState(false, false, false, false);
                break;
        }
    }

    void Patrol()
    {
        // Move the enemy in the current facing direction
        rb.linearVelocity = new Vector2(facingRight ? patrolSpeed : -patrolSpeed, rb.linearVelocity.y);

        // Check if the enemy is on the patrol path. Else, flip.
        bool onPath = Physics2D.OverlapCircle(patrolCheck.position, checkRadius, patrolLayer);
        if (!onPath && canFlip) Flip();

        // Check if the player is within trigger range and visible. If yes, chase them to death!!!
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer < triggerRange && IsPlayerVisible()) aiState = State.Chase;
    }

    void ChasePlayer()
    {
        // Get direction vector (ignore speed, normalized) from enemy to the player. Flip if enemy is not facing the player. Not relevant currently since enemy does not check behind them.
        Vector2 direction = (player.transform.position - transform.position).normalized;
        if ((direction.x > 0 && !facingRight) || (direction.x < 0 && facingRight)) Flip();

        // Start chasing towards the player
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocityY);

        // If the player is too far, return to patrol. Else, go to wait for a few seconds before returning back to checkpoint.
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > chaseRange || !IsPlayerVisible()) aiState = State.Wait;
    }

    bool IsPlayerVisible()
    {
        // Check if player is in the direction the enemy is facing (left or right only). Set y to 0 to avoid Raycast from detecting player if above or below ground level compared to the enemy
        Vector2 directionToPlayer = (player.transform.position - transform.position);
        directionToPlayer.y = 0;

        if (facingRight && directionToPlayer.x > 0 || !facingRight && directionToPlayer.x < 0)
        {
            // Ignore the enemy's own collider. The following code inverts the mask to ignore the "Enemy" layer. This is to prevent detecting self mesh, which would otherwise always give raycast hit output as the Enemy itself.
            int enemyCollider = ~(1 << LayerMask.NameToLayer("Enemies"));
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToPlayer, triggerRange, enemyCollider);

            // Furthermore, if the enemy is returning to return checkpoint, then raycast will detect the checkpoint collider. Thus, we are using an array of raycast hits. Then, continue checking for player even if it detects the return checkpoint.
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.CompareTag("ReturnPoint")) continue;
                if (hit.collider.gameObject == player) return true;
            }
        }
        return false; // Return false by default.
    }

    public void WaitState()
    {
        // Freeze and return to checkpoint after 3 seconds, unless frozen
        rb.linearVelocity = Vector2.zero;
        if (!isFrozen) Invoke(nameof(ReturnToCheckpoint), 3.0f);
    }

    void ReturnToCheckpoint()
    {
        aiState = State.Returning;
    }

    void MoveToCheckpoint()
    {
        if (isFrozen)
        {
            aiState = State.Frozen;
            return;
        }

        // Check if the player is visible while returning. If yes, then immediately start chasing instead of continuing to the checkpoint,
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer < triggerRange && IsPlayerVisible())
        {
            aiState = State.Chase;
            return;
        }

        // Return to checkpoint. If not facing in checkpoint's direction, then flip.
        Vector2 direction = (checkpoint.position - transform.position).normalized;
        if ((direction.x > 0 && !facingRight) || (direction.x < 0 && facingRight)) Flip();
        rb.linearVelocity = new Vector2(direction.x * patrolSpeed, rb.linearVelocity.y);

        // Once returned to checkpoint, go back to patrol
        if (Vector2.Distance(transform.position, checkpoint.position) < 1f) aiState = State.Patrol;
    }


    /* --------- Helper Functions kinda ---------- */

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet")) aiState = State.Frozen;
    }

    void SetFrozenState()
    {
        if (!isFrozen)
        {
            spriteRenderer.sprite = frozenSprite;
            isFrozen = true;

            // Stop movement immediately when frozen
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // Give the frozen enemy some rocky-like properties and avoid sliding
            rb.mass = 100f;
            rb.linearDamping = 70f; // Avoid sliding
            rb.angularDamping = 500f; // Avoid rotations
            rb.gravityScale = 2f; // Increase gravity

            // Change the layer of enemy to groud, so that the player can walk on top of the enemy
            gameObject.layer = LayerMask.NameToLayer("Ground");

            // Unfreeze the enemy after 7 seconds
            StartCoroutine(UnfreezeAfterDelay());
        }
    }

    IEnumerator UnfreezeAfterDelay()
    {
        yield return new WaitForSeconds(7f);

        spriteRenderer.sprite = normalSprite;
        isFrozen = false;
        gameObject.layer = LayerMask.NameToLayer("Enemies");
        aiState = State.Returning;
    }

    void Flip()
    {
        // Flip the direction
        facingRight = !facingRight;
        transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);

        // Reverse movement direction
        rb.linearVelocity = new Vector2(facingRight ? patrolSpeed : -patrolSpeed, rb.linearVelocityY);

        // Prevent multiple flips in the same frame by re-enabling flipping after a short delay
        canFlip = false;
        Invoke(nameof(ResetFlip), 0.5f);
    }

    void ResetFlip()
    {
        canFlip = true;
    }

    void SetAnimationState(bool isPatrolling, bool isChasing, bool isWaiting, bool isFrozen)
    {
        anim.SetBool("isPatrolling", isPatrolling);
        anim.SetBool("isChasing", isChasing);
        anim.SetBool("isWaiting", isWaiting);
        anim.SetBool("isFrozen", isFrozen);
    }
}