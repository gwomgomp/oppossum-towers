using UnityEngine;

public class EnemyBody : MonoBehaviour
{
    private Enemy enemy;

    private void Start() {
        enemy = this.RequireComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out LaneCheckpoint next)) {
            enemy.HandleCheckpoint(next);
            return;
        }

        if (collider.TryGetComponentInParentForTag(TagConstants.LOOT, out Loot loot)) {
            enemy.HandleLoot(loot);
            return;
        }
    }
}
