using UnityEngine;
using TMPro;
public class PlayerUI : MonoBehaviour
{
    private TextMeshProUGUI promptText;
    private void Start()
    {
        var obj = GameObject.Find("InteractText");
        if (obj != null)
        {
            promptText = obj.GetComponent<TextMeshProUGUI>();
        }
        
        if (promptText == null)
        {
            Debug.LogWarning("PlayerUI: Could not find 'InteractText' object or TextMeshProUGUI component. UI prompts will not work.");
        }
    }
    public void UpdateText(string promptMessage)
    {
        if (promptText != null)
        {
            promptText.text = promptMessage;
        }
    }
}
