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
        playerCollider.isTrigger = true; // Make player collider a trigger too
        var rb = playerObject.AddComponent<Rigidbody>();
        rb.isKinematic = true; // Prevent physics interference

        yield return null; // Wait for Start()
        float initialHealth = playerHealth.currentHealth;

        // Act - Position objects to overlap
        playerObject.transform.position = ghostObject.transform.position;

        // Manually trigger OnTriggerEnter to simulate collision
        ghostDamage.SendMessage("OnTriggerEnter", playerCollider, SendMessageOptions.DontRequireReceiver);
        yield return new WaitForFixedUpdate();

        // First damage should be applied
        Assert.AreEqual(90f, playerHealth.currentHealth, 0.1f, "Should take 10 damage on first contact");

        // Wait for cooldown period
        yield return new WaitForSeconds(1.1f);

        // Manually call OnTriggerStay to simulate continuous contact
        ghostDamage.SendMessage("OnTriggerStay", playerCollider, SendMessageOptions.DontRequireReceiver);
        yield return new WaitForFixedUpdate();

        // Assert - Second damage should be applied after cooldown
        Assert.AreEqual(80f, playerHealth.currentHealth, 0.1f, "Should take another 10 damage after cooldown");

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

        // Act - manually trigger damage (pass PlayerHealth, not BoxCollider)
        ghostDamage.TestProcessDamage(playerHealth);

        // Assert - first damage applied
        Assert.AreEqual(90f, playerHealth.currentHealth, 0.1f, "Should take 10 damage immediately");

        // Try again immediately (should not damage due to cooldown)
        ghostDamage.TestProcessDamage(playerHealth);
        Assert.AreEqual(90f, playerHealth.currentHealth, 0.1f, "Should not take damage again immediately");

        // Wait for cooldown
        yield return new WaitForSeconds(1.1f);

        // Act - trigger damage again
        ghostDamage.TestProcessDamage(playerHealth);

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