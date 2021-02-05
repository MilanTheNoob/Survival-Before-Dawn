using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Inventory System/Food Item Settings")]
public class FoodItemSettings : ItemSettings
{
    [Header("How much does the item restore health & hunger by")]
    public float healthChange = 0.05f;
    public float hungerChange = 0.4f;

    bool used;

    private void Awake() { isUsableItem = true; }

    public override void Use()
    {
        if (SavingManager.GameState == SavingManager.GameStateEnum.Singleplayer)
        {
            base.Use();

            VitalsManager.instance.ModifyVitalAmount(0, healthChange);
            VitalsManager.instance.ModifyVitalAmount(1, hungerChange);

            Inventory.instance.Destroy(this);
        }
        else if (SavingManager.GameState == SavingManager.GameStateEnum.Multiplayer && !used)
        {
            ClientSend.UpdateVital(healthChange, hungerChange);
            used = true;
        }
    }
}
