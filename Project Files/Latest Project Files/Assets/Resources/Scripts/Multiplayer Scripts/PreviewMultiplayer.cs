using UnityEngine.UI;
using UnityEngine;

public class PreviewMultiplayer : MonoBehaviour
{
    [Header("UI Components")]
    public PreviewButton playButton;
    public InputField nameField;
    public InputField serverField;
    public InputField portField;


    void FixedUpdate()
    {
        if (playButton.onClicked)
        {
            if (portField.text.Length > 0)
            {
                bool c = int.TryParse(portField.text, out int port);
                if (c) { SavingManager.LoadServer(serverField.text, port); }
            }
            else { SavingManager.LoadServer(serverField.text, 26950); }
        }
    }
}
