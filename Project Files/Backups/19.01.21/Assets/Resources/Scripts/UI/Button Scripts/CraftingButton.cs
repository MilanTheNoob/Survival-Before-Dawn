using UnityEngine;

public class CraftingButton : Button
{
    [Header("The recipe settings associated with this recipe ui")]
    public CraftingSettings currentRecipe;

    // Called when the button is clicked
    public override void OnPointerDown()
    {
        // Reset the button
        InputManager.instance.Click(this, false);
    }
}
