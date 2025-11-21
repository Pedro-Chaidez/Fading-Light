using Unity.VisualScripting;
using UnityEngine;

public class PlayerCoordinates : MonoBehaviour {
    private CharacterController controller;
    private Vector3 initPosition;
    private Vector3 currPosition;
    private Quaternion rotation;
    void Start() {
        controller = GetComponent<CharacterController>();
        initPosition = controller.transform.position;
        currPosition = controller.transform.position;
        rotation = controller.transform.rotation;
    }

    void Update() {
        currPosition = controller.transform.position;
    }

    public Vector3 GetInitPosition() {
        return initPosition;
    }

    public Vector3 GetCurrPosition() {
        return currPosition;
    }
    public Quaternion GetRotation() {
        return rotation;
    }

}
