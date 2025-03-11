using System;
using System.Collections;
using UnityEngine;

public class BoostController : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField]
    private float maxBoostEnergy = 100f;

    [SerializeField]
    private float currentBoostEnergy;

    [SerializeField]
    private float boostEnergyConsumptionRate = 25f; // Energy used per second while boosting

    [SerializeField]
    private float boostEnergyRechargeRate = 15f; // Energy recharged per second while not boosting

    [Header("Cooldown Settings")]
    [SerializeField]
    private float boostCooldownTime = 2f; // Time to wait before allowing boost again after depletion

    [Header("Boost Force")]
    [SerializeField]
    private float boostForce = 10f; // The force applied when boosting

    [SerializeField]
    private ForceMode2D forceMode = ForceMode2D.Force; // How the force is applied

    [Header("Effects")]
    [SerializeField]
    private AudioClip boostStartSound;

    [SerializeField]
    private AudioClip boostLoopSound;

    [SerializeField]
    private AudioClip boostEndSound;

    [SerializeField]
    private ParticleSystem boostParticles;

    [SerializeField]
    private GameObject boostVfxPrefab; // Optional alternative to particle system

    // Events
    public delegate void BoostStateChanged(bool isBoosting);
    public event BoostStateChanged OnBoostStateChanged;

    public delegate void BoostEnergyChanged(float energyPercentage);
    public event BoostEnergyChanged OnBoostEnergyChanged;

    public delegate void BoostCooldownStarted(float cooldownDuration);
    public event BoostCooldownStarted OnBoostCooldownStarted;

    public delegate void BoostCooldownEnded();
    public event BoostCooldownEnded OnBoostCooldownEnded;

    // Properties
    public float BoostEnergyPercentage => currentBoostEnergy / maxBoostEnergy;
    public bool CanBoost => currentBoostEnergy > 0f && !isInCooldown;
    public bool IsBoosting { get; private set; }
    public float BoostForce => boostForce;
    public bool IsInCooldown => isInCooldown;

    // Audio components
    private AudioSource sfxAudioSource;
    private AudioSource loopAudioSource;

    // VFX game object reference
    private GameObject activeBoostVfx;

    // Cooldown tracking
    private bool isInCooldown = false;
    private Coroutine cooldownCoroutine;

    private void Start()
    {
        // Initialize boost energy to maximum
        currentBoostEnergy = maxBoostEnergy;

        // Notify UI of initial energy level
        OnBoostEnergyChanged?.Invoke(BoostEnergyPercentage);

        // Set up audio sources
        SetupAudioSources();
    }

    private void SetupAudioSources()
    {
        // Create audio source for one-shot sounds
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.playOnAwake = false;
        sfxAudioSource.spatialBlend = 0f; // 2D sound

        // Create separate audio source for looping boost sound
        loopAudioSource = gameObject.AddComponent<AudioSource>();
        loopAudioSource.playOnAwake = false;
        loopAudioSource.spatialBlend = 0f; // 2D sound
        loopAudioSource.loop = true;

        if (boostLoopSound != null)
        {
            loopAudioSource.clip = boostLoopSound;
        }
    }

    private void Update()
    {
        UpdateBoostState();
        UpdateBoostEnergy();
    }

    private void UpdateBoostState()
    {
        bool wasBoosting = IsBoosting;

        // Check if player is pressing boost button and has energy and not in cooldown
        if (Input.GetKey(KeyCode.Space) && CanBoost)
        {
            IsBoosting = true;
        }
        else
        {
            IsBoosting = false;
        }

        // Notify listeners if boost state changed
        if (wasBoosting != IsBoosting)
        {
            OnBoostStateChanged?.Invoke(IsBoosting);

            // Play appropriate sounds and effects
            if (IsBoosting)
            {
                StartBoostEffects();
            }
            else
            {
                StopBoostEffects();
            }
        }
    }

    private void StartBoostEffects()
    {
        // Play start sound
        if (boostStartSound != null && sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(boostStartSound);
        }

        // Start loop sound
        if (boostLoopSound != null && loopAudioSource != null)
        {
            loopAudioSource.Play();
        }

        // Start particle system if assigned
        if (boostParticles != null)
        {
            boostParticles.Play();
        }

        // Instantiate VFX prefab if assigned
        if (boostVfxPrefab != null && activeBoostVfx == null)
        {
            boostVfxPrefab.SetActive(true);
        }
    }

    private void StopBoostEffects()
    {
        // Play end sound
        if (boostEndSound != null && sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(boostEndSound);
        }

        // Stop loop sound
        if (loopAudioSource != null && loopAudioSource.isPlaying)
        {
            loopAudioSource.Stop();
        }

        // Stop particle system
        if (boostParticles != null && boostParticles.isPlaying)
        {
            boostParticles.Stop();
        }

        // Disable VFX prefab
        if (boostVfxPrefab != null)
        {
            boostVfxPrefab.SetActive(false);
        }
    }

    private void UpdateBoostEnergy()
    {
        float previousEnergy = currentBoostEnergy;

        if (IsBoosting)
        {
            // Consume boost energy
            currentBoostEnergy = Mathf.Max(
                0f,
                currentBoostEnergy - (boostEnergyConsumptionRate * Time.deltaTime)
            );

            // Check if energy is depleted
            if (currentBoostEnergy <= 0f)
            {
                // Disable boosting
                IsBoosting = false;
                OnBoostStateChanged?.Invoke(IsBoosting);
                StopBoostEffects();

                // Start cooldown
                StartBoostCooldown();
            }
        }
        else
        {
            // CHANGE: Always recharge boost energy, even during cooldown
            currentBoostEnergy = Mathf.Min(
                maxBoostEnergy,
                currentBoostEnergy + (boostEnergyRechargeRate * Time.deltaTime)
            );
        }

        // Notify listeners if energy changed significantly
        if (Mathf.Abs(previousEnergy - currentBoostEnergy) > 0.01f)
        {
            OnBoostEnergyChanged?.Invoke(BoostEnergyPercentage);

            // Adjust loop sound volume based on energy level if boosting
            if (IsBoosting && loopAudioSource != null && loopAudioSource.isPlaying)
            {
                loopAudioSource.volume = Mathf.Max(0.3f, BoostEnergyPercentage);
            }
        }
    }

    private void StartBoostCooldown()
    {
        // Start cooldown coroutine
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        cooldownCoroutine = StartCoroutine(BoostCooldownRoutine());
    }

    private IEnumerator BoostCooldownRoutine()
    {
        // Set cooldown flag
        isInCooldown = true;

        // Notify listeners that cooldown has started
        OnBoostCooldownStarted?.Invoke(boostCooldownTime);

        // Wait for cooldown time
        yield return new WaitForSeconds(boostCooldownTime);

        // End cooldown
        isInCooldown = false;

        // Notify listeners that cooldown has ended
        OnBoostCooldownEnded?.Invoke();
    }

    // Method to add boost energy (could be used for pickups)
    public void AddBoostEnergy(float amount)
    {
        currentBoostEnergy = Mathf.Min(maxBoostEnergy, currentBoostEnergy + amount);
        OnBoostEnergyChanged?.Invoke(BoostEnergyPercentage);

        // Play a pickup sound if desired
        // sfxAudioSource.PlayOneShot(energyPickupSound);
    }

    // Gets the boost force vector based on the direction provided
    public Vector2 GetBoostForceVector(Vector2 direction)
    {
        return direction.normalized * boostForce;
    }

    // Method to manually end cooldown (for power-ups or testing)
    public void EndCooldown()
    {
        if (isInCooldown && cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            isInCooldown = false;
            OnBoostCooldownEnded?.Invoke();
        }
    }

    private void OnDisable()
    {
        // Make sure to stop all effects if the component is disabled
        StopBoostEffects();

        // Stop any active cooldown coroutine
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
    }
}
