using UnityEngine;

public static class GameObjectExtensions {
    public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T result) {
        result = gameObject.GetComponentInParent<T>();
        if (result == null) {
            return false;
        } else {
            return true;
        }
    }

    public static bool TryGetComponentInParentForTag<T>(this GameObject gameObject, string tag, out T result) {
        if (gameObject.CompareTag(tag)) {
            bool success = gameObject.TryGetComponentInParent(out result);
            if (!success) {
                Debug.Log($"Game object tagged as ${tag}, but no ${result.GetType().Name} script found.");
            }
            return success;
        } else {
            result = default;
            return false;
        }
    }

    public static T RequireComponent<T>(this GameObject gameObject) {
        T result = gameObject.GetComponent<T>();
        if (result == null) {
            Debug.LogError($"Could not find component of type ${typeof(T).Name}");
            return default;
        } else {
            return result;
        }
    }

    public static T RequireComponentInParent<T>(this GameObject gameObject, bool includeInactive = false) {
        T result = gameObject.GetComponentInParent<T>(includeInactive);
        if (result == null) {
            Debug.LogError($"Could not find component of type ${typeof(T).Name} in parent");
            return default;
        } else {
            return result;
        }
    }

    public static T RequireComponentInChildren<T>(this GameObject gameObject, bool includeInactive = false) {
        T result = gameObject.GetComponentInChildren<T>(includeInactive);
        if (result == null) {
            Debug.LogError($"Could not find component of type ${typeof(T).Name} in children");
            return default;
        } else {
            return result;
        }
    }
}
