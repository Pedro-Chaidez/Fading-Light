using UnityEngine;

public class GhostDamage : MonoBehaviour {
    public float damagePerSecond = 10f;
    private float damageInterval = 1f;
    private float lastDamageTime = -999f;

    private void OnTriggerStay(Collider other) {
        ProcessDamage(other);
    }

    // Extract logic for testing
    private void ProcessDamage(Collider other) {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null) {
            if (Time.time - lastDamageTime >= damageInterval) {
                player.TakeDamage(damagePerSecond);
                lastDamageTime = Time.time;
            }
        }
    }

#if UNITY_EDITOR
    public void TestProcessDamage(Collider other) {
        ProcessDamage(other);
    }
#endif
}