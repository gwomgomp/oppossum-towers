using UnityEngine;

public static class PlayerIgnoreCollisionHelper {
    public static void IgnorePlayerCollision(GameObject ignoredCollision) {
        GameObject player = PlayerFinder.Instance.Player;
        if (player != null && player.TryGetComponent(out Collider playerCollider)
            && ignoredCollision != null && ignoredCollision.TryGetComponent(out Collider ignoredCollider)) {
            Physics.IgnoreCollision(playerCollider, ignoredCollider);
        }
    }
}
