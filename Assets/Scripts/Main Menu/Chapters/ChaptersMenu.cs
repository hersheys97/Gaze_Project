using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChaptersMenu : MonoBehaviour
{
    public PlayerDataManager playerDataManager;
    public PlayerData playerData;
    public GameObject mainMenu;
    public GameObject chaptersMenu;
    public GameObject goButton;
    private Button goButtonComponent;
    private List<CheckpointData> unlockedCheckpoints = new();
    private Button changeCheckpoint;
    private Button selectedCheckpoint; // Persist the selected button

    void Awake()
    {
        goButtonComponent = goButton.GetComponent<Button>();
        goButtonComponent.interactable = false;
    }

    void Update()
    {
        LoadUnlockedCheckpoints();
        InitializeChapters();
        if (selectedCheckpoint != changeCheckpoint)
        {
            selectedCheckpoint = changeCheckpoint;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(selectedCheckpoint.gameObject);
        }
    }

    private void LoadUnlockedCheckpoints()
    {
        string path = Application.persistentDataPath + "/playerData.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            if (data != null && data.unlockedCheckpoints != null) unlockedCheckpoints = new List<CheckpointData>(data.unlockedCheckpoints);
            else
            {
                unlockedCheckpoints = new List<CheckpointData>();
                Debug.LogWarning("Loaded player data, but no checkpoints found.");
            }
        }
        else
        {
            unlockedCheckpoints = new List<CheckpointData>();
            Debug.LogWarning("No save file found. No checkpoints unlocked.");
        }
    }

    private void InitializeChapters()
    {
        GameObject[] chapters = GameObject.FindGameObjectsWithTag("Chapters");

        foreach (GameObject chapter in chapters)
        {
            bool isUnlocked = unlockedCheckpoints.Exists(c => c.name == chapter.name);

            if (isUnlocked)
            {
                if (chapter.GetComponent<BoxCollider2D>() == null)
                {
                    BoxCollider2D collider = chapter.AddComponent<BoxCollider2D>();
                    collider.isTrigger = true;
                }
            }

            ChapterClickHandler clickHandler = chapter.AddComponent<ChapterClickHandler>();
            clickHandler.Initialize(chapter, isUnlocked);
        }
    }

    public void OnChapterClicked()
    {
        Button newSelection = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();

        if (newSelection != null && newSelection != changeCheckpoint)
        {
            changeCheckpoint = newSelection;
            goButtonComponent.interactable = true;
        }
    }

    public void OnGoClicked()
    {
        string path = Application.persistentDataPath + "/playerData.json";

        // Step 1: Load existing data if the file exists
        PlayerData existingData;
        if (File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            existingData = JsonUtility.FromJson<PlayerData>(json);
        }
        else existingData = new PlayerData(); // Create a new instance if no file exists

        // Step 2: Update only playerPosition
        CheckpointData checkpoint = unlockedCheckpoints.Find(c => c.name == selectedCheckpoint.name);
        if (checkpoint != null) existingData.playerPosition = checkpoint.position;

        // Step 3: Save back the updated data
        string updatedJson = JsonUtility.ToJson(existingData);
        System.IO.File.WriteAllText(path, updatedJson);

        // Step 4: TO-DO Add a level variable in playerData as well. As of now, we are just loading the first scene to keep it simple.
        SceneManager.LoadScene("FirstLevel");
    }

    public void BackToMainMenu()
    {
        mainMenu.SetActive(true);
        chaptersMenu.SetActive(false);
    }

    public void ResetProgress()
    {
        string path = Application.persistentDataPath + "/playerData.json";

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Game save reset successfully.");
        }
        else
        {
            Debug.LogWarning("No save file found to delete.");
        }
    }
}