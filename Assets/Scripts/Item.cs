using UnityEngine;

public abstract class Item : MonoBehaviour
{
    private string itemName;
    private string itemType;
    private float durability;
    protected virtual void CopyFrom(Item item)
    {
        this.name = item.name;
        this.itemType = item.itemType;
        this.durability = item.durability;
    }
}
public class Banish : Item
{
    private string color;

}
public class PortableBanish : Item
{

}
public class DamageProtection : Item
{

}
public class FriendResurrection : Item
{

}
public class SpeedBoost : Item
{

}
public class Battery : Item
{

}
