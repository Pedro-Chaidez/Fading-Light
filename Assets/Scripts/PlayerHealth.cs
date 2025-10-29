using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    public float maxHealth = 100f;
    public float currentHealth;

    private void Start() {
        currentHealth = maxHealth;
        Debug.Log("Player Health: " + currentHealth);
    }

    public void TakeDamage(float amount) {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0f) {
            Die();
        }
    }

    private void Die() {
        Debug.Log("You died!");
    }
}
