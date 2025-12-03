using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BanishTests
{
    [UnityTest]
    public IEnumerator BanishItem() {
        // Arrange
        var banishItem = new GameObject().AddComponent<BanishItem>();
        var ghost1 = new GameObject();
        ghost1.tag = "Ghost";
        var ghost2 = new GameObject();
        ghost2.tag = "Ghost";

        yield return null;

        // Act
        banishItem.TestInteract(); // You'll need to make Interact() public or call it via reflection

        yield return null; // Wait for Destroy to process

        // Assert
        var remainingGhosts = GameObject.FindGameObjectsWithTag("Ghost");
        Assert.AreEqual(1, remainingGhosts.Length, "Should have 1 ghost remaining");
        Assert.AreEqual(ghost2, remainingGhosts[0], "Second ghost should remain");

        // Cleanup
        Object.Destroy(banishItem.gameObject);
        Object.Destroy(ghost2);
    }

    [UnityTest]
    public IEnumerator BanishNothing() {
        // Arrange
        var banishItem = new GameObject().AddComponent<BanishItem>();

        yield return null;

        // Act - should not crash
        banishItem.TestInteract();

        yield return null;

        // Assert - just checking it doesn't crash
        Assert.IsTrue(true);

        // Cleanup
        Object.Destroy(banishItem.gameObject);
    }
}
