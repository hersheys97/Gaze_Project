using System.Collections;
using UnityEngine;

public class StoneShader : MonoBehaviour
{
    public Material enemyMaterial; // Shader material
    public GameObject enemyObject;
    public Sprite unfrozenSprite;
    public Sprite frozenSprite;
    private EnemyBehaviour enemyScript;
    private SpriteRenderer enemyRenderer;

    private const float totalFrozenTime = 7f;
    private const float transitionTime = 2f;

    private float progress = 0f;
    private Material instanceMaterial;
    private bool isFrozenState = false;
    private bool isTransitioning = false; // Prevents multiple transitions at once

    void Start()
    {
        enemyScript = GetComponent<EnemyBehaviour>();
        enemyRenderer = enemyObject.GetComponent<SpriteRenderer>();

        // Clone material per enemy
        instanceMaterial = new Material(enemyMaterial);
        enemyRenderer.material = instanceMaterial;

        // Ensure enemy starts in unfrozen state
        SetMaterialTexture(unfrozenSprite); // unable to stay in this because animation
    }

    void Update()
    {
        if (enemyScript == null) return;

        // Start the transition to frozen state
        if (enemyScript.isFrozen && !isFrozenState && !isTransitioning)
        {
            isFrozenState = true;
            isTransitioning = true;
            StartCoroutine(FreezeAndUnfreezeCycle());
        }
    }

    IEnumerator FreezeAndUnfreezeCycle()
    {
        // Transition to frozen (2 seconds)
        yield return StartCoroutine(StartStoneEffect(unfrozenSprite, frozenSprite, 1f));

        // Stay in frozen state for 3 seconds
        yield return new WaitForSeconds(totalFrozenTime - (2 * transitionTime));

        // Transition back to normal (2 seconds)
        yield return StartCoroutine(StartStoneEffect(frozenSprite, unfrozenSprite, 0f));

        // Reset state
        isFrozenState = false;
        isTransitioning = false;
        //SetMaterialTexture(unfrozenSprite);
    }

    IEnumerator StartStoneEffect(Sprite currentSprite, Sprite targetSprite, float targetProgress)
    {
        float elapsedTime = 0f;
        float startProgress = progress;

        // Update shader textures before transition
        SetMaterialTexture(currentSprite);
        instanceMaterial.SetTexture("_SecondTex", targetSprite.texture);

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            progress = Mathf.Lerp(startProgress, targetProgress, elapsedTime / transitionTime);
            instanceMaterial.SetFloat("_StoneProgress", progress);
            yield return null;
        }

        // Ensure final state is properly set
        progress = targetProgress;
        instanceMaterial.SetFloat("_StoneProgress", progress);
        SetMaterialTexture(currentSprite);
    }

    private void SetMaterialTexture(Sprite sprite)
    {
        enemyRenderer.sprite = sprite;
        instanceMaterial.SetTexture("_MainTex", sprite.texture);
    }
}