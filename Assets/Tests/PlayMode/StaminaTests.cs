using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class StaminaTests {
    [UnityTest]
    public IEnumerator InitializesCurrentToMax() {
        var playerObj = new GameObject();
        var stamina = playerObj.AddComponent<Stamina>();
        var coordinates = playerObj.AddComponent<PlayerCoordinates>();
        var motor = playerObj.AddComponent<PlayerMotor>();
        stamina.max = 100f;

        var barObj = new GameObject();
        var canvas = barObj.AddComponent<Canvas>();
        stamina.staminaBar = barObj.AddComponent<Image>();

        yield return null;

        Assert.AreEqual(100f, stamina.current, 0.01f, "Current should initialize to max");

        Object.Destroy(playerObj);
        Object.Destroy(barObj);
    }

    [UnityTest]
    public IEnumerator SprintingAndMoving() {
        var playerObj = new GameObject();
        var stamina = playerObj.AddComponent<Stamina>();
        var coordinates = playerObj.AddComponent<PlayerCoordinates>();
        var motor = playerObj.AddComponent<PlayerMotor>();
        stamina.max = 100f;
        stamina.drainSpeed = 20f;

        var barObj = new GameObject();
        var canvas = barObj.AddComponent<Canvas>();
        stamina.staminaBar = barObj.AddComponent<Image>();

        yield return null;

        float initialStamina = stamina.current;

        stamina.HandleStamina(true, true);

        Assert.Less(stamina.current, initialStamina, "Stamina should drain when sprinting and moving");

        Object.Destroy(playerObj);
        Object.Destroy(barObj);
    }

    [UnityTest]
    public IEnumerator SprintingButNotMoving() {
        var playerObj = new GameObject();
        var stamina = playerObj.AddComponent<Stamina>();
        var coordinates = playerObj.AddComponent<PlayerCoordinates>();
        var motor = playerObj.AddComponent<PlayerMotor>();
        stamina.max = 100f;
        stamina.drainSpeed = 20f;

        var barObj = new GameObject();
        var canvas = barObj.AddComponent<Canvas>();
        stamina.staminaBar = barObj.AddComponent<Image>();

        yield return null;

        float initialStamina = stamina.current;

        stamina.HandleStamina(true, false);

        Assert.AreEqual(initialStamina, stamina.current, 0.01f, "Stamina should not drain when not moving");

        Object.Destroy(playerObj);
        Object.Destroy(barObj);
    }

    [UnityTest]
    public IEnumerator MovingButNotSprinting() {
        var playerObj = new GameObject();
        var stamina = playerObj.AddComponent<Stamina>();
        var coordinates = playerObj.AddComponent<PlayerCoordinates>();
        var motor = playerObj.AddComponent<PlayerMotor>();
        stamina.max = 100f;
        stamina.drainSpeed = 20f;

        var barObj = new GameObject();
        var canvas = barObj.AddComponent<Canvas>();
        stamina.staminaBar = barObj.AddComponent<Image>();

        yield return null;

        float initialStamina = stamina.current;

        stamina.HandleStamina(false, true);

        Assert.AreEqual(initialStamina, stamina.current, 0.01f, "Stamina should not drain when not sprinting");

        Object.Destroy(playerObj);
        Object.Destroy(barObj);
    }

    [UnityTest]
    public IEnumerator RegeneratesAfterDelay() {
        var playerObj = new GameObject();
        var stamina = playerObj.AddComponent<Stamina>();
        var coordinates = playerObj.AddComponent<PlayerCoordinates>();
        var motor = playerObj.AddComponent<PlayerMotor>();
        stamina.max = 100f;
        stamina.current = 50f;
        stamina.regenSpeed = 15f;
        stamina.regenDelay = 1f;

        var barObj = new GameObject();
        var canvas = barObj.AddComponent<Canvas>();
        stamina.staminaBar = barObj.AddComponent<Image>();

        yield return null;

        stamina.current = 50f;

        yield return new WaitForSeconds(1.2f);

        stamina.HandleStamina(false, false);

        Assert.Greater(stamina.current, 50f, "Stamina should regenerate after delay");

        Object.Destroy(playerObj);
        Object.Destroy(barObj);
    }

    [UnityTest]
    public IEnumerator DoesNotRegenerateBeforeDelay() {
        var playerObj = new GameObject();
        var stamina = playerObj.AddComponent<Stamina>();
        var coordinates = playerObj.AddComponent<PlayerCoordinates>();
        var motor = playerObj.AddComponent<PlayerMotor>();
        stamina.max = 100f;
        stamina.regenSpeed = 15f;
        stamina.regenDelay = 2f;

        var barObj = new GameObject();
        var canvas = barObj.AddComponent<Canvas>();
        stamina.staminaBar = barObj.AddComponent<Image>();

        yield return null;

        stamina.current = 50f;

        stamina.HandleStamina(false, false);

        Assert.AreEqual(50f, stamina.current, 0.01f, "Stamina should not regenerate before delay");

        Object.Destroy(playerObj);
        Object.Destroy(barObj);
    }

    [UnityTest]
    public IEnumerator ClampsAtZero() {
        var playerObj = new GameObject();
        var stamina = playerObj.AddComponent<Stamina>();
        var coordinates = playerObj.AddComponent<PlayerCoordinates>();
        var motor = playerObj.AddComponent<PlayerMotor>();
        stamina.max = 100f;
        stamina.drainSpeed = 100f;

        var barObj = new GameObject();
        var canvas = barObj.AddComponent<Canvas>();
        stamina.staminaBar = barObj.AddComponent<Image>();

        yield return null;

        stamina.current = 10f;

        stamina.HandleStamina(true, true);
        stamina.HandleStamina(true, true);

        Assert.GreaterOrEqual(stamina.current, 0f, "Stamina should not go below 0");

        Object.Destroy(playerObj);
        Object.Destroy(barObj);
    }

    [UnityTest]
    public IEnumerator ClampsAtMax() {
        var playerObj = new GameObject();
        var stamina = playerObj.AddComponent<Stamina>();
        var coordinates = playerObj.AddComponent<PlayerCoordinates>();
        var motor = playerObj.AddComponent<PlayerMotor>();
        stamina.max = 100f;
        stamina.regenSpeed = 50f;
        stamina.regenDelay = 0f;

        var barObj = new GameObject();
        var canvas = barObj.AddComponent<Canvas>();
        stamina.staminaBar = barObj.AddComponent<Image>();

        yield return null;

        stamina.current = 95f;

        stamina.HandleStamina(false, false);
        stamina.HandleStamina(false, false);
        stamina.HandleStamina(false, false);

        Assert.LessOrEqual(stamina.current, 100f, "Stamina should not exceed max");

        Object.Destroy(playerObj);
        Object.Destroy(barObj);
    }

    [UnityTest]
    public IEnumerator DisablesSprintingWhenStaminaReachesZero() {
        var playerObj = new GameObject();
        var stamina = playerObj.AddComponent<Stamina>();
        var coordinates = playerObj.AddComponent<PlayerCoordinates>();
        var motor = playerObj.AddComponent<PlayerMotor>();
        stamina.max = 100f;
        stamina.drainSpeed = 100f;

        var barObj = new GameObject();
        var canvas = barObj.AddComponent<Canvas>();
        stamina.staminaBar = barObj.AddComponent<Image>();

        yield return null;

        stamina.current = 1f;
        motor.sprinting = true;
        motor.speed = 12f;

        float timeElapsed = 0f;
        while (stamina.current > 0 && timeElapsed < 1f) {
            stamina.HandleStamina(true, true);
            yield return null;
            timeElapsed += Time.deltaTime;
        }

        Assert.AreEqual(0f, stamina.current, 0.01f, "Stamina should be at 0");
        Assert.IsFalse(motor.sprinting, "Sprinting should be disabled");
        Assert.AreEqual(6f, motor.speed, 0.01f, "Speed should be reset to walk speed");

        Object.Destroy(playerObj);
        Object.Destroy(barObj);
    }

    [UnityTest]
    public IEnumerator ResetsRegenTimerWhenSprinting() {
        var playerObj = new GameObject();
        var stamina = playerObj.AddComponent<Stamina>();
        var coordinates = playerObj.AddComponent<PlayerCoordinates>();
        var motor = playerObj.AddComponent<PlayerMotor>();
        stamina.max = 100f;
        stamina.drainSpeed = 10f;
        stamina.regenDelay = 2f;

        var barObj = new GameObject();
        var canvas = barObj.AddComponent<Canvas>();
        stamina.staminaBar = barObj.AddComponent<Image>();

        yield return null;

        stamina.current = 80f;

        yield return new WaitForSeconds(1f);
        stamina.HandleStamina(false, false);
        yield return new WaitForSeconds(0.5f);

        stamina.HandleStamina(true, true);

        stamina.HandleStamina(false, false);

        float currentAfterSprint = stamina.current;

        Assert.LessOrEqual(stamina.current, 80f, "Stamina should not regenerate immediately after sprinting");

        Object.Destroy(playerObj);
        Object.Destroy(barObj);
    }

    [UnityTest]
    public IEnumerator UpdatesStaminaBar() {
        var playerObj = new GameObject();
        var stamina = playerObj.AddComponent<Stamina>();
        var coordinates = playerObj.AddComponent<PlayerCoordinates>();
        var motor = playerObj.AddComponent<PlayerMotor>();
        stamina.max = 100f;
        stamina.lerp = 10f;

        var barObj = new GameObject();
        var canvas = barObj.AddComponent<Canvas>();
        stamina.staminaBar = barObj.AddComponent<Image>();
        stamina.staminaBar.fillAmount = 1f;

        yield return null;

        stamina.current = 50f;

        stamina.UpdateBar();
        yield return null;

        Assert.Less(stamina.staminaBar.fillAmount, 1f, "Bar fill amount should decrease");

        Object.Destroy(playerObj);
        Object.Destroy(barObj);
    }
}