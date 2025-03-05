using UnityEngine;

public class DestroyBullet : MonoBehaviour
{
    private Vector2 spawnPosition;
    private float maxDistance = 10f;  // Max distance the bullet can travel before being destroyed

    void Start()
    {
        // Set the spawn position when the bullet is instantiated
        spawnPosition = transform.position;
    }

    void Update()
    {
        // Check if the bullet has traveled more than the maximum distance
        if (Vector2.Distance(transform.position, spawnPosition) >= maxDistance)
        {
            Destroy(gameObject);  // Destroy the bullet
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with something other than the player
        if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);  // Destroy the bullet on collision with anything other than the player
        }
    }
}