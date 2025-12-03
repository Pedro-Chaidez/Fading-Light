using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DamageTests {
    [UnityTest]
    public IEnumerator Damage_OnTriggerStay() {
        // Arrange
        var ghostObject = new GameObject();
        var ghostDamage = ghostObject.AddComponent<GhostDamage>();
        ghostDamage.damagePerSecond = 10f;

        var ghostCollider = ghostObject.AddComponent<BoxCollider>();
        ghostCollider.isTrigger = true;

        var playerObject = new GameObject();
        var playerHealth = playerObject.AddComponent<PlayerHealth>();

        var healthBarObject = new GameObject();
        playerHealth.healthBar = healthBarObject.AddComponent<UnityEngine.UI.Image>();

        var playerCollider = playerObject.AddComponent<BoxCollider>();
        playerObject.AddComponent<Rigidbody>();

        yield return null; // Wait for Start()

        float initialHealth = playerHealth.currentHealth;

        // Act
        playerObject.transform.position = ghostObject.transform.position;

        // Ensure the trigger event fires
        yield return new WaitForFixedUpdate();

        // Wait enough time for damage to apply
        yield return new WaitForSeconds(1f);

        // Assert - should drop by EXACTLY 10
        Assert.AreEqual(initialHealth - 10f, playerHealth.currentHealth, 0.1f);

        // Cleanup
        Object.Destroy(ghostObject);
        Object.Destroy(playerObject);
        Object.Destroy(healthBarObject);
    }


    [UnityTest]
    public IEnumerator Damage_OncePerSecond() {
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
    public IEnumerator Damage_DoesNotDamageNonPlayerObjects() {
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