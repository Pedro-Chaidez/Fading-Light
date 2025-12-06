using NUnit.Framework;
using UnityEngine;

public class PlayerMotorTests {
    [Test]
    public void SprintToggle() {
        var playerMotor = new PlayerMotor();
        playerMotor.speed = 6f;
        bool initialSprinting = playerMotor.sprinting;

        playerMotor.Sprint();

        Assert.IsTrue(playerMotor.sprinting, "Should be sprinting");
        Assert.AreEqual(12f, playerMotor.speed, "Speed should be 12 when sprinting");

        playerMotor.Sprint();

        Assert.IsFalse(playerMotor.sprinting, "Should stop sprinting");
        Assert.AreEqual(6f, playerMotor.speed, "Speed should return to 6");
    }

    [Test]
    public void CrouchToggle() {
        var playerMotor = new PlayerMotor();

        playerMotor.Crouch();

        Assert.IsTrue(playerMotor.crouching, "Should be crouching");
        Assert.AreEqual(0f, playerMotor.crouchTimer, "Timer should reset to 0");

        playerMotor.Crouch();

        Assert.IsFalse(playerMotor.crouching, "Should stop crouching");
    }

    [Test]
    public void CalculateJumpVelocity() {
        var playerMotor = new PlayerMotor();
        playerMotor.jumpHeight = 7f;
        playerMotor.gravity = -10f;

        float expectedVelocity = Mathf.Sqrt(7f * -3.0f * -10f);
        Assert.AreEqual(14.49f, expectedVelocity, 0.01f, "Jump velocity calculation");
    }

    [Test]
    public void Speed() {
        var playerMotor = new PlayerMotor();

        playerMotor.speed = 10f;

        Assert.AreEqual(10f, playerMotor.speed, "Speed property should work");
    }

    [Test]
    public void Sprinting() {
        var playerMotor = new PlayerMotor();

        playerMotor.sprinting = true;
        Assert.IsTrue(playerMotor.sprinting, "Sprinting property should work");
    }

    [Test]
    public void CorrectDefaultValues() {
        var playerMotor = new PlayerMotor();

        Assert.AreEqual(6f, playerMotor.speed, "Default speed should be 6");
        Assert.AreEqual(-10f, playerMotor.gravity, "Default gravity should be -10");
        Assert.AreEqual(7f, playerMotor.jumpHeight, "Default jump height should be 7");
        Assert.IsFalse(playerMotor.sprinting, "Should not be sprinting by default");
        Assert.IsFalse(playerMotor.crouching, "Should not be crouching by default");
    }
}