using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    public bool maxLightToggle;
    public bool normLightToggle;
    public float viewDistance;
    private void Start()
    {
        viewDistance = 5f;
        normLightToggle = false;
        maxLightToggle = false;
    }

    private void Update()
    {
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
