using NUnit.Framework;
using UnityEngine;

public class PowerManagerTests {
    [Test]
    public void DrainForNormalLight() {
        var powerManager = new PowerManager();
        powerManager.tickValue = 0.01f;

        float drain = powerManager.CalculateDrainAmount(10f);

        Assert.AreEqual(0.01f, drain, "Normal light should drain tickValue");
    }

    [Test]
    public void DrainForMaxLight() {
        var powerManager = new PowerManager();
        powerManager.tickValue = 0.01f;

        float drain = powerManager.CalculateDrainAmount(15f);

        Assert.AreEqual(0.03f, drain, 0.0001f, "Max light should drain 3x tickValue");
    }

    [Test]
    public void NoDrainWhenLightOff() {
        var powerManager = new PowerManager();
        powerManager.tickValue = 0.01f;

        float drain = powerManager.CalculateDrainAmount(5f);

        Assert.AreEqual(0f, drain, "No drain when light is off");
    }

    [Test]
    public void ClampsCurrentToZero() {
        var powerManager = new PowerManager();
        powerManager.start = 100f;
        powerManager.current = -10f;

        powerManager.ProcessPowerDrain(0f);

        Assert.AreEqual(0f, powerManager.current, "Current should clamp to 0");
    }

    [Test]
    public void ClampsCurrentToMax() {
        var powerManager = new PowerManager();
        powerManager.start = 100f;
        powerManager.current = 150f;

        powerManager.ProcessPowerDrain(0f);

        // Assert
        Assert.AreEqual(100f, powerManager.current, "Current should clamp to start value");
    }

    [Test]
    public void CorrectDefaults() {
        var powerManager = new PowerManager();

        Assert.AreEqual(100f, powerManager.start, "Default start should be 100");
        Assert.AreEqual(0f, powerManager.current, "Default current should be 0");
        Assert.AreEqual(0.01f, powerManager.tickValue, "Default tick value should be 0.01");
    }
}