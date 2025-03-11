using System.Collections;
using UnityEngine;

public class SpeedGate : MonoBehaviour
{
    [Header("Boost Settings")]
    public float boostDuration = 2.0f; // How long the boost lasts
    public float boostForce = 15.0f; // The force applied by this speed gate

    [Header("Visual Settings")]
    [SerializeField]
    private SpriteRenderer gateRenderer;

    [SerializeField]
    private Color activatedColor = Color.yellow;

    [SerializeField]
    private float colorFadeDuration = 1.0f;

    [SerializeField]
    private ParticleSystem activationParticles;

    [Header("Audio")]
    [SerializeField]
    private AudioClip activationSound;

    private Color originalColor;
    private AudioSource audioSource;

    private void Awake()
    {
        if (gateRenderer == null)
            gateRenderer = GetComponent<SpriteRenderer>();

        if (gateRenderer != null)
            originalColor = gateRenderer.color;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && activationSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Activate()
    {
        // Play activation particles
        if (activationParticles != null)
            activationParticles.Play();

        // Play activation sound
        if (audioSource != null && activationSound != null)
            audioSource.PlayOneShot(activationSound);

        // Change color temporarily
        if (gateRenderer != null)
            StartCoroutine(FlashColor());
    }

    private IEnumerator FlashColor()
    {
        // Change to activation color
        gateRenderer.color = activatedColor;

        // Wait and fade back to original
        float elapsedTime = 0;
        while (elapsedTime < colorFadeDuration)
        {
            gateRenderer.color = Color.Lerp(
                activatedColor,
                originalColor,
                elapsedTime / colorFadeDuration
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Make sure we end with the exact original color
        gateRenderer.color = originalColor;
    }
}
