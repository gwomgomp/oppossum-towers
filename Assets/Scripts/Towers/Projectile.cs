using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour {
    private bool trackObject = true;
    private Vector3 targetPosition = Vector3.zero;
    private GameObject targetObject = null;

    private float speed = 10.0f;

    private EnemyHitEvent enemyHitEvent = null;
    private PositionHitEvent positionHitEvent = null;

    void Update() {
    float step = speed * Time.deltaTime;

    if (trackObject) {
        if (targetObject != null) {
            targetPosition = targetObject.transform.position;
        } else {
            trackObject = false;
        }
    }

    transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

    if (Vector3.Distance(transform.position, targetPosition) < 0.5f) {
        if (trackObject && enemyHitEvent != null) {
            enemyHitEvent.Invoke(targetObject);
        } else if (positionHitEvent != null) {
            positionHitEvent.Invoke(targetPosition);
        }

        Destroy(gameObject);
    }

    Vector3 lookDirection = targetPosition - transform.position;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    public void SetTargetPosition(Vector3 targetPosition) {
        this.targetPosition = targetPosition;
        trackObject = false;
    }

    public void SetTargetObject(GameObject targetObject) {
        this.targetObject = targetObject;
        trackObject = true;
    }

    public void SetSpeed(float speed) {
        this.speed = speed;
    }

    public void SetEnemyHitEvent(EnemyHitEvent enemyHitEvent) {
        this.enemyHitEvent = enemyHitEvent;
    }

    public void SetPositionHitEvent(PositionHitEvent positionHitEvent) {
        this.positionHitEvent = positionHitEvent;
    }
}
