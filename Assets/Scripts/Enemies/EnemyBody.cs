using UnityEngine;

public class EnemyBody : MonoBehaviour
{
    private Enemy enemy;

    private void Start() {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out LaneCheckpoint next)) {
            enemy.HandleCheckpoint(next);
            return;
        }

        if (collider.CompareTag(TagConstants.LOOT)) {
            enemy.HandleLoot(collider.GetComponentInParent<Loot>());
            return;
        }
    }
}
