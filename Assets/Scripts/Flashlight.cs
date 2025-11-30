using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    public bool maxLightToggle;
    public bool normLightToggle;
    public float viewDistance;
    private PowerManager powerManager;
    public Light lightSource;
    private void Start()
    {
        viewDistance = 5f;
        normLightToggle = false;
        maxLightToggle = false;
        powerManager = GetComponent<PowerManager>();
        lightSource = GetComponentInChildren<Light>();
        if(lightSource != null)
        {
            lightSource.enabled = true;
            lightSource.range = 5f;
        }
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
        else if(!normLightToggle && !maxLightToggle)
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
        if(lightSource != null)
        {
            lightSource.range = viewDistance;
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
