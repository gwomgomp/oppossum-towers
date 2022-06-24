using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
    public enum TargetingMethod {
        HighestPriority,
        LowestPriority,
        Closest
    }

    public TargetingMethod targetingMethod;

    public float damagePerShot;
    public float shotCooldown;
    public float range;

    private float currentShotCooldown = 0.0f;

    private HashSet<Enemy> enemiesInRange;
    private Enemy currentTarget = null;

    void Start() {
        enemiesInRange = new HashSet<Enemy>();
        currentShotCooldown = shotCooldown;

        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

        capsuleCollider.radius = range;
        capsuleCollider.height = range * 4.0f;
    }

    void Update() {
        if (enemiesInRange.Count > 0) {
            SelectNewTarget();
        }

        UpdateShotCooldown();
    }

    private void UpdateShotCooldown() {
        if (currentShotCooldown <= 0.0f && currentTarget != null) {
            ShootAtEnemy();
            currentShotCooldown = shotCooldown;
        } else {
            currentShotCooldown -= Time.deltaTime;
        }
    }

    private void ShootAtEnemy() {
        if (currentTarget != null) {
            bool killed = currentTarget.Damage(damagePerShot);
            if (killed) {
                enemiesInRange.Remove(currentTarget);
            }
        }
    }

    private void SelectNewTarget() {
        switch (targetingMethod) {
            case TargetingMethod.HighestPriority:
                currentTarget = FindHighestPriorityEnemy();
                break;
            case TargetingMethod.LowestPriority:
                currentTarget = FindLowestPriorityEnemy();
                break;
            case TargetingMethod.Closest:
                currentTarget = FindClosestEnemy();
                break;
            default:
                currentTarget = null;
                break;
        }
    }

    private Enemy FindHighestPriorityEnemy() {
        Enemy target = null;

        foreach (Enemy enemy in enemiesInRange) {
            if (target == null) {
                target = enemy;
            } else {
                if (target.GetPriority() < enemy.GetPriority()) {
                    target = enemy;
                }
            }
        }

        VisualizeTargeting(target);

        return target;
    }

    private Enemy FindLowestPriorityEnemy() {
        Enemy target = null;

        foreach (Enemy enemy in enemiesInRange) {
            if (target == null) {
                target = enemy;
            } else {
                if (target.GetPriority() > enemy.GetPriority()) {
                    target = enemy;
                }
            }
        }

        VisualizeTargeting(target);

        return target;
    }

    private Enemy FindClosestEnemy() {
        Enemy target = null;

        foreach (Enemy enemy in enemiesInRange) {
            if (target == null) {
                target = enemy;
            } else {
                if (Vector3.Distance(transform.position, enemy.gameObject.transform.position) < Vector3.Distance(transform.position, target.gameObject.transform.position)) {
                    target = enemy;
                }
            }
        }

        VisualizeTargeting(target);

        return target;
    }

    // This is purely for visualization purposes (i.e. serves no functional purpose)
    private void VisualizeTargeting(Enemy target) {
        if (currentTarget != target) {
            if (currentTarget != null) {
                if (currentTarget != null) {
                    currentTarget.BeUntargeted();
                }
            }

            if (target != null) {
                target.BeTargeted();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<Enemy>(out Enemy enemy)) {
            enemiesInRange.Add(enemy);
        }

        SelectNewTarget();
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent<Enemy>(out Enemy enemy)) {
            enemiesInRange.Remove(enemy);
        }

        SelectNewTarget();
    }
}
