
using UnityEngine;
using UnityEngine.EventSystems;

public class SingletonEventSystem : MonoBehaviour
{
    private static SingletonEventSystem instance;

    void Awake()
    {
        // We do NOT persist this object. Each scene should handle its own EventSystem.
        // This prevents the "Multiple EventSystems" error.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
}
