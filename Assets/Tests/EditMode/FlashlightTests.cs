using NUnit.Framework;
using UnityEngine;

public class FlashlightTests {
    [Test]
    public void Flashlight_NormalLight() {
        // Arrange
        var flashlight = new Flashlight();
        bool initialState = flashlight.normLightToggle;

        // Act
        flashlight.normLight();

        // Assert
        Assert.AreNotEqual(initialState, flashlight.normLightToggle, "normLightToggle should flip");

        // Act again
        flashlight.normLight();

        // Assert
        Assert.AreEqual(initialState, flashlight.normLightToggle, "Should toggle back to original");
    }

    [Test]
    public void Flashlight_MaxLight() {
        // Arrange
        var flashlight = new Flashlight();
        bool initialState = flashlight.maxLightToggle;

        // Act
        flashlight.maxLight();

        // Assert
        Assert.AreNotEqual(initialState, flashlight.maxLightToggle, "maxLightToggle should flip");

        // Act again
        flashlight.maxLight();

        // Assert
        Assert.AreEqual(initialState, flashlight.maxLightToggle, "Should toggle back to original");
    }
}