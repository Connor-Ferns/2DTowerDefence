using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Properties")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 2;
    [SerializeField] private float maxLifetime = 5f;
    [SerializeField] private float leadMultiplier = 1.2f;

    private Transform target;
    private float lifetime = 0f;
    private Enemy targetEnemy;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        targetEnemy = target.GetComponent<Enemy>();
    }

    private void Update()
    {
        if (lifetime >= maxLifetime || target == null)
        {
            Destroy(gameObject);
            return;
        }
        lifetime += Time.deltaTime;

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = PredictTargetPosition();
        
        Vector2 direction = (targetPosition - currentPosition).normalized;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    private Vector2 PredictTargetPosition()
    {
        if (targetEnemy == null) return target.position;

        Vector2 targetPos = target.position;
        Vector2 targetVelocity = targetEnemy.GetVelocity();
        Vector2 toTarget = targetPos - (Vector2)transform.position;
        float distance = toTarget.magnitude;
        float timeToTarget = distance / speed;

        return targetPos + (targetVelocity * timeToTarget * leadMultiplier);
    }

    void HitTarget()
    {
        if (targetEnemy != null)
        {
            targetEnemy.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}