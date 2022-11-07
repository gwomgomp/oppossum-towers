using UnityEngine;

public class PlayerFinder : MonoBehaviour {
    public static PlayerFinder Instance { get; private set; }

    public GameObject Player {
        get { return FindPlayer(); }
        private set {
            playerGameObject = value;
        }
    }

    private GameObject playerGameObject;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    private GameObject FindPlayer() {
        if (playerGameObject == null) {
            MonoBehaviour playerScript = FindObjectOfType<ThirdPersonMovement>();
            if (playerScript == null) {
                playerScript = FindObjectOfType<CharacterMovementScript>();
            }
            if (playerScript != null) {
                playerGameObject = playerScript.gameObject;
            }
        }
        return playerGameObject;
    }
}
