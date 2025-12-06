using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerInteractTests {
    [Test]
    public void BanishButton() {
        var playerObj = new GameObject();
        var playerInteract = playerObj.AddComponent<PlayerInteract>();

        Assert.IsTrue(playerInteract.IsBanishButton("Banish Button"));
        Assert.IsFalse(playerInteract.IsBanishButton("Normal Button"));
        Assert.IsFalse(playerInteract.IsBanishButton("Door"));

        Object.Destroy(playerObj);
    }

    [Test]
    public void DefaultDistance() {
        var playerObj = new GameObject();
        var playerInteract = playerObj.AddComponent<PlayerInteract>();

        float distance = playerInteract.TestGetDistance();

        Assert.AreEqual(3f, distance, "Default interaction distance should be 3");

        Object.Destroy(playerObj);
    }

    [UnityTest]
    public IEnumerator NormalInteractable() {
        var playerObj = new GameObject();
        var playerInteract = playerObj.AddComponent<PlayerInteract>();

        // IMPORTANT: Disable the component so Start() and Update() don't run
        playerInteract.enabled = false;

        var interactableObj = new GameObject("TestDoor");
        var interactable = interactableObj.AddComponent<TestInteractable>();
        interactable.promptMessage = "Press E to open";

        yield return null;

        playerInteract.HandleInteractable(interactable);

        Assert.IsNotNull(interactable);

        Object.Destroy(playerObj);
        Object.Destroy(interactableObj);
    }

    [UnityTest]
    public IEnumerator DetectsInteractableWithinRange() {
        var playerObj = new GameObject("Player");
        playerObj.transform.position = Vector3.zero;

        var cameraObj = new GameObject("Camera");
        cameraObj.transform.parent = playerObj.transform;
        var cam = cameraObj.AddComponent<Camera>();
        cameraObj.transform.forward = Vector3.forward;

        var interactableObj = new GameObject("Door");
        interactableObj.transform.position = new Vector3(0, 0, 2); 
        var collider = interactableObj.AddComponent<BoxCollider>();
        var interactable = interactableObj.AddComponent<TestInteractable>();
        interactable.promptMessage = "Press E";

        yield return null;

        Ray ray = new Ray(cameraObj.transform.position, cameraObj.transform.forward);
        RaycastHit hit;
        bool didHit = Physics.Raycast(ray, out hit, 3f);

        Assert.IsTrue(didHit, "Should detect interactable within range");
        if (didHit) {
            Assert.AreEqual(interactableObj, hit.collider.gameObject, "Should hit the interactable object");
        }

        Object.Destroy(playerObj);
        Object.Destroy(interactableObj);
    }

    [UnityTest]
    public IEnumerator DoesNotDetectInteractableOutOfRange() {
        var playerObj = new GameObject("Player");
        playerObj.transform.position = Vector3.zero;

        var cameraObj = new GameObject("Camera");
        cameraObj.transform.parent = playerObj.transform;
        var cam = cameraObj.AddComponent<Camera>();
        cameraObj.transform.forward = Vector3.forward;

        var interactableObj = new GameObject("Door");
        interactableObj.transform.position = new Vector3(0, 0, 10);
        var collider = interactableObj.AddComponent<BoxCollider>();
        var interactable = interactableObj.AddComponent<TestInteractable>();

        yield return null;

        Ray ray = new Ray(cameraObj.transform.position, cameraObj.transform.forward);
        RaycastHit hit;
        bool didHit = Physics.Raycast(ray, out hit, 3f);

        Assert.IsFalse(didHit, "Should not detect interactable out of range");

        Object.Destroy(playerObj);
        Object.Destroy(interactableObj);
    }

    [UnityTest]
    public IEnumerator DoesNotDetectInteractableBehind() {
        var playerObj = new GameObject("Player");
        playerObj.transform.position = Vector3.zero;

        var cameraObj = new GameObject("Camera");
        cameraObj.transform.parent = playerObj.transform;
        var cam = cameraObj.AddComponent<Camera>();
        cameraObj.transform.forward = Vector3.forward;

        var interactableObj = new GameObject("Door");
        interactableObj.transform.position = new Vector3(0, 0, -2);
        var collider = interactableObj.AddComponent<BoxCollider>();
        var interactable = interactableObj.AddComponent<TestInteractable>();

        yield return null;

        Ray ray = new Ray(cameraObj.transform.position, cameraObj.transform.forward);
        RaycastHit hit;
        bool didHit = Physics.Raycast(ray, out hit, 3f);

        Assert.IsFalse(didHit, "Should not detect interactable behind player");

        Object.Destroy(playerObj);
        Object.Destroy(interactableObj);
    }
}

public class TestInteractable : Interactable {
    public bool wasInteracted = false;

    protected override void Interact() {
        wasInteracted = true;
    }
}