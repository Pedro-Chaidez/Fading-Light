using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float distance = 3f;
    [SerializeField]
    private LayerMask mask;
    private PlayerUI playerUI;
    private InputManager inputManager;
    private void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
    }
    private void Update()
    {
        playerUI.UpdateText(string.Empty);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            Debug.Log("Raycast hit: " + hitInfo.collider.gameObject.name);
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if(interactable != null) {
                Debug.Log("Interactable found: " + interactable.name);
                playerUI.UpdateText(interactable.promptMessage);
                if(inputManager.onFoot.Interact.triggered)
                {
                    Debug.Log("Interact key pressed! Calling Interact...");
                    interactable.BaseInteract();
                }
            }
        }
    }

}
