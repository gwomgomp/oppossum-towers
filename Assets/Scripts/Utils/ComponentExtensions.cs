using UnityEngine;

public static class ComponentExtensions {
    public static bool TryGetComponentInParent<T>(this Component component, out T result) {
        result = component.GetComponentInParent<T>();
        if (result == null) {
            return false;
        } else {
            return true;
        }
    }

    public static bool TryGetComponentInParentForTag<T>(this Component component, string tag, out T result) {
        if (component.CompareTag(tag)) {
            bool success = component.TryGetComponentInParent(out result);
            if (!success) {
                Debug.Log($"Game object tagged as ${tag}, but no ${result.GetType().Name} script found.");
            }
            return success;
        } else {
            result = default;
            return false;
        }
    }

    public static T RequireComponent<T>(this Component component) {
        T result = component.GetComponent<T>();
        if (result == null) {
            Debug.LogError($"Could not find component of type ${typeof(T).Name}");
            return default;
        } else {
            return result;
        }
    }

    public static T RequireComponentInParent<T>(this Component component, bool includeInactive = false) {
        T result = component.GetComponentInParent<T>(includeInactive);
        if (result == null) {
            Debug.LogError($"Could not find component of type ${typeof(T).Name} in parent");
            return default;
        } else {
            return result;
        }
    }

    public static T RequireComponentInChildren<T>(this Component component, bool includeInactive = false) {
        T result = component.GetComponentInChildren<T>(includeInactive);
        if (result == null) {
            Debug.LogError($"Could not find component of type ${typeof(T).Name} in children");
            return default;
        } else {
            return result;
        }
    }
}
