using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

// Concrete TestItem class for testing (since Item is abstract)
public class TestItem : Item {
    protected override void useItem() {
        // Stub for testing
    }
}

public class InventoryTests {
    private GameObject inventoryObj;
    private Inventory inventory;
    private PowerManager powerManager;

    // Helper to create a test item
    private Item CreateItem(string name, Sprite icon = null) {
        GameObject go = new GameObject(name);
        var item = go.AddComponent<TestItem>();
        item.itemName = name;
        item.icon = icon;
        return item;
    }

    // Helper to create UI slot icons
    private GameObject CreateSlotIcon() {
        GameObject slot = new GameObject("Slot");
        slot.AddComponent<UnityEngine.UI.Image>();
        slot.SetActive(false);
        return slot;
    }

    [UnitySetUp]
    public IEnumerator Setup() {
        // Reset Inventory singleton
        Inventory.instance = null;

        // Inventory GameObject
        inventoryObj = new GameObject("Inventory");
        inventory = inventoryObj.AddComponent<Inventory>();

        // PowerManager (disable Update to avoid NullReferenceException)
        powerManager = inventoryObj.AddComponent<PowerManager>();
        powerManager.current = 0f;
        powerManager.enabled = false;

        // Add 5 UI slot icons
        GameObject[] slots = new GameObject[5];
        for (int i = 0; i < 5; i++)
            slots[i] = CreateSlotIcon();

        typeof(Inventory)
            .GetField("slotIcons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(inventory, slots);

        // Add dropPoint
        GameObject dropPoint = new GameObject("DropPoint");
        dropPoint.transform.position = Vector3.zero;
        typeof(Inventory)
            .GetField("dropPoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(inventory, dropPoint.transform);

        yield return null; // Wait for Awake/Start
    }

    [UnityTearDown]
    public IEnumerator Teardown() {
        // Reset singleton
        Inventory.instance = null;

        // Destroy all GameObjects in scene
        var all = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var g in all)
            Object.DestroyImmediate(g);

        yield return null; // Wait for destruction
    }

    [UnityTest]
    public IEnumerator AddItem() {
        var item = CreateItem("Sword");
        inventory.AddItem(item);

        yield return null;

        Assert.AreEqual(1, GetPrivateItems().Count);
        Assert.IsTrue(GetSlot(0).activeSelf);
    }

    [UnityTest]
    public IEnumerator ConsumeBattery() {
        var item = CreateItem("Potion");
        inventory.AddItem(item);
        yield return null;

        inventory.UseItem();
        yield return null;

        Assert.AreEqual(0, GetPrivateItems().Count);
        Assert.AreEqual(20, powerManager.current);
    }

    [UnityTest]
    public IEnumerator ConsumeBanish() {
        var item = CreateItem("Banish");
        inventory.AddItem(item);
        yield return null;

        bool used = inventory.UseItem_Banish();
        yield return null;

        Assert.IsTrue(used);
        Assert.AreEqual(0, GetPrivateItems().Count);
    }

    [UnityTest]
    public IEnumerator DropItem() {
        var itemGO = CreateItem("Rock");
        inventory.AddItem(itemGO);
        yield return null;

        // Set dropPoint position
        var dp = (Transform)typeof(Inventory)
            .GetField("dropPoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(inventory);
        dp.position = new Vector3(1f, 2f, 3f);

        inventory.DropItem();
        yield return null;

        Assert.AreEqual(0, GetPrivateItems().Count);
        Assert.IsTrue(itemGO.gameObject.activeSelf);
        Assert.AreEqual(dp.position, itemGO.transform.position);
    }

    [UnityTest]
    public IEnumerator isBanishCheck() {
        inventory.AddItem(CreateItem("Rock"));
        inventory.AddItem(CreateItem("Banish"));
        yield return null;

        inventory.scrollUp();
        yield return null;

        Assert.IsTrue(inventory.isBanish());
    }

    [UnityTest]
    public IEnumerator ScrollUp_ChangesSelectedItem() {
        inventory.AddItem(CreateItem("Item1"));
        inventory.AddItem(CreateItem("Item2"));
        yield return null;

        int initialSelected = GetPrivateSelectedItem();

        inventory.scrollUp();
        yield return null;

        int newSelected = GetPrivateSelectedItem();
        Assert.AreNotEqual(initialSelected, newSelected, "Selected item should change");
    }

    // ------- Helpers to read private fields -------
    private List<Item> GetPrivateItems() {
        return (List<Item>)typeof(Inventory)
            .GetField("items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(inventory);
    }

    private int GetPrivateSelectedItem() {
        return (int)typeof(Inventory)
            .GetField("selectedItem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(inventory);
    }

    private GameObject GetSlot(int index) {
        var arr = (GameObject[])typeof(Inventory)
            .GetField("slotIcons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(inventory);
        return arr[index];
    }
}
