using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderNumberUpdate : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI numberText;

    void Start()
    {
        // Initialize Number text with default value
        UpdateNumberText(slider.value);

        // Add a listener to update the text whenever the slider value changes
        slider.onValueChanged.AddListener(UpdateNumberText);
    }

    // Method to update the text when the slider changes
    private void UpdateNumberText(float value)
    {
        numberText.text = value.ToString("F0"); // 0 decimal places
    }

    void OnDestroy()
    {
        // Remove listener to avoid memory leaks
        slider.onValueChanged.RemoveListener(UpdateNumberText);
    }
}