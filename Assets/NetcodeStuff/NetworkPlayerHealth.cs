using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkObject))]
public class NetworkPlayerHealth : NetworkBehaviour
{
    public float maxHealth = 100f;
    private NetworkVariable<float> currentHealth = new NetworkVariable<float>(100f);
    private Image healthBar;
    [SerializeField]
    private GameObject diedScreen;
    public float lerp;
    float lerpSpeed;

    [Header("Low Health Flash Settings")]
    public float lowHealthThreshold = 25f;
    public float flashSpeed = 4f;
    private bool isLowHealthFlashing = false;

    private PlayerCoordinates playerCoordinates;
    private CharacterController character;
    private NetworkPlayerMotor movement;
    private Stamina stamina;
    private PowerManager powerManager;
    private Inventory inventory;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsOwner)
        {
            currentHealth.OnValueChanged += OnHealthChanged;
            InitializeHealth();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner && currentHealth != null)
        {
            currentHealth.OnValueChanged -= OnHealthChanged;
        }
        base.OnNetworkDespawn();
    }

    private void InitializeHealth()
    {
        maxHealth = 100f;
        if (IsServer)
        {
            currentHealth.Value = 100f;
        }
        
        Debug.Log($"NetworkPlayerHealth: Initialized with {currentHealth.Value} health");
        
        if (diedScreen != null)
        {
            diedScreen.SetActive(false);
        }
        
        playerCoordinates = GetComponent<PlayerCoordinates>();
        character = GetComponent<CharacterController>();
        movement = GetComponent<NetworkPlayerMotor>();
        stamina = GetComponent<Stamina>();
        powerManager = GetComponent<PowerManager>();
        inventory = GetComponent<Inventory>();
        
        // These components are optional, so don't error if they don't exist
        
        // Find health bar - try multiple paths
        GameObject healthBarObj = GameObject.Find("/HealthBar/Bar");
        if (healthBarObj == null)
        {
            healthBarObj = GameObject.Find("HealthBar/Bar");
        }
        if (healthBarObj != null)
        {
            healthBar = healthBarObj.GetComponent<Image>();
        }
        
        if (diedScreen != null)
        {
            diedScreen = Instantiate(diedScreen);
            diedScreen.SetActive(false);
        }
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        if (!IsOwner) return;
        
        Debug.Log($"NetworkPlayerHealth: Health changed from {oldValue} to {newValue}");
        
        if (newValue <= 0f && oldValue > 0f)
        {
            Die();
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        
        if (currentHealth.Value > maxHealth && IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        lerpSpeed = lerp * Time.deltaTime;

        HealthBarFiller();
        ColorChanger();
    }

    void HealthBarFiller()
    {
        if (healthBar == null) return;
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth.Value / maxHealth, lerpSpeed);
    }

    void ColorChanger()
    {
        if (healthBar == null) return;
        
        float healthPercent = currentHealth.Value / maxHealth;

        if (currentHealth.Value <= lowHealthThreshold && currentHealth.Value > 0)
        {
            isLowHealthFlashing = true;
            float t = Mathf.Abs(Mathf.Sin(Time.time * flashSpeed));
            Color flashColor = Color.Lerp(new Color(0.5f, 0, 0, 0.8f), Color.red, t);
            healthBar.color = flashColor;
        }
        else
        {
            isLowHealthFlashing = false;
            Color healthColor = Color.Lerp(Color.red, Color.green, healthPercent);
            healthBar.color = healthColor;
        }
    }

    public void TakeDamage(float amount)
    {
        if (!IsServer) return;
        
        currentHealth.Value -= amount;
        currentHealth.Value = Mathf.Max(currentHealth.Value, 0f);
        Debug.Log($"NetworkPlayerHealth: Player took {amount} damage. Health: {currentHealth.Value}");
    }

    public void Heal(float amount)
    {
        if (!IsServer) return;
        
        currentHealth.Value += amount;
        currentHealth.Value = Mathf.Min(currentHealth.Value, maxHealth);
        Debug.Log($"NetworkPlayerHealth: Player healed {amount}. Health: {currentHealth.Value}");
    }

    private void Die()
    {
        if (!IsOwner) return;
        
        Debug.Log("NetworkPlayerHealth: Player died!");
        if (diedScreen != null)
        {
            diedScreen.SetActive(true);
        }
        
        var inputManager = GetComponent<InputManager>();
        if (inputManager != null) inputManager.enabled = false;
        if (stamina != null) stamina.enabled = false;
        if (powerManager != null) powerManager.enabled = false;
        if (inventory != null) inventory.enabled = false;
    }

    public float GetCurrentHealth()
    {
        return currentHealth.Value;
    }
}
