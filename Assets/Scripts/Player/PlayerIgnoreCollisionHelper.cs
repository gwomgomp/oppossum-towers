using UnityEngine;

public static class PlayerIgnoreCollisionHelper {
    public static void IgnorePlayerCollision(GameObject ignoredCollision) {
        GameObject player = PlayerFinder.Instance.Player;
        if (player.TryGetComponent(out Collider playerCollider) && ignoredCollision.TryGetComponent(out Collider ignoredCollider)) {
            Physics.IgnoreCollision(playerCollider, ignoredCollider);
        }
    }
}
