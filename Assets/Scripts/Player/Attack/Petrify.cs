using UnityEngine;

public class Petrify : MonoBehaviour
{
    public GameObject BulletPrefab;
    public float BulletForce = 20f;
    private Collider2D playerCollider;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !playerMovement.isBlocking)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Convert mouse position from screen space to world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

        // Calculate direction from player to mouse position
        Vector2 direction = ((Vector2)mousePosition - (Vector2)transform.position).normalized;

        Vector2 spawnPosition = (Vector2)transform.position + direction;
        GameObject bullet = Instantiate(BulletPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();

        // Ignore collision between player and bullet
        Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
        if (playerCollider != null && bulletCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, bulletCollider);
        }

        // Apply force in the correct direction
        bulletRB.linearVelocity = direction * BulletForce;

        // Rotate bullet to face the firing direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
