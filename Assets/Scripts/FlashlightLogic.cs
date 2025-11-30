using UnityEngine;

public class FlashlightLogic : MonoBehaviour
{
    Light lightSource;
    Flashlight currLight;
    private void Start()
    {
        lightSource = GetComponentInChildren<Light>();
        currLight = GetComponent<Flashlight>();
    }

    private void Update()
    {
        lightSource.range = currLight.viewDistance;
    }
}
