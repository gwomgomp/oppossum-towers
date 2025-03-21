using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHitEvent : UnityEvent<GameObject, float> { }

public class PositionHitEvent : UnityEvent<Vector3> { }

public class Tower : MonoBehaviour {
    public TowerType towerType;

    private float currentShotCooldown = 0.0f;

    private HashSet<Enemy> enemiesInRange;
    private readonly HashSet<Target> currentTargets = new();

    private GameObject launchOrigin;

    public GameObject animationObject;

    private Animator animator;

    private bool useLaunchEvent = false;

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

        if (animationObject != null && animationObject.TryGetComponent<LaunchEventHandler>(out LaunchEventHandler launchEventHandler)) {
            launchEventHandler.launchEvent = LaunchAttack;

            if (animationObject.TryGetComponent<Animator>(out animator)) {
                animator.SetFloat("attack_speed", 1f / towerType.shotCooldown);

                useLaunchEvent = true;

                Debug.Log("Using launch event");
            }
        }
    }

    void Update() {
        UpdateTarget();

        if (!useLaunchEvent) {
            if (currentShotCooldown > 0.0f) {
                currentShotCooldown -= Time.deltaTime;
            } else {
                if (currentTargets.Count > 0) {

                    if (towerType.attackAllInRange) {
                        AreaAroundSelf();
                    } else {
                        ShootAtCurrentTargets();
                    }

                    currentShotCooldown = towerType.shotCooldown;
                }
            }
        } else {
            animator.SetBool("firing", currentTargets.Count > 0);
        }

        if (towerType.turnTowardsTarget && currentTargets.Count > 0) {
            Vector3 targetPosition = new Vector3(
                currentTargets.First().Enemy.transform.position.x,
                this.transform.position.y,
                currentTargets.First().Enemy.transform.position.z
                );
            
            this.transform.LookAt(targetPosition);
        }
    }

    private void LaunchAttack() {
        UpdateTarget();

        if (currentTargets.Count > 0) {

            if (towerType.attackAllInRange) {
                AreaAroundSelf();
            } else {
                ShootAtCurrentTargets();
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

                var enemyHitEvent = new EnemyHitEvent();
                enemyHitEvent.AddListener(OnEnemyHit);

                var positionHitEvent = new PositionHitEvent();
                positionHitEvent.AddListener(OnPositionHit);

                projectile.SetEnemyHitEvent(enemyHitEvent);
                projectile.SetPositionHitEvent(positionHitEvent);
            }
        }
    }

    private void AreaAroundSelf() {
        if (towerType.projectilePrefab != null) {
            GameObject projectileObject = Instantiate(towerType.projectilePrefab, launchOrigin.transform.position, Quaternion.identity);
            Projectile projectile = projectileObject.RequireComponent<Projectile>();

            projectile.SetTargetPosition(launchOrigin.transform.position);
            projectile.SetSpeed(towerType.projectileSpeed);

            var enemyHitEvent = new EnemyHitEvent();
            enemyHitEvent.AddListener(OnEnemyHit);

            var positionHitEvent = new PositionHitEvent();
            positionHitEvent.AddListener(OnPositionHit);

            projectile.SetEnemyHitEvent(enemyHitEvent);
            projectile.SetPositionHitEvent(positionHitEvent);
        }
    }

    private void UpdateTarget() {
        PurgeDestroyedEnemies();
        PurgeOutOfRangeEnemies();

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
        foreach (Enemy enemy in enemiesInRange.Except(currentTargets.Select(target => target.Enemy))) {
            if (currentTargets.Any(target => target.Enemy.GetPriority() < enemy.GetPriority())) {
                var target = currentTargets.Where(target => enemy.GetPriority() > target.Enemy.GetPriority()).GetFarthestTarget(transform.position);
                target.Enemy = enemy;
                target.ConsecutiveHits = 0f;
            }
        }
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
        foreach (Enemy enemy in enemiesInRange.Except(currentTargets.Select(target => target.Enemy))) {
            if (currentTargets.Any(target => enemy.GetPriority() < target.Enemy.GetPriority())) {
                var target = currentTargets.Where(target => enemy.GetPriority() < target.Enemy.GetPriority()).GetFarthestTarget(transform.position);
                target.Enemy = enemy;
                target.ConsecutiveHits = 0f;
            }
        }
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
        var farthestTarget = currentTargets.GetFarthestTarget(transform.position);
        foreach (Enemy enemy in enemiesInRange.Except(currentTargets.Select(target => target.Enemy))) {
            if (GetCloserEnemy(farthestTarget.Enemy, enemy).Equals(enemy)) {
                farthestTarget.Enemy = enemy;
                farthestTarget.ConsecutiveHits = 0f;
            }
        }
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
        currentTargets.RemoveWhere(target => target.Enemy == null || target.Enemy.gameObject == null);
    }

    private void PurgeOutOfRangeEnemies() {
        currentTargets.RemoveWhere(target => !enemiesInRange.Contains(target.Enemy));
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
            enemy.Damage(towerType.damagePerShot * GetConsecutiveMultiplier(consecutiveHits));
            enemy.CheckForTriggerEffects(towerType.damageType);

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
    private float GetConsecutiveMultiplier(float consecutiveHits) {
        if (towerType.rampUpShotsNeeded == 0) {
            return 1;
        }

        var rampUpCount = towerType.rampUpShotsNeeded <= consecutiveHits ? towerType.rampUpShotsNeeded : consecutiveHits;

        return towerType.rampUpMultiplier / towerType.rampUpShotsNeeded * rampUpCount;
    }

    private void OnPositionHit(Vector3 position) {
        if (towerType.areaEffectPrefab != null) {
            Instantiate(towerType.areaEffectPrefab, position, Quaternion.identity);
        }
    }
}
