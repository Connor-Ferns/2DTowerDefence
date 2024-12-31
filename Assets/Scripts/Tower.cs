using UnityEngine;

public class Tower : MonoBehaviour {
    [Header("Tower Properties")]
    [SerializeField] private float range = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    private float fireCooldown = 0f;
    private Enemy currentTarget;

    void Update() {
        if (currentTarget == null || !IsInRange(currentTarget.transform)) {
            FindNewTarget();
        }

        if (currentTarget != null && fireCooldown <= 0f) {
            Fire(currentTarget.transform);
            fireCooldown = 1f / fireRate;
        }

        fireCooldown -= Time.deltaTime;
    }

    private bool IsInRange(Transform target) {
        return Vector2.Distance(transform.position, target.position) <= range;
    }

    private void FindNewTarget() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);
        float shortestDistance = float.MaxValue;
        
        foreach (Collider2D collider in colliders) {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null) {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < shortestDistance) {
                    shortestDistance = distance;
                    currentTarget = enemy;
                }
            }
        }
    }

    // Optional: Visualize the range in the editor
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private void Fire(Transform target){
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetTarget(target);
    }
}
