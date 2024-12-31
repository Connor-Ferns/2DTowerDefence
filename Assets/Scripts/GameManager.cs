using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private int startingHealth = 100;
    
    private int playerHealth;

    [Header("Economy Settings")]
    [SerializeField] private int startingGold = 100;
    [SerializeField] private int waveCompletionBonus = 50;
    [SerializeField] private float waveCompletionBonusIncrease = 0.1f; // 10% increase per wave
    [SerializeField] private int interestRate = 5; // 5% interest between waves
    
    private int playerGold;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            playerHealth = startingHealth;
            playerGold = startingGold;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        playerHealth = Mathf.Max(0, playerHealth - damage);
        
        if (playerHealth <= 0)
        {
            GameOver();
        }
    }

    public int GetPlayerHealth()
    {
        return playerHealth;
    }

    public void AddGold(int amount)
    {
        playerGold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (playerGold >= amount)
        {
            playerGold -= amount;
            return true;
        }
        return false;
    }

    public int GetPlayerGold()
    {
        return playerGold;
    }

    public void AwardWaveCompletionBonus(int waveNumber)
    {
        float bonusMultiplier = 1f + (waveCompletionBonusIncrease * waveNumber);
        int bonus = Mathf.RoundToInt(waveCompletionBonus * bonusMultiplier);
        AddGold(bonus);
    }

    private void CalculateInterest()
    {
        int interest = Mathf.RoundToInt(playerGold * (interestRate / 100f));
        AddGold(interest);
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        // Implement game over logic here
    }
} 