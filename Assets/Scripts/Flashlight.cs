using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    public bool maxLightToggle;
    public bool normLightToggle;
    public float viewDistance;
    private PowerManager powerManager;
    private void Start()
    {
        viewDistance = 5f;
        normLightToggle = false;
        maxLightToggle = false;
        powerManager = GetComponent<PowerManager>();
    }

    private void Update()
    {
        if (powerManager != null && powerManager.current <= 0) {
            viewDistance = 5f;
            if (normLightToggle) {
                normLight();
            }
            if (maxLightToggle) {
                maxLight();
            }
            return;
        }
        if(!normLightToggle && !maxLightToggle)
        {
            viewDistance = 5f;
        }
        else if(normLightToggle && !maxLightToggle)
        {
            viewDistance = 10f;
        }
        else
        {
            viewDistance = 15f;
        }
    }

    public void normLight()
    {
        normLightToggle = !normLightToggle;
    }

    public void maxLight()
    {
        maxLightToggle = !maxLightToggle;
    }
}
