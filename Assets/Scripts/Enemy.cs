using UnityEngine;

public class Enemy : MonoBehaviour{
    [Header("Enemy Properties")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int goldValue = 10;

    public event System.Action<Enemy> OnEnemyDeath;
    public event System.Action<Enemy> OnEnemyReachedEnd;

    private int currentHealth;
    private Transform[] waypoints;
    private int currentWaypoint = 0;

    public float DistanceToEnd { get; private set; }

    private Vector2 currentVelocity;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateDistanceToEnd();
    }

    private void UpdateDistanceToEnd()
    {
        if (waypoints == null || currentWaypoint >= waypoints.Length)
        {
            DistanceToEnd = 0;
            return;
        }

        float distance = Vector2.Distance(transform.position, waypoints[currentWaypoint].position);
        for (int i = currentWaypoint + 1; i < waypoints.Length; i++)
        {
            distance += Vector2.Distance(waypoints[i-1].position, waypoints[i].position);
        }
        DistanceToEnd = distance;
    }

    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
    }

    void Update()
    {
        if (waypoints == null || currentWaypoint >= waypoints.Length) return;

        Vector2 oldPosition = transform.position;
        
        transform.position = Vector2.MoveTowards(
            transform.position, 
            waypoints[currentWaypoint].position, 
            speed * Time.deltaTime
        );

        // Calculate actual velocity
        currentVelocity = ((Vector2)transform.position - oldPosition) / Time.deltaTime;

        if (Vector2.Distance(transform.position, waypoints[currentWaypoint].position) < 0.1f)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                OnEnemyReachedEnd?.Invoke(this);
                GameManager.Instance.TakeDamage(1);
                Destroy(gameObject);
            }
        }

        UpdateDistanceToEnd();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Enemy took {damageAmount} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        OnEnemyDeath?.Invoke(this);
        GameManager.Instance.AddGold(goldValue);
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

    public Vector2 GetVelocity()
    {
        return currentVelocity;
    }

    public void ApplyWaveModifiers(float healthMultiplier, float speedMultiplier)
    {
        maxHealth = Mathf.RoundToInt(maxHealth * healthMultiplier);
        currentHealth = maxHealth;
        speed *= speedMultiplier;
    }
}