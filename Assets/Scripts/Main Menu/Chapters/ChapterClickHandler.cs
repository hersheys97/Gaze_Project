using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterClickHandler : MonoBehaviour
{
    private GameObject chapter;
    private Button chapterButton;

    public void Initialize(GameObject chapterObject, bool isUnlocked)
    {
        chapter = chapterObject;
        chapterButton = chapter.GetComponent<Button>();
        UpdateButtonState(isUnlocked);
    }

    private void UpdateButtonState(bool isUnlocked)
    {
        if (isUnlocked) chapterButton.interactable = true;
        else chapterButton.interactable = false;
    }
}