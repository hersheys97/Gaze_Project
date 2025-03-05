using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerDataManager playerDataManager;
    public GameObject player;
    public GameObject respawnPoint;
    public GameObject pauseMenuCanvas;
    public GameObject mainUI;
    public GameObject settingsUI;
    public int playerHealth;
    public List<CheckpointData> unlockedCheckpoints = new();
    private bool isPaused = false;

    void Start()
    {
        playerDataManager = GetComponent<PlayerDataManager>();
        playerDataManager.LoadGame();

        pauseMenuCanvas.SetActive(false); // Ensure the pause menu is hidden when the game starts

        Time.timeScale = 1f; // Ensure the game runs at normal speed

        StartCoroutine(InitiateAutoSave());
    }

    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuCanvas.SetActive(true);
        mainUI.SetActive(true);
        settingsUI.SetActive(false);
        Time.timeScale = 0f; // Completely pauses the game
    }

    public void LoadSettings()
    {
        mainUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    public void GoBack()
    {
        settingsUI.SetActive(false);
        mainUI.SetActive(true);
    }

    public void ApplySettings()
    {
        // Save locally
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f; // Resumes the game
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale before loading the main menu
        SceneManager.LoadScene("MainMenu");
    }

    // Save every 10 seconds
    IEnumerator InitiateAutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            playerDataManager.SaveGame();
        }
    }

    // If the player reaches a checkpoint which is not added in Save data, then add the name and position of that checkpoint.
    public void UnlockCheckpoint(GameObject checkpoint)
    {
        if (!unlockedCheckpoints.Exists(c => c.name == checkpoint.name))
        {
            unlockedCheckpoints.Add(new CheckpointData(checkpoint.name, checkpoint.transform.position));
        }
    }
}