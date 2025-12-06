using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PowerManagerPlayModeTests {
    [UnityTest]
    public IEnumerator InitializesCurrentToStart() {
        var powerObj = new GameObject();
        var powerManager = powerObj.AddComponent<PowerManager>();
        powerManager.start = 100f;

        yield return null;

        Assert.AreEqual(100f, powerManager.current, "Current should initialize to start");

        Object.Destroy(powerObj);
    }

    [UnityTest]
    public IEnumerator DrainsPowerWithNormalLight() {
        var powerObj = new GameObject();


        var flashlight = powerObj.AddComponent<Flashlight>();
        var powerManager = powerObj.AddComponent<PowerManager>();
        powerManager.start = 100f;
        powerManager.tickValue = 1f;

        yield return null;

        flashlight.viewDistance = 10f;

        float initialPower = powerManager.current;

        powerManager.DrainPowerBasedOnFlashlight();

        Assert.Less(powerManager.current, initialPower, "Power should drain");
        Assert.AreEqual(99f, powerManager.current, 0.01f, "Should drain 1 unit for normal light");

        Object.Destroy(powerObj);
    }

    [UnityTest]
    public IEnumerator DrainsMorePowerWithMaxLight() {
        var powerObj = new GameObject();
        var flashlight = powerObj.AddComponent<Flashlight>();
        var powerManager = powerObj.AddComponent<PowerManager>();

        powerManager.start = 100f;
        powerManager.current = 100f;
        powerManager.tickValue = 1f;

        yield return null;

        flashlight.viewDistance = 15f;

        powerManager.DrainPowerBasedOnFlashlight();

        Assert.AreEqual(97f, powerManager.current, 0.01f, "Should drain 3 units for max light");

        Object.Destroy(powerObj);
    }

    [UnityTest]
    public IEnumerator StopAtZero() {
        var powerObj = new GameObject();
        var flashlight = powerObj.AddComponent<Flashlight>();
        var powerManager = powerObj.AddComponent<PowerManager>();
        powerManager.start = 100f;
        powerManager.tickValue = 1f;

        yield return null;

        powerManager.current = 0.5f;
        flashlight.viewDistance = 10f;
        powerManager.DrainPowerBasedOnFlashlight();

        Assert.AreEqual(0f, powerManager.current, 0.01f, "Should stop at 0, not go negative");

        Object.Destroy(powerObj);
    }

    [UnityTest]
    public IEnumerator BarVisibility() {
        var powerObj = new GameObject();
        var powerManager = powerObj.AddComponent<PowerManager>();

        var bar1 = new GameObject("Bar1");
        var bar3 = new GameObject("Bar3");
        powerManager.bar1 = bar1;
        powerManager.bar3 = bar3;

        yield return null;

        powerManager.SetBarVisibility(true, false);

        Assert.IsTrue(bar1.activeSelf, "Bar1 should be active");
        Assert.IsFalse(bar3.activeSelf, "Bar3 should be inactive");

        powerManager.SetBarVisibility(true, true);

        Assert.IsTrue(bar1.activeSelf, "Both bars should be active");
        Assert.IsTrue(bar3.activeSelf, "Both bars should be active");

        powerManager.SetBarVisibility(false, false);

        Assert.IsFalse(bar1.activeSelf, "Bars should be inactive");
        Assert.IsFalse(bar3.activeSelf, "Bars should be inactive");

        Object.Destroy(powerObj);
        Object.Destroy(bar1);
        Object.Destroy(bar3);
    }
}