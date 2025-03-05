using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if (collision.gameObject.CompareTag("Player"))
        {
            gameManager.player.transform.position = gameManager.respawnPoint.transform.position;
        }
    }
}
