using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;
<<<<<<< HEAD

    private InputSystem_Actions playerControls;
    private Animator anim;

    private bool CanLook()
    {
        // Allow camera/look if not spawned (singleplayer) or if this is the owner in multiplayer
        return !IsSpawned || IsOwner;
    }
    
    public override void OnNetworkSpawn()
    {
        playerControls = new InputSystem_Actions(); 
        anim = this.GetComponent<Animator>(); 

        if (CanLook())
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
        if (!CanLook())
        {
            return;
        }
=======
    public void ProcessLook(Vector2 input)
    {
>>>>>>> main
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