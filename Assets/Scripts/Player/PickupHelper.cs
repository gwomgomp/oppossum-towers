using UnityEngine;

public class PickupHelper
{
    public static void Attach(MonoBehaviour target, Transform attachmentPoint) {
        Interactable interactable = target.RequireComponentInChildren<Interactable>();
        interactable.gameObject.SetActive(false);
        ManagerProvider.Instance.GetManager<InteractableManager>().StepOutOffRange(interactable);
        Collider[] colliders = target.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders) {
            collider.enabled = false;
        }
        target.transform.SetParent(attachmentPoint, true);
        target.transform.position = attachmentPoint.position;
        target.transform.Translate(Vector3.up * 2, Space.World);
    }

    public static void Detach(MonoBehaviour target) {
        Interactable interactable = target.RequireComponentInChildren<Interactable>(true);
        interactable.gameObject.SetActive(true);
        Collider[] colliders = target.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders) {
            collider.enabled = true;
        }
        target.transform.position = target.transform.parent.transform.position;
        target.transform.SetParent(target.transform.parent.parent, true);
    }
}
