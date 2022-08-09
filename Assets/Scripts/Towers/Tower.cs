using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
    public enum TargetingMethod {
        HighestPriority,
        LowestPriority,
        Closest
    }

    public TargetingMethod targetingMethod;

    public TowerType type;

    private float currentShotCooldown = 0.0f;

    private HashSet<Enemy> enemiesInRange;
    private Enemy currentTarget = null;

    void Start() {
        enemiesInRange = new HashSet<Enemy>();
        currentShotCooldown = type.ShotCooldown;

        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

        capsuleCollider.radius = type.Range;
        capsuleCollider.height = type.Range * 4.0f;
    }

    void Update() {
        if (currentTarget == null && enemiesInRange.Count > 0) {
            SelectNewTarget();
        }

        UpdateShotCooldown();
    }

    private void UpdateShotCooldown() {
        if (currentShotCooldown <= 0.0f && currentTarget != null) {
            ShootAtEnemy();
            currentShotCooldown = type.ShotCooldown;
        } else {
            currentShotCooldown -= Time.deltaTime;
        }
    }

    private void ShootAtEnemy() {
        if (currentTarget != null) {
            bool killed = currentTarget.Damage(type.DamagePerShot);
            if (killed) {
                enemiesInRange.Remove(currentTarget);
            }
        }
    }

    private void SelectNewTarget() {
        currentTarget = targetingMethod switch {
            TargetingMethod.HighestPriority => FindHighestPriorityEnemy(),
            TargetingMethod.LowestPriority => FindLowestPriorityEnemy(),
            TargetingMethod.Closest => FindClosestEnemy(),
            _ => null
        };
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
        if (other.TryGetComponent(out Enemy enemy)) {
            enemiesInRange.Add(enemy);
        }

        SelectNewTarget();
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Enemy enemy)) {
            enemiesInRange.Remove(enemy);
        }

        SelectNewTarget();
    }
}
