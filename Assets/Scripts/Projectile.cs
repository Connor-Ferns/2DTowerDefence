using UnityEngine;

public class Projectile : MonoBehaviour{
    [Header("Projectile Properties")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private int damage = 2;
    [SerializeField] private float maxLifetime = 5f;

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
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime || target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = PredictTargetPosition();
        float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);

        if (distanceToTarget < 0.1f)
        {
            HitTarget();
            return;
        }

        float moveDistance = speed * Time.deltaTime;
        if (moveDistance >= distanceToTarget)
        {
            HitTarget();
            return;
        }

        Vector2 direction = (targetPosition - currentPosition).normalized;
        transform.position += (Vector3)(direction * moveDistance);
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private Vector2 PredictTargetPosition()
    {
        if (targetEnemy == null) return target.position;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        float timeToTarget = distanceToTarget / speed;

        Vector2 enemyPos = target.position;
        Vector2 enemyVelocity = targetEnemy.GetVelocity();

        return enemyPos + (enemyVelocity * timeToTarget);
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