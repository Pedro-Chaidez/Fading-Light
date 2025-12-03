using UnityEngine;
using TMPro;
public class PlayerUI : MonoBehaviour
{
    private TextMeshProUGUI promptText;
    private void Start()
    {
        promptText = GameObject.Find("/Canvas/InteractText").GetComponent<TextMeshProUGUI>();
    }
    public void UpdateText(string promptMessage)
    {
        promptText.text = promptMessage;
    }
}
