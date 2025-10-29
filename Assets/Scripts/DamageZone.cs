using UnityEngine;

public class GhostDamage : MonoBehaviour {
    public float damagePerSecond = 10f;
    private float damageInterval = 1f;
    private float lastDamageTime = 0f;

    private void OnTriggerStay(Collider other) {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null) {
            if (Time.time - lastDamageTime >= damageInterval) {
                player.TakeDamage(damagePerSecond);
                lastDamageTime = Time.time;
            }
        }
    }
}
