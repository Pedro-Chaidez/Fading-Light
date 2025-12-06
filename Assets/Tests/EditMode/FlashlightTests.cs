using NUnit.Framework;
using UnityEngine;

public class FlashlightTests {
    [Test]
    public void NormalLight() {
        var flashlight = new Flashlight();
        bool initialState = flashlight.normLightToggle;

        flashlight.normLight();

        Assert.AreNotEqual(initialState, flashlight.normLightToggle, "normLightToggle should flip");

        flashlight.normLight();

        Assert.AreEqual(initialState, flashlight.normLightToggle, "Should toggle back to original");
    }

    [Test]
    public void MaxLight() {
        var flashlight = new Flashlight();
        bool initialState = flashlight.maxLightToggle;

        flashlight.maxLight();

        Assert.AreNotEqual(initialState, flashlight.maxLightToggle, "maxLightToggle should flip");

        flashlight.maxLight();

        Assert.AreEqual(initialState, flashlight.maxLightToggle, "Should toggle back to original");
    }
}