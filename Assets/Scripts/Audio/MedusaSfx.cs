using System.Collections;
using UnityEngine;

public class MedusaSfx : MonoBehaviour
{
    private uint bankID;
    private Animator animator;

    private bool isWalking;
    private bool isPlayingFootsteps = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        AkUnitySoundEngine.LoadBank("Main", out bankID);
    }

    void Update()
    {
        bool isRunning = animator.GetBool("isRunning");
        float walkSpeed = animator.speed;

        //AkUnitySoundEngine.SetRTPCValue("MedusaWalkSpeed", walkSpeed);

        // Play footsteps only when running starts
        if (isRunning && !isPlayingFootsteps)
        {
            isPlayingFootsteps = true;
            StartCoroutine(PlayFootstepSounds(walkSpeed));
        }
        else if (!isRunning)
        {
            isPlayingFootsteps = false;
            StopAllCoroutines(); // Stop playing footsteps when player stops walking
        }
    }

    private IEnumerator PlayFootstepSounds(float speed)
    {
        while (true)
        {
            AkUnitySoundEngine.PostEvent("MedusaWalk", gameObject); // Play random footstep
            float stepInterval = 0.5f / speed; // Adjust timing based on animation speed
            yield return new WaitForSeconds(stepInterval); // Wait before playing next step
        }
    }
}