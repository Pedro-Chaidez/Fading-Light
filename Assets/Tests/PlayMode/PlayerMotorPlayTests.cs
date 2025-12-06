using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerMotorPlayModeTests {
    [UnityTest]
    public IEnumerator InitializesWithCharacterController() {
        var playerObj = new GameObject();
        var controller = playerObj.AddComponent<CharacterController>();
        var playerMotor = playerObj.AddComponent<PlayerMotor>();

        yield return null;

        Assert.IsNotNull(playerMotor);
        Assert.AreEqual(6f, playerMotor.speed, "Speed should be initialized");

        Object.Destroy(playerObj);
    }

    [UnityTest]
    public IEnumerator SprintChangesSpeed() {
        var playerObj = new GameObject();
        playerObj.AddComponent<CharacterController>();
        var playerMotor = playerObj.AddComponent<PlayerMotor>();

        yield return null;
        playerMotor.Sprint();

        Assert.IsTrue(playerMotor.sprinting);
        Assert.AreEqual(12f, playerMotor.speed);

        playerMotor.Sprint();

        Assert.IsFalse(playerMotor.sprinting);
        Assert.AreEqual(6f, playerMotor.speed);

        // Cleanup
        Object.Destroy(playerObj);
    }

    [UnityTest]
    public IEnumerator CrouchResetsCrouchTimer() {
        var playerObj = new GameObject();
        playerObj.AddComponent<CharacterController>();
        var playerMotor = playerObj.AddComponent<PlayerMotor>();

        yield return null;

        playerMotor.crouchTimer = 5f;

        playerMotor.Crouch();

        Assert.AreEqual(0f, playerMotor.crouchTimer, "Timer should reset");
        Assert.IsTrue(playerMotor.crouching);

        Object.Destroy(playerObj);
    }

    [UnityTest]
    public IEnumerator MoveDoesNotCrash() {
        var playerObj = new GameObject();
        playerObj.AddComponent<CharacterController>();
        var playerMotor = playerObj.AddComponent<PlayerMotor>();

        yield return null;

        playerMotor.ProcessMove(new Vector2(1f, 0f));

        yield return null;

        Assert.IsNotNull(playerMotor);

        Object.Destroy(playerObj);
    }
}