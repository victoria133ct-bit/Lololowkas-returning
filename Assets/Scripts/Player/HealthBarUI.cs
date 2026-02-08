using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{

    [Header("References")]
    [Tooltip("Player's Health component")]
    public Health targetHealth;

    [Header("UI Elements")]
    [Tooltip("Slider for displaying health")]
    public Slider healthSlider;

    [Tooltip("Fill image (alternative to Slider)")]
    public Image healthFillImage;

    public Text healthText;

    [Header("Settings")]
    [Tooltip("Speed of health change animation")]
    public float smoothSpeed = 5f;

    [Tooltip("Color at full health")]
    public Color fullHealthColor = Color.green;

    [Tooltip("Color at low health")]
    public Color lowHealthColor = Color.red;

    [Tooltip("Low health threshold (0-1)")]
    [Range(0f, 1f)]
    public float lowHealthThreshold = 0.3f;

    private float targetValue;
    private float currentValue;

    private void Start()
    {
        // Initialize values
        if (targetHealth == null)
            targetHealth = GetComponentInParent<Health>();

        if (targetHealth != null)
        {
            currentValue = (float)targetHealth.Current / targetHealth.maxHp;
            targetValue = currentValue;
            UpdateUI(currentValue);
        }

    }

    private void Update()
    {
        // Smooth health change animation
        if (Mathf.Abs(currentValue - targetValue) > 0.01f)
        {
            currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * smoothSpeed);
            UpdateUI(currentValue);
        }
    }

    private void OnEnable()
    {
        // Subscribe to health change event
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += UpdateHealthBar;
        }
    }
    private void OnDisable()
    {
        // Subscribe to health change event
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }

    /// <summary>
    /// Called when health changes
    /// </summary>
    private void UpdateHealthBar(int current, int max)
    {
        targetValue = (float)current / max;
    }

    /// <summary>
    /// Updates visual representation
    /// </summary>
    private void UpdateUI(float value)
    {
        // Update Slider if present
        if (healthSlider != null)
        {
            healthSlider.value = value;
        }

        // Update Image if present
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = value;

            // Change color based on health amount
            healthFillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, value / lowHealthThreshold);

            healthText.text = $"{Math.Floor(value * 100)}/100";
        }
    }
}
