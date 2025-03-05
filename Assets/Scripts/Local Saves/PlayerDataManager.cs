using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PlayerDataManager : MonoBehaviour
{
    public GameObject player;

    public void SaveGame()
    {
        PlayerData playerData = new PlayerData();
        GameManager gameManager = GetComponent<GameManager>();
        playerData.playerPosition = gameManager.respawnPoint.transform.position;
        playerData.playerHealth = gameManager.playerHealth;
        playerData.unlockedCheckpoints = gameManager.unlockedCheckpoints;

        Debug.Log("Saving at " + playerData.playerPosition + ". Checkpoint " + gameManager.respawnPoint.name);

        string json = JsonUtility.ToJson(playerData);
        string path = Application.persistentDataPath + "/playerData.json";
        System.IO.File.WriteAllText(path, json);
    }

    public void LoadGame()
    {
        GameManager gameManager = GetComponent<GameManager>();
        string path = Application.persistentDataPath + "/playerData.json";

        if (File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            // Update the player's data
            player.transform.position = playerData.playerPosition;
            gameManager.playerHealth = playerData.playerHealth;
            gameManager.unlockedCheckpoints = playerData.unlockedCheckpoints;

            Debug.Log("Loading at " + playerData.playerPosition);
        }
        else
        {
            Debug.LogWarning("There are no saves. A new save will be created.");
        }
    }
}