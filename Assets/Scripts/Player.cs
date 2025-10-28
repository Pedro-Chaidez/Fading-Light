using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 1;
    public int stamina = 100;
    private Inventory inventory;
    private Item itemInHand;
    public bool flashlightIsOn;
    public void PickUpItem()
    {
        
    }
    public void UseItem()
    {

    }
    public void Die()
    {
        //activates the death screen
    }
    public void FlashlightToggle()
    {
        flashlightIsOn = !flashlightIsOn;
    }
}
