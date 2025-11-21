using UnityEngine;
using TMPro;

public class PowerManager : MonoBehaviour
{
    public float start = 100f;
    public float current = 100f;
    public float tickValue = 0.01f;
    private float tickTime = 0.01f;
    private float lastTick = 0f;
    public TMP_Text percentage;
    public GameObject bar1;
    public GameObject bar3;
    private Flashlight flashlight;
    void Start()
    {
        flashlight = GetComponent<Flashlight>();
        current = start;
    }

    void Update() {
        if (current <= 0) {
            current = 0;
        }

        if (flashlight != null && Time.time - lastTick >= tickTime) {
            if (flashlight.viewDistance == 10f) {
                current -= tickValue;
                bar1.SetActive(true);
                bar3.SetActive(false);
            } else if (flashlight.viewDistance == 15f) {
                current -= (tickValue) * 5;
                bar1.SetActive(true);
                bar3.SetActive(true);
            } else {
                bar1.SetActive(false);
                bar3.SetActive(false);
            }
                lastTick = Time.time;
        }
        current = Mathf.Clamp(current, 0f, start);
        percentage.text = Mathf.RoundToInt(current).ToString() + "%";
    }
}
