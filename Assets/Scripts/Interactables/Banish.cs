using UnityEngine;
public class Banish : Item
{
    private string color;
    private void Start()
    {
        itemName = "Banish";
        itemType = "Consumable";
        durability = 1;
        color = "grey";
    }
    protected override void useItem()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Ghost");
        if(temp.Length > 0)
        {
            Destroy(temp[0]);
        }
        Debug.Log("Used Banish with color: " + color);
    }
}