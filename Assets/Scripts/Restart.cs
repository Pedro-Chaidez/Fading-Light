using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour {
    public void LoadGame() {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}