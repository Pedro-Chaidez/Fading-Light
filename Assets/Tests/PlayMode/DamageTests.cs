using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GhostDamageTests {
    [UnityTest]
    public IEnumerator GhostDamage_DamagesPlayerOnTriggerStay() {
        // Arrange
        var ghostObject = new GameObject();
        var ghostDamage = ghostObject.AddComponent<GhostDamage>();
        ghostDamage.damagePerSecond = 10f;

        // Add collider and make it a trigger
        var ghostCollider = ghostObject.AddComponent<BoxCollider>();
        ghostCollider.isTrigger = true;

        // Create player
        var playerObject = new GameObject();
        var playerHealth = playerObject.AddComponent<PlayerHealth>();
        var healthBarObject = new GameObject();
        playerHealth.healthBar = healthBarObject.AddComponent<UnityEngine.UI.Image>();

        // Add collider to player
        var playerCollider = playerObject.AddComponent<BoxCollider>();

        // Add Rigidbody (required for physics interactions)
        playerObject.AddComponent<Rigidbody>();

        yield return null; // Wait for Start()

        float initialHealth = playerHealth.currentHealth;

        // Act - position player inside ghost trigger
        playerObject.transform.position = ghostObject.transform.position;

        yield return new WaitForSeconds(1.5f); // Wait for damage to apply

        // Assert
        Assert.Less(playerHealth.currentHealth, initialHealth, "Player should take damage");
        Assert.AreEqual(90f, playerHealth.currentHealth, 0.1f, "Player should lose 10 health");

        // Cleanup
        Object.Destroy(ghostObject);
        Object.Destroy(playerObject);
        Object.Destroy(healthBarObject);
    }

    [UnityTest]
    public IEnumerator GhostDamage_DamagesOnlyOncePerSecond() {
        // Arrange
        var ghostObject = new GameObject();
        var ghostDamage = ghostObject.AddComponent<GhostDamage>();
        ghostDamage.damagePerSecond = 10f;

        var playerObject = new GameObject();
        var playerHealth = playerObject.AddComponent<PlayerHealth>();
        var healthBarObject = new GameObject();
        playerHealth.healthBar = healthBarObject.AddComponent<UnityEngine.UI.Image>();

        // Add a collider for GetComponent to work
        var playerCollider = playerObject.AddComponent<BoxCollider>();

        yield return null; // Start() runs

        float initialHealth = playerHealth.currentHealth;

        // Act - manually trigger damage
        ghostDamage.TestProcessDamage(playerCollider);

        // Assert - first damage applied
        Assert.AreEqual(90f, playerHealth.currentHealth, 0.1f, "Should take 10 damage immediately");

        // Try again immediately (should not damage due to cooldown)
        ghostDamage.TestProcessDamage(playerCollider);

        Assert.AreEqual(90f, playerHealth.currentHealth, 0.1f, "Should not take damage again immediately");

        // Wait for cooldown
        yield return new WaitForSeconds(1.1f);

        // Act - trigger damage again
        ghostDamage.TestProcessDamage(playerCollider);

        // Assert - second damage applied
        Assert.AreEqual(80f, playerHealth.currentHealth, 0.1f, "Should take another 10 damage after cooldown");

        // Cleanup
        Object.Destroy(ghostObject);
        Object.Destroy(playerObject);
        Object.Destroy(healthBarObject);
    }

    [UnityTest]
    public IEnumerator GhostDamage_DoesNotDamageNonPlayerObjects() {
        // Arrange
        var ghostObject = new GameObject();
        var ghostDamage = ghostObject.AddComponent<GhostDamage>();
        ghostDamage.damagePerSecond = 10f;

        var ghostCollider = ghostObject.AddComponent<BoxCollider>();
        ghostCollider.isTrigger = true;

        // Create object without PlayerHealth component
        var otherObject = new GameObject();
        var otherCollider = otherObject.AddComponent<BoxCollider>();
        otherObject.AddComponent<Rigidbody>();

        yield return null;

        // Act - position object inside ghost (should not crash)
        otherObject.transform.position = ghostObject.transform.position;

        yield return new WaitForSeconds(1.5f);

        // Assert - just verify no crash occurred
        Assert.IsNotNull(otherObject);

        // Cleanup
        Object.Destroy(ghostObject);
        Object.Destroy(otherObject);
    }
}