using UnityEngine;
using UnityEngine.UI;

public class BoostUIComponent : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private Slider boostSlider;

    [SerializeField]
    private Image fillImage;

    [SerializeField]
    private GameObject boostActiveIndicator;

    [Header("UI Settings")]
    [SerializeField]
    private Color fullEnergyColor = Color.green;

    [SerializeField]
    private Color lowEnergyColor = Color.red;

    [SerializeField]
    private float lowEnergyThreshold = 0.3f; // 30% energy or less is considered low

    // Boost effect animations
    [SerializeField]
    private bool animateWhenBoosting = true;

    [SerializeField]
    private float pulseSpeed = 2f;

    [SerializeField]
    private float pulseAmount = 0.1f;

    private BoostController boostController;
    private bool isBoosting = false;
    private float pulseTime = 0f;

    private void Start()
    {
        // Initialize the slider
        if (boostSlider != null)
        {
            boostSlider.minValue = 0f;
            boostSlider.maxValue = 1f;
            boostSlider.value = 1f;
        }

        // Get fill image if not assigned
        if (fillImage == null && boostSlider != null)
        {
            fillImage = boostSlider.fillRect.GetComponent<Image>();
        }

        // Find the boost controller if not assigned
        if (boostController == null)
        {
            boostController = FindObjectOfType<BoostController>();
            if (boostController != null)
            {
                boostController.OnBoostStateChanged += HandleBoostStateChanged;
                boostController.OnBoostEnergyChanged += UpdateBoostSlider;
            }
        }
    }

    private void Update()
    {
        // Animate the boost slider when active
        if (animateWhenBoosting && isBoosting && fillImage != null)
        {
            pulseTime += Time.deltaTime * pulseSpeed;

            // Pulsate the slider color
            float pulse = 0.5f + (Mathf.Sin(pulseTime) * pulseAmount);
            fillImage.color = Color.Lerp(fillImage.color, Color.white, pulse);
        }
    }

    private void OnDestroy()
    {
        // Clean up event subscription
        if (boostController != null)
        {
            boostController.OnBoostStateChanged -= HandleBoostStateChanged;
            boostController.OnBoostEnergyChanged -= UpdateBoostSlider;
        }
    }

    private void UpdateBoostSlider(float fillPercentage)
    {
        if (boostSlider != null)
        {
            // Update slider value
            boostSlider.value = fillPercentage;

            // Update fill color if we have access to the fill image and not boosting
            if (fillImage != null && !isBoosting)
            {
                fillImage.color =
                    (fillPercentage <= lowEnergyThreshold)
                        ? lowEnergyColor
                        : Color.Lerp(
                            lowEnergyColor,
                            fullEnergyColor,
                            (fillPercentage - lowEnergyThreshold) / (1f - lowEnergyThreshold)
                        );
            }
        }
    }

    private void HandleBoostStateChanged(bool isBoosting)
    {
        this.isBoosting = isBoosting;

        // Show/hide the boost active indicator if available
        if (boostActiveIndicator != null)
        {
            boostActiveIndicator.SetActive(isBoosting);
        }

        // Reset pulse effect when boost ends
        if (!isBoosting && fillImage != null)
        {
            pulseTime = 0f;
            UpdateBoostSlider(boostController.BoostEnergyPercentage);
        }
    }
}
