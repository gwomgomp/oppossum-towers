using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHitEvent : UnityEvent<GameObject> { }

public class PositionHitEvent : UnityEvent<Vector3> { }

public class Tower : MonoBehaviour {
    public TowerType towerType;

    private float currentShotCooldown = 0.0f;

    private List<Collider> enemiesInRange;
    private Collider currentTarget = null;

    private GameObject launchOrigin;

    void Start() {
        enemiesInRange = new List<Collider>();
        currentShotCooldown = towerType.shotCooldown;

        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

        capsuleCollider.radius = towerType.range;
        capsuleCollider.height = towerType.range * 4.0f;

        launchOrigin = gameObject;

        foreach (Transform child in transform) {
            if (child.CompareTag(TagConstants.LAUNCHORIGIN)) {
                launchOrigin = child.gameObject;
                break;
            }
        }
    }

    void Update() {
        if (currentShotCooldown > 0.0f) {
            currentShotCooldown -= Time.deltaTime;
        } else {
            UpdateTarget();

            if (currentTarget != null) {
                ShootAtEnemy();
                currentShotCooldown = towerType.shotCooldown;
            }
        }
    }

    private void ShootAtEnemy() {
        if (towerType.projectilePrefab != null) {
            GameObject projectileObject = Instantiate(towerType.projectilePrefab, launchOrigin.transform.position, Quaternion.identity);
            Projectile projectile = projectileObject.GetComponent<Projectile>();

            if (towerType.projectileTracksEnemy) {
                projectile.SetTargetObject(currentTarget.gameObject);
            } else {
                projectile.SetTargetPosition(currentTarget.gameObject.transform.position);
            }

            projectile.SetSpeed(towerType.projectileSpeed);

            EnemyHitEvent enemyHitEvent = new EnemyHitEvent();
            enemyHitEvent.AddListener(OnEnemyHit);

            PositionHitEvent positionHitEvent = new PositionHitEvent();
            positionHitEvent.AddListener(OnPositionHit);

            projectile.SetEnemyHitEvent(enemyHitEvent);
            projectile.SetPositionHitEvent(positionHitEvent);
        }
    }

    private void UpdateTarget() {
        PurgeDestroyedEnemies();

        currentTarget = towerType.targetingMethod switch {
            TowerType.TargetingMethod.HighestPriority => FindHighestPriorityEnemy(),
            TowerType.TargetingMethod.LowestPriority => FindLowestPriorityEnemy(),
            TowerType.TargetingMethod.Closest => FindClosestEnemy(),
            _ => null
        };
    }

    private Collider FindHighestPriorityEnemy() {
        Collider target = null;

        foreach (Collider collider in enemiesInRange) {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();

            if (enemy != null) {
                if (target == null) {
                    target = collider;
                } else {
                    Enemy targetStats = target.gameObject.GetComponent<Enemy>();

                    if (targetStats != null && targetStats.GetPriority() < enemy.GetPriority()) {
                        target = collider;
                    }
                }
            }
        }

        return target;
    }

    private Collider FindLowestPriorityEnemy() {
        Collider target = null;

        foreach (Collider collider in enemiesInRange) {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();

            if (enemy != null) {
                if (target == null) {
                    target = collider;
                } else {
                    Enemy targetStats = target.gameObject.GetComponent<Enemy>();

                    if (targetStats != null && targetStats.GetPriority() > enemy.GetPriority()) {
                        target = collider;
                    }
                }
            }
        }

        return target;
    }

    private Collider FindClosestEnemy() {
        Collider target = null;

        foreach (Collider collider in enemiesInRange) {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();

            if (enemy != null) {
                if (target == null) {
                    target = collider;
                } else {
                    Enemy targetStats = target.gameObject.GetComponent<Enemy>();

                    if (targetStats != null && Vector3.Distance(transform.position, collider.gameObject.transform.position) < Vector3.Distance(transform.position, target.gameObject.transform.position)) {
                        target = collider;
                    }
                }
            }
        }

        return target;
    }

    private void PurgeDestroyedEnemies() {
        for (int i = enemiesInRange.Count - 1; i >= 0; i--) {
            if (enemiesInRange[i] == null) {
                enemiesInRange.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(TagConstants.ENEMY) && !enemiesInRange.Contains(other)) {
            enemiesInRange.Add(other);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(TagConstants.ENEMY) && enemiesInRange.Contains(other)) {
            enemiesInRange.Remove(other);
        }
    }

    private void OnEnemyHit(GameObject enemyObject) {
        if (towerType.areaEffectPrefab != null) {
            Instantiate(towerType.areaEffectPrefab, enemyObject.transform.position, Quaternion.identity);
        }

        Enemy enemy = enemyObject.GetComponent<Enemy>();

        if (enemy != null) {
            enemy.Damage(towerType.damagePerShot);

            if (towerType.statusEffect != null) {
                enemy.ApplyTimedStatusEffect(towerType.statusEffect, this);
            }
        }
    }

    private void OnPositionHit(Vector3 position) {
        if (towerType.areaEffectPrefab != null) {
            Instantiate(towerType.areaEffectPrefab, position, Quaternion.identity);
        }
    }
}
