using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerateNextLevel : MonoBehaviour
{
    public void level1()
    {
        SceneManager.LoadScene("Level1");
    }
}
