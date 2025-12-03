using UnityEngine;
using TMPro;

public class PowerManager : MonoBehaviour
{
    public float start = 100f;
    public float current = 0f;
    public float tickValue = 0.01f;
    private float tickTime = 0.01f;
    private float lastTick = 0f;
    private TMP_Text percentage;
    private GameObject bar1;
    private GameObject bar3;
    private Flashlight flashlight;
    [SerializeField]
    private GameObject PowerUI;
    void Start()
    {
        flashlight = GetComponent<Flashlight>();
        current = start;
        PowerUI = Instantiate(PowerUI);
        percentage = PowerUI.transform.GetChild(1).GetComponent<TMP_Text>();
        bar1 = PowerUI.transform.GetChild(3).gameObject;
        bar1.SetActive(false);
        bar3 = PowerUI.transform.GetChild(4).gameObject;
        bar3.SetActive(false);
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
                current -= (tickValue * 3);
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
