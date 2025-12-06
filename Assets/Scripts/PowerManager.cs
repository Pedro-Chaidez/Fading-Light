using UnityEngine;
using TMPro;

public class PowerManager : MonoBehaviour {
    public float start = 100f;
    public float current = 0f;
    public float tickValue = 0.01f;
    private float tickTime = 0.01f;
    private float lastTick = 0f;
    public TMP_Text percentage;
    public GameObject bar1;
    public GameObject bar3;
    private Flashlight flashlight;

    void Start() {
        flashlight = GetComponent<Flashlight>();
        current = start;
    }

    void Update() {
        ProcessPowerDrain(Time.time);
        UpdateUI();
    }

    // Extract for testing
    public void ProcessPowerDrain(float currentTime) {
        if (current <= 0)
            current = 0;

        if (flashlight != null && currentTime - lastTick >= tickTime) {
            DrainPowerBasedOnFlashlight();
            lastTick = currentTime;
        }

        current = Mathf.Clamp(current, 0f, start);
    }

    // Extract drain logic for testing
    public void DrainPowerBasedOnFlashlight() {
        if (flashlight == null) return;

        if (flashlight.viewDistance == 10f) {
            current -= tickValue;
            SetBarVisibility(true, false);
        } else if (flashlight.viewDistance == 15f) {
            current -= tickValue * 3;
            SetBarVisibility(true, true);
        } else {
            SetBarVisibility(false, false);
        }

        current = Mathf.Clamp(current, 0f, start);
    }

    // Extract bar visibility for testing
    public void SetBarVisibility(bool bar1Active, bool bar3Active) {
        if (bar1 != null)
            bar1.SetActive(bar1Active);
        if (bar3 != null)
            bar3.SetActive(bar3Active);
    }

    // Extract UI update for testing
    public void UpdateUI() {
        if (percentage != null)
            percentage.text = Mathf.RoundToInt(current).ToString() + "%";
    }

    // Calculate drain amount based on view distance
    public float CalculateDrainAmount(float viewDistance) {
        if (viewDistance == 10f)
            return tickValue;
        else if (viewDistance == 15f)
            return tickValue * 3;
        else
            return 0f;
    }

#if UNITY_EDITOR
    // Test helpers
    public void TestSetCurrent(float value) { current = value; }
    public void TestSetLastTick(float value) { lastTick = value; }
    public float TestGetLastTick() { return lastTick; }
#endif
}