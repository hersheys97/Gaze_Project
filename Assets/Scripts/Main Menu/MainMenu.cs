using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject playButton;
    public GameObject chaptersMenu;
    public GameObject settingsMenu;
    private string path;

    private void Awake()
    {
        path = Application.persistentDataPath + "/playerData.json";
        Debug.Log(Application.persistentDataPath);
    }

    void Start()
    {
        // Ensure only the Main Menu is visible at the start. Disable the Chapters and Settings UI on Canvas.
        mainMenu.SetActive(true);
        chaptersMenu.SetActive(false);
        settingsMenu.SetActive(false);
        Debug.Log("Your saves are stored in: " + path);
    }

    // Check for saves every frame - Ex: If progress is reset from Chapters Menu, display 'Start' button.
    private void FixedUpdate()
    {
        CheckSaves();
    }

    void CheckSaves()
    {
        // Persistent data path is the local path where the playerData.json file is stored
        TextMeshProUGUI buttonText = playButton.GetComponentInChildren<TextMeshProUGUI>();

        // If the saved data exists, display 'Continue' button. Otherwise, display 'Play' button.
        if (File.Exists(path)) buttonText.text = "Continue";
        else buttonText.text = "Play";

    }

    // Loads the first level, which is named as "FirstLevel" in Scenes folder
    public void PlayGame()
    {
        SceneManager.LoadScene("FirstLevel");
    }

    // Hide the Main Menu and display the Chapters Menu
    public void OpenChapters()
    {
        mainMenu.SetActive(false);
        chaptersMenu.SetActive(true);
    }

    // Hide the Main Menu and display the Settings Menu
    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    // Exit the game - Checks if the game is running in the Unity Editor or a standalone build and runs the appropriate method.
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}