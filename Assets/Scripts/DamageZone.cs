using UnityEngine;

public class GhostDamage : MonoBehaviour {
    public float damagePerSecond = 10f;
    private float damageInterval = 1f;
    private float lastDamageTime = -999f;

    private PlayerHealth playerInside;

    private void OnTriggerEnter(Collider other) {
        var p = other.GetComponent<PlayerHealth>();
        if (p != null)
            playerInside = p;
    }

    private void OnTriggerExit(Collider other) {
        var p = other.GetComponent<PlayerHealth>();
        if (p != null && p == playerInside)
            playerInside = null;
    }

    private void FixedUpdate() {
        if (playerInside == null) return;

        if (Time.time - lastDamageTime >= damageInterval) {
            playerInside.TakeDamage(damagePerSecond);
            lastDamageTime = Time.time;
            Debug.Log("Player Health: " + playerInside.currentHealth);
        }
    }

#if UNITY_EDITOR
    public void TestProcessDamage(PlayerHealth ph) {
        playerInside = ph;
        FixedUpdate();
    }
#endif
}
