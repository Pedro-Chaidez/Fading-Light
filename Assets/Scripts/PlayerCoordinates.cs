using Unity.VisualScripting;
using UnityEngine;

public class PlayerCoordinates : MonoBehaviour {
    private CharacterController controller;
    private Vector3 position;
    private Quaternion rotation;
    void Start() {
        controller = GetComponent<CharacterController>();
        position = controller.transform.position;
        rotation = controller.transform.rotation;
    }

    public Vector3 GetPosition() {
        return position;
    }

    public Quaternion GetRotation() {
        return rotation;
    }

}
