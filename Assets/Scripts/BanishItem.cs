using UnityEngine;

public class BanishItem : Interactable {
    protected override void Interact() {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Ghost");
        if (temp.Length > 0) {
            Destroy(temp[0]);
        }
    }

    // Add this test helper method
#if UNITY_EDITOR
    public void TestInteract() {
        Interact();
    }
#endif
}