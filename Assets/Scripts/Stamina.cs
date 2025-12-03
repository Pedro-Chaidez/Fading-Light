using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public float max = 100f;
    public float current;
    private Image staminaBar;
    public float lerp;
    float lerpSpeed;
    public float drainSpeed = 20f;
    public float regenSpeed = 15f;
    public float regenDelay = 2f;
    private float regenTimer;
    private PlayerCoordinates playerCoordinates;
    private Vector3 position;
    private PlayerMotor movement;
    private bool isMoving = false;
    void Start()
    {
        current = max;
        regenTimer = 0f;
        if (movement == null) {
            movement = GetComponent<PlayerMotor>();
        }
        playerCoordinates = GetComponent<PlayerCoordinates>();
        position = playerCoordinates.GetInitPosition();
        staminaBar = GameObject.Find("/Stamina/Bar").GetComponent<Image>();
    }

    void Update()
    {
        if (current > max) {
            current = max;
        }
        if (position != playerCoordinates.GetCurrPosition()) {
            isMoving = true;
            Debug.Log("Player moved");
            position = playerCoordinates.GetCurrPosition();
        } else {
            isMoving = false;
        }
        lerpSpeed = lerp * Time.deltaTime;
        if (movement != null) {
            HandleStamina(movement.sprinting, isMoving);
        }

        UpdateBar();
    }
    public void HandleStamina(bool isSprinting, bool isMoving) {
        if (isSprinting && current > 0 && isMoving) {
            current -= drainSpeed * Time.deltaTime;
            if (current < 0) {
                current = 0;
                movement.sprinting = false;
                movement.speed = 6f;
            }
            regenTimer = 0f;
        } else {
            if (current < max) {
                regenTimer += Time.deltaTime;
                if (regenTimer >= regenDelay) {
                    current += regenSpeed * Time.deltaTime;
                    if (current >= max) {
                        current = max;
                    }
                }
            }
        }
    }

    public void UpdateBar() {
        staminaBar.fillAmount = Mathf.Lerp(staminaBar.fillAmount, current / max, lerpSpeed);
    }
}