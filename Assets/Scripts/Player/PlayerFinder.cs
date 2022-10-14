using UnityEngine;

public class PlayerFinder : MonoBehaviour
{
    public static PlayerFinder Instance { get; private set; }

    public GameObject Player { get; private set; }

    void Start() {
        MonoBehaviour playerScript = FindObjectOfType<ThirdPersonMovement>();
        if (playerScript == null) {
            playerScript = FindObjectOfType<CharacterMovementScript>();
        }
        if (playerScript != null) {
            Player = playerScript.gameObject;
        }
    }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }
}
