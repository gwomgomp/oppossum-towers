using System;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float health;

    private const float rotationSpeed = 5f;
    private const float moveDampening = 5f;

    private bool initialized = false;
    private EnemyType type = null;
    private LaneCheckpoint currentTarget = null;

    private Vector3 velocity = Vector3.zero;

    public void Initialize(EnemyType type, LaneCheckpoint startingCheckpoint) {
        if (!initialized) {
            this.type = type;
            health = type.MaxHealth;
            currentTarget = startingCheckpoint;
            transform.rotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
            initialized = true;
        } else {
            Debug.LogError("Do not try to initialize enemy twice");
        }
    }

    public void Update() {
        if (initialized && currentTarget != null) {
            Vector3 newPosition =  Vector3.SmoothDamp(
                transform.position,
                currentTarget.transform.position,
                ref velocity,
                calculateExpectedTravelTime(),
                type.Speed
            );
            transform.position = newPosition;
            
            Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if (initialized && collider.gameObject.TryGetComponent(out LaneCheckpoint next) && next == currentTarget) {
            currentTarget = next.NextCheckpoint;
        }
    }

    private float calculateExpectedTravelTime() {
        if (currentTarget != null) {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            return distance / type.Speed / moveDampening;
        } else {
            return 0f;
        }
    }
}
