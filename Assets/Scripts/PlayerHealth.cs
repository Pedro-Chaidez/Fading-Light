using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBar;
    public GameObject diedScreen;
    public float lerp;
    float lerpSpeed;

    [Header("Low Health Flash Settings")]
    public float lowHealthThreshold = 25f;
    public float flashSpeed = 4f;
    private bool isLowHealthFlashing = false;

    private PlayerCoordinates playerCoordinates;
    private CharacterController character;

    private void Start() {
        currentHealth = maxHealth;
        Debug.Log("Player Health: " + currentHealth);
        if (diedScreen != null) {
            diedScreen.SetActive(false);
        }
        playerCoordinates = GetComponent<PlayerCoordinates>();
        character = GetComponent<CharacterController>();
    }
    private void Update() {
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }

        lerpSpeed = lerp * Time.deltaTime;

        HealthBarFiller();
        ColorChanger();
    }

    void HealthBarFiller() {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, lerpSpeed);
    }

    void ColorChanger() {
        float healthPercent = currentHealth / maxHealth;

        if (currentHealth <= lowHealthThreshold && currentHealth > 0) {
            isLowHealthFlashing = true;
            float t = Mathf.Abs(Mathf.Sin(Time.time * flashSpeed)); // oscillates 0–1
            Color flashColor = Color.Lerp(new Color(0.5f, 0, 0, 0.8f), Color.red, t);
            healthBar.color = flashColor;
        } else {
            isLowHealthFlashing = false;
            Color healthColor = Color.Lerp(Color.red, Color.green, healthPercent);
            healthBar.color = healthColor;
        }
    }

    public void TakeDamage(float amount) {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0f) {
            Die();
        }
    }

    public void Heal(float amount) {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, 100f);
        Debug.Log("Player Health: " + currentHealth);
    }

    private void Die() {
        Debug.Log("You died!");
        if (diedScreen != null) {
            diedScreen.SetActive(true);
        }
        GetComponent<InputManager>().enabled = false;
    }

    public void Resurrect() {
        Debug.Log("Resurrected");
        if (diedScreen != null) {
            diedScreen.SetActive(false);
        }
        currentHealth = maxHealth;
        GetComponent<InputManager>().enabled = true;
        
        if (playerCoordinates != null && character != null) {
            character.enabled = false;
            transform.position = playerCoordinates.GetInitPosition();
            transform.rotation = playerCoordinates.GetRotation();
            character.enabled = true;
        }

        Debug.Log("Player Health: " + currentHealth);
    }
}
