using UnityEngine;

public class BanishItem : Interactable {
    protected override void Interact() {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Ghost");
        if (temp.Length > 0) {
            Destroy(temp[0]);
        }
    }
    public void TestInteract() {
        Interact();
    }
}