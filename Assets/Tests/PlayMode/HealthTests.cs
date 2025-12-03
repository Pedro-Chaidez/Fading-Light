using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class PlayerHealthTests {
    [UnityTest]
    public IEnumerator Player_StartsWithFullHealth() {
        // Create GameObject and attach PlayerHealth
        var playerObject = new GameObject();
        var playerHealth = playerObject.AddComponent<PlayerHealth>();

        // Create a dummy health bar Image (required by the script)
        var healthBarObject = new GameObject();
        playerHealth.healthBar = healthBarObject.AddComponent<UnityEngine.UI.Image>();

        // Wait one frame for Start() to be called
        yield return null;

        // Test that current health equals max health (100)
        Assert.AreEqual(100f, playerHealth.currentHealth);
        Assert.AreEqual(playerHealth.maxHealth, playerHealth.currentHealth);

        // Clean up
        Object.Destroy(playerObject);
        Object.Destroy(healthBarObject);
    }

    [UnityTest]
    public IEnumerator Player_TakeDamage() {
        var playerObject = new GameObject();
        var playerHealth = playerObject.AddComponent<PlayerHealth>();
        var healthBarObject = new GameObject();
        playerHealth.healthBar = healthBarObject.AddComponent<UnityEngine.UI.Image>();

        yield return null;

        playerHealth.TakeDamage(20f);
        Assert.AreEqual(80f, playerHealth.currentHealth);
        
        Object.Destroy(playerObject);
        Object.Destroy(healthBarObject);
    }

    [UnityTest]
    public IEnumerator Player_LethalDamage() {
        var playerObject = new GameObject();
        var playerHealth = playerObject.AddComponent<PlayerHealth>();

        // Add mock components instead of real ones
        playerObject.AddComponent<MockInputManager>();
        playerObject.AddComponent<MockStamina>();

        var healthBarObject = new GameObject();
        playerHealth.healthBar = healthBarObject.AddComponent<UnityEngine.UI.Image>();

        yield return null;

        playerHealth.currentHealth = 10f;
        playerHealth.TakeDamage(20f);

        Assert.AreEqual(0f, playerHealth.currentHealth);

        Object.Destroy(playerObject);
        Object.Destroy(healthBarObject);
    }

    [UnityTest]
    public IEnumerator Player_Heal() {
        var playerObject = new GameObject();
        var playerHealth = playerObject.AddComponent<PlayerHealth>();
        var healthBarObject = new GameObject();
        playerHealth.healthBar = healthBarObject.AddComponent<UnityEngine.UI.Image>();

        yield return null;

        playerHealth.currentHealth = 50f;
        playerHealth.Heal(20f);
        Assert.AreEqual(70f, playerHealth.currentHealth);

        Object.Destroy(playerObject);
        Object.Destroy(healthBarObject);
    }

    [UnityTest]
    public IEnumerator Player_Overheal() {
        var playerObject = new GameObject();
        var playerHealth = playerObject.AddComponent<PlayerHealth>();
        var healthBarObject = new GameObject();
        playerHealth.healthBar = healthBarObject.AddComponent<UnityEngine.UI.Image>();

        yield return null;

        playerHealth.currentHealth = 95f;
        playerHealth.Heal(20f);
        Assert.AreEqual(100f, playerHealth.currentHealth);

        Object.Destroy(playerObject);
        Object.Destroy(healthBarObject);
    }

    [UnityTest]
    public IEnumerator Player_Dies() {
        var playerObject = new GameObject();
        var playerHealth = playerObject.AddComponent<PlayerHealth>();

        // Create died screen
        var diedScreen = new GameObject();
        diedScreen.SetActive(false);
        playerHealth.diedScreen = diedScreen;

        var healthBarObject = new GameObject();
        playerHealth.healthBar = healthBarObject.AddComponent<UnityEngine.UI.Image>();

        yield return null;

        // Deal lethal damage
        playerHealth.TakeDamage(100f);

        // Assert
        Assert.AreEqual(0f, playerHealth.currentHealth);
        Assert.IsTrue(diedScreen.activeSelf, "Died screen should be active");

        Object.Destroy(playerObject);
        Object.Destroy(healthBarObject);
        Object.Destroy(diedScreen);
    }

    [UnityTest]
    public IEnumerator Player_Resurrects() {
        var playerObject = new GameObject();
        var playerHealth = playerObject.AddComponent<PlayerHealth>();

        // Create mock objects (NOT components!)
        var mockStamina = new MockStaminaComponent {
            current = 0f,
            max = 100f,
            enabled = false
        };

        var mockMovement = new MockPlayerMotorComponent {
            sprinting = true,
            speed = 10f
        };

        // INJECT the dependencies BEFORE yield return null (before Start() runs)
        playerHealth.Initialize(mockStamina, mockMovement);

        var diedScreen = new GameObject();
        diedScreen.SetActive(true);
        playerHealth.diedScreen = diedScreen;

        var healthBarObject = new GameObject();
        playerHealth.healthBar = healthBarObject.AddComponent<UnityEngine.UI.Image>();

        yield return null; // Start() runs, but uses injected dependencies

        playerHealth.currentHealth = 0f;
        playerHealth.Resurrect();

        // Assert
        Assert.AreEqual(100f, playerHealth.currentHealth);
        Assert.AreEqual(100f, mockStamina.current, "Stamina should be restored");
        Assert.IsTrue(mockStamina.enabled, "Stamina should be enabled");
        Assert.IsFalse(mockMovement.sprinting, "Should not be sprinting");
        Assert.AreEqual(6f, mockMovement.speed, "Speed should be reset to 6");
        Assert.IsFalse(diedScreen.activeSelf);

        Object.Destroy(playerObject);
        Object.Destroy(healthBarObject);
        Object.Destroy(diedScreen);
    }

    // Mock classes (plain C# objects, NOT MonoBehaviours)
    public class MockStaminaComponent : IStamina {
        public float current { get; set; }
        public float max { get; set; }
        public bool enabled { get; set; }
    }

    public class MockPlayerMotorComponent : IPlayerMotor {
        public bool sprinting { get; set; }
        public float speed { get; set; }
    }
}

public class MockInputManager : MonoBehaviour {
    // Empty mock - does nothing, won't throw errors
}

public class MockStamina : MonoBehaviour {
    public float current = 100f;
    public float max = 100f;
    // No Start() or Update() methods that would cause issues
}