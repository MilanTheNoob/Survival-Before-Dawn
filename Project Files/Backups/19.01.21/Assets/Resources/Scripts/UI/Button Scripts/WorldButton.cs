using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldButton : Button
{
    [Header("The world settings associated with this button")]
    public string currentWorld;

    // Called when the button is clicked
    public override void OnPointerDown()
    {
        // Reset the button
        InputManager.instance.Click(this, false);
    }
}
