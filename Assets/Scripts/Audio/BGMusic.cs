using UnityEngine;

public class BGMusic : MonoBehaviour
{
    private uint bankID;
    private readonly string[] soundEvents = { "Level1Music" };

    void Start()
    {
        AkUnitySoundEngine.LoadBank("Main", out bankID);
        PlayRandomMusic();
    }

    void PlayRandomMusic()
    {
        // Choose a random sound event from the list
        int randomIndex = Random.Range(0, soundEvents.Length);
        string randomSoundEvent = soundEvents[randomIndex];

        AkUnitySoundEngine.PostEvent(randomSoundEvent, gameObject);
    }
}