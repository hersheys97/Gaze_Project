using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameManager gameManager;
    private BoxCollider2D boxCollider;
    private PlayerDataManager playerDataManager;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerDataManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataManager>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameManager.respawnPoint = this.gameObject;
            gameManager.transform.position = this.transform.position;

            // Unlock this checkpoint in PlayerDataManager
            if (playerDataManager != null)
            {
                gameManager.UnlockCheckpoint(this.gameObject); // Unlock using the checkpoint's name
                playerDataManager.SaveGame();
            }

            boxCollider.enabled = false;
            Debug.Log("Changed respawn point to " + gameManager.transform.position);
        }
    }
}