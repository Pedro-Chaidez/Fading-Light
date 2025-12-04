using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour {
    public void LoadGame() {
        SceneManager.LoadScene("Main Game");
    }
}
