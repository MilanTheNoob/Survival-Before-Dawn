using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyButton : Button
{
    public override void OnPointerDown()
    {
        InputManager.instance.Click(this, true);
    }
}
