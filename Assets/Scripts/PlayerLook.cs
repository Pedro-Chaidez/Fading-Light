using UnityEngine;
using Unity.Netcode;

public class PlayerLook : NetworkBehaviour
{
    public Camera cam;
    private float xRotation = 0f;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    private InputSystem_Actions playerControls;
    private Animator anim;
    
    public override void OnNetworkSpawn()
    {
        playerControls = new InputSystem_Actions(); 
        anim = this.GetComponent<Animator>(); 

        if (IsOwner)
        {
            if (Camera.main != null)
            {
                Camera.main.gameObject.SetActive(false);
            }

            cam.gameObject.SetActive(true); 
           
        }
        else // This is not the owner
        {
            cam.gameObject.SetActive(false); 
        }
    }
    
    public void ProcessLook(Vector2 input)
    {
        if (!IsOwner)
        {
            return;
        }
        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
    }
    private void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
}