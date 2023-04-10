using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Towers;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHitEvent : UnityEvent<GameObject, float> { }

public class PositionHitEvent : UnityEvent<Vector3> { }

public class Tower : MonoBehaviour {
    public TowerType towerType;

    private float currentShotCooldown = 0.0f;

    private HashSet<Enemy> enemiesInRange;
    private List<Target> currentTargets = new();

    private GameObject launchOrigin;

    void Start() {
        enemiesInRange = new HashSet<Enemy>();
        currentShotCooldown = towerType.shotCooldown;

        CapsuleCollider capsuleCollider = this.RequireComponent<CapsuleCollider>();

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

            if (currentTargets.Count > 0) {
                ShootAtCurrentTargets();
                currentShotCooldown = towerType.shotCooldown;
            }
        }
    }

    private void ShootAtCurrentTargets() {
        if (towerType.projectilePrefab != null) {
            foreach (var currentTarget in currentTargets) {
                GameObject projectileObject = Instantiate(towerType.projectilePrefab, launchOrigin.transform.position, Quaternion.identity);
                Projectile projectile = projectileObject.RequireComponent<Projectile>();

                if (towerType.projectileTracksEnemy) {
                    projectile.SetTargetObject(currentTarget.Enemy.gameObject);
                } else {
                    projectile.SetTargetPosition(currentTarget.Enemy.gameObject.transform.position);
                }

                projectile.SetSpeed(towerType.projectileSpeed);

                currentTarget.ConsecutiveHits += 1f;
                projectile.ConescutiveHits = currentTarget.ConsecutiveHits;

                EnemyHitEvent enemyHitEvent = new EnemyHitEvent();
                enemyHitEvent.AddListener(OnEnemyHit);

                PositionHitEvent positionHitEvent = new PositionHitEvent();
                positionHitEvent.AddListener(OnPositionHit);

                projectile.SetEnemyHitEvent(enemyHitEvent);
                projectile.SetPositionHitEvent(positionHitEvent);
            }
        }
    }

    private void UpdateTarget() {
        PurgeDestroyedEnemies();
        PurgeOutOfRangeEnemies();

        //Debug.Log($"{towerType.targetCount}, {currentTargets.Count}, {enemiesInRange.Count}");
        // If multitarget is at max but there are still other enemies in range
        if (currentTargets.Count == towerType.targetCount && currentTargets.Count < enemiesInRange.Count) {
            switch (towerType.targetingMethod) {
                case TowerType.TargetingMethod.HighestPriority:
                    FindAnyHigherPriorityEnemy();
                    break;
                case TowerType.TargetingMethod.LowestPriority:
                    FindAnyLowerPriorityEnemy();
                    break;
                case TowerType.TargetingMethod.Closest:
                    FindAnyCloserEnemy();
                    break;
            }
        }

        while (currentTargets.Count < towerType.targetCount && currentTargets.Count < enemiesInRange.Count) {
            currentTargets.Add(new Target {
                Enemy = towerType.targetingMethod switch {
                    TowerType.TargetingMethod.HighestPriority => FindHighestPriorityEnemy(),
                    TowerType.TargetingMethod.LowestPriority => FindLowestPriorityEnemy(),
                    TowerType.TargetingMethod.Closest => FindClosestEnemy(),
                    _ => throw new System.NotImplementedException()
                }
            });
        }
    }

    private void FindAnyHigherPriorityEnemy() {
        currentTargets.ForEach(target => {
            foreach (Enemy enemy in enemiesInRange) {
                if (!currentTargets.Any(target => target.Enemy.Equals(enemy)) && target.Enemy.GetPriority() < enemy.GetPriority()) {
                    target.Enemy = enemy;
                    target.ConsecutiveHits = 0f;
                }
            }
        });
    }

    private Enemy FindHighestPriorityEnemy() {
        Enemy target = null;

        foreach (Enemy enemy in enemiesInRange) {
            if (!currentTargets.Any(target => target.Enemy.Equals(enemy))) {
                if (target == null) {
                    target = enemy;
                } else if (target.GetPriority() < enemy.GetPriority()) {
                    target = enemy;
                }
            }
        }

        return target;
    }

    private void FindAnyLowerPriorityEnemy() {
        currentTargets.ForEach(target => {
            foreach (Enemy enemy in enemiesInRange) {
                if (!currentTargets.Any(target => target.Enemy.Equals(enemy)) && target.Enemy.GetPriority() > enemy.GetPriority()) {
                    target.Enemy = enemy;
                    target.ConsecutiveHits = 0f;
                }
            }
        });
    }

    private Enemy FindLowestPriorityEnemy() {
        Enemy target = null;

        foreach (Enemy enemy in enemiesInRange) {
            if (!currentTargets.Any(target => target.Enemy.Equals(enemy))) {
                if (target == null) {
                    target = enemy;
                } else if (target.GetPriority() > enemy.GetPriority()) {
                    target = enemy;
                }
            }
        }

        return target;
    }

    private void FindAnyCloserEnemy() {
        currentTargets.ForEach(target => {
            foreach (Enemy enemy in enemiesInRange) {
                if (!currentTargets.Any(target => target.Enemy.Equals(enemy))) {
                    var potentialCloserTarget = GetCloserEnemy(target.Enemy, enemy);
                    if (!potentialCloserTarget.Equals(target)) {
                        target.Enemy = potentialCloserTarget;
                        target.ConsecutiveHits = 0f;
                    }
                }
            }
        });
    }

    private Enemy FindClosestEnemy() {
        Enemy target = null;

        foreach (Enemy enemy in enemiesInRange) {
            if (!currentTargets.Any(target => target.Enemy.Equals(enemy))) {
                if (target == null) {
                    target = enemy;
                } else {
                    target = GetCloserEnemy(target, enemy);
                }
            }
        }

        return target;
    }

    private Enemy GetCloserEnemy(Enemy left, Enemy right) {
        var leftDistance = Vector3.Distance(transform.position, left.gameObject.transform.position);
        var rightDistance = Vector3.Distance(transform.position, right.gameObject.transform.position);
        if (leftDistance < rightDistance) {
            return left;
        } else {
            return right;
        }
    }

    private void PurgeDestroyedEnemies() {
        enemiesInRange.RemoveWhere(enemy => enemy == null || enemy.gameObject == null);
        currentTargets.RemoveAll(target => target.Enemy == null || target.Enemy.gameObject == null);
    }

    private void PurgeOutOfRangeEnemies() {
        currentTargets.RemoveAll(target => !enemiesInRange.Contains(target.Enemy));
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponentInParentForTag(TagConstants.ENEMY, out Enemy enemy)) {
            enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponentInParentForTag(TagConstants.ENEMY, out Enemy enemy)) {
            enemiesInRange.Remove(enemy);
        }
    }

    private void OnEnemyHit(GameObject enemyObject, float consecutiveHits) {
        if (towerType.areaEffectPrefab != null) {
            Instantiate(towerType.areaEffectPrefab, enemyObject.transform.position, Quaternion.identity);
        }

        if (enemyObject.TryGetComponent(out Enemy enemy)) {
            enemy.Damage(towerType.damagePerShot * ConsecutiveMultiplier(consecutiveHits));

            if (towerType.statusEffect != null) {
                enemy.ApplyTimedStatusEffect(towerType.statusEffect, this);
            }
        }
    }

    /// <summary>
    /// At the moment calculates from 0 to rampUpMultiplier, might change that to 1 to rampUpMultiplier?
    /// </summary>
    /// <param name="consecutiveHits"></param>
    /// <returns></returns>
    private float ConsecutiveMultiplier(float consecutiveHits) {
        if (towerType.rampUpShotsNeeded == 0)
            return 1;

        var rampUpCount = towerType.rampUpShotsNeeded <= consecutiveHits ? towerType.rampUpShotsNeeded : consecutiveHits;

        return towerType.rampUpMultiplier / towerType.rampUpShotsNeeded * rampUpCount;
    }

    private void OnPositionHit(Vector3 position) {
        if (towerType.areaEffectPrefab != null) {
            Instantiate(towerType.areaEffectPrefab, position, Quaternion.identity);
        }
    }
}
