using UnityEngine;
public class FriendResurrection : Item
{
    private void Start()
    {
        itemName = "FriendResurrection";
        itemType = "Revive";
        durability = 1;
    }
    protected override void useItem()
    {

    }
}