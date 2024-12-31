using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI healthText;
    
    [Header("Animation")]
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private float flashDuration = 0.2f;
    
    private Color originalHealthColor;
    private float flashTimer;

    private void Start()
    {
        originalHealthColor = healthText.color;
    }

    private void Update()
    {
        // Update displays
        int currentGold = GameManager.Instance.GetPlayerGold();
        goldText.text = $"Gold: ${currentGold}";

        int currentHealth = GameManager.Instance.GetPlayerHealth();
        healthText.text = $"Health: {currentHealth}";

        // Handle damage flash animation
        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            float t = flashTimer / flashDuration;
            healthText.color = Color.Lerp(originalHealthColor, damageFlashColor, t);
        }
    }

    public void FlashHealthText()
    {
        flashTimer = flashDuration;
    }
} 