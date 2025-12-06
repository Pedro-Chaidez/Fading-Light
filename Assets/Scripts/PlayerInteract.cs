using UnityEngine;

public class PlayerInteract : MonoBehaviour {
    private Camera cam;
    [SerializeField]
    private float distance = 3f;
    [SerializeField]
    private LayerMask mask;
    private PlayerUI playerUI;
    private InputManager inputManager;

    private void Start() {
        var playerLook = GetComponent<PlayerLook>();
        if (playerLook != null) {
            cam = playerLook.cam;
        }
        playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
    }

    private void Update() {
        if (playerUI != null) {
            playerUI.UpdateText(string.Empty);
        }

        if (cam == null) return; // Can't raycast without camera

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance, mask)) {
            ProcessHit(hitInfo);
        }
    }

    // Extract for testing
    public void ProcessHit(RaycastHit hitInfo) {
        Debug.Log("Raycast hit: " + hitInfo.collider.gameObject.name);
        Interactable interactable = hitInfo.collider.GetComponent<Interactable>();

        if (interactable != null) {
            HandleInteractable(interactable);
        }
    }

    // Extract logic for testing
    public void HandleInteractable(Interactable interactable) {
        if (interactable.name != "Banish Button") {
            HandleNormalInteractable(interactable);
        } else {
            HandleBanishButton(interactable);
        }
    }

    private void HandleNormalInteractable(Interactable interactable) {
        Debug.Log("Interactable found: " + interactable.name);
        if (playerUI != null) {
            playerUI.UpdateText(interactable.promptMessage);
        }

        if (inputManager != null && inputManager.onFoot.Interact.triggered) {
            Debug.Log("Interact key pressed! Calling Interact...");
            interactable.BaseInteract();
        }
    }

    private void HandleBanishButton(Interactable interactable) {
        Inventory playerInventory = GetComponentInParent<Inventory>();
        Debug.Log("Interactable found: " + interactable.name);

        if (playerUI != null) {
            playerUI.UpdateText(interactable.promptMessage);
        }

        if (inputManager != null && inputManager.onFoot.Interact.triggered) {
            Debug.Log("Interact key pressed! Calling Interact...");
            if (playerInventory != null && playerInventory.isBanish() && playerInventory.UseItem_Banish()) {
                interactable.BaseInteract();
            }
        }
    }

    // For testing - check if object is banish button
    public bool IsBanishButton(string name) {
        return name == "Banish Button";
    }

#if UNITY_EDITOR
    // Test helper to set distance
    public void TestSetDistance(float dist) { distance = dist; }
    public float TestGetDistance() { return distance; }

    // Add this to disable Update in tests
    public void DisableUpdate() { enabled = false; }
#endif
}