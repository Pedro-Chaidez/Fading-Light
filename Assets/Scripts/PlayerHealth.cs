using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {
    public float maxHealth = 100f;
    public float currentHealth;
    public GameObject diedScreen;

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
            transform.position = playerCoordinates.GetPosition();
            transform.rotation = playerCoordinates.GetRotation();
            character.enabled = true;
        }

        Debug.Log("Player Health: " + currentHealth);
    }
}
