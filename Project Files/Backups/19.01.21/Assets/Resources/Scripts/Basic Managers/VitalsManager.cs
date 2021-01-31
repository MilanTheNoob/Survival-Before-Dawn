using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class VitalsManager : MonoBehaviour
{

    [Header("All the player's vitals")]
    public VitalStruct[] vitals;

    [Header("The UI components")]
    public GameObject bloodPanel;
    public GameObject deathScreen;

    // Our singleton
    public static VitalsManager instance;
    // Are we dying?
    bool isDying;

    // Start is called before the first frame update
    void Awake()
    {
        // Set the instance to ourselves
        instance = this;

        // Add the Save Func to the Save Game Callback, same goes for Load Func
        SavingManager.SaveGameCallback += SaveVitals;
        LoadVitals();

        // Quickly update the ealth
        ModifyVitalAmount(0, 0);

        for (int i = 0; i < vitals.Length; i++)
        {
            if (vitals[i].subtract != 0) { StartCoroutine(ModifyVitals(vitals[i])); }
        }
    }

    // Called to change a vital throughout the game avery second
    IEnumerator ModifyVitals(VitalStruct vital)
    {
        // Loop forever
        while (true)
        {
            // Modify the vital
            ModifyVitalAmount(vital, -vital.subtract);
            // Wait a second
            yield return new WaitForSeconds(1f);
        }
    }

    // Called to modify the varAmount amount of a vital
    public void ModifyVitalAmount(int vital, float change) { ModifyVitalAmount(vitals[vital], change); }
    public void ModifyVitalAmount(VitalStruct vital, float change)
    {
        // Change the varAmount
        vital.amount += change;
        // Set the fill amount of the vital UI
        vital.bar.fillAmount = vital.amount;

        // Enable the vital warning if the vital is low enough
        if (vital.warning != null && vital.amount <= 0.2f) { vital.warning.gameObject.SetActive(true); } else { vital.warning.gameObject.SetActive(false); }

        // Cap the the amount if its too high
        if (vital.amount > 1f)
            vital.amount = 1f;

        // Start hurting the player if the vital (isnt the health vital aswell) is at 0
        if (vital.amount < 0f && vital != vitals[0]) { ModifyVitalAmount(0, -vitals[0].hit); vital.amount = 0f; }

        // Show that the player is hurting
        if (vital == vitals[0] && change < 0)
            StartCoroutine(HurtPlayerVisual());

        if (vitals[0].amount <= 0)
        {
            // Reset Vitals
            ResetVitals();

            // Disable the UI
            InputManager.instance.ToggleUISectionsInt(0);

            // Start fading the death screen
            StartCoroutine(Death());
        }
    }

    // Called to reset all the vitals
    public void ResetVitals()
    {
        // Loop through all the vitals and reset them
        for (int i = 0; i < vitals.Length; i++) { vitals[i].amount = 1f; }
    }

    // Called to visualise the player being hurt
    public IEnumerator HurtPlayerVisual()
    {
        // Is the player not dying?
        if (!isDying)
        {
            // Well he is now!
            isDying = true;
            // Fade in the blood panel
            TweeningLibrary.FadeIn(bloodPanel, 0.5f);
            // Wait for 0.3 seconds
            yield return new WaitForSeconds(1f);
            // Fade out the blood panel
            TweeningLibrary.FadeOut(bloodPanel, 0.5f);
            yield return new WaitForSeconds(3f);
            // And now he is no longer dying :)
            isDying = false;
        }
    }

    // Called to fade a death component
    public IEnumerator Death()
    {
        // Wait for a while and do a whole lot of fading
        TweeningLibrary.FadeIn(deathScreen, 0.2f);
        yield return new WaitForSeconds(5.5f);
        TweeningLibrary.FadeOut(deathScreen, 0.2f);

        ModifyVitalAmount(0, 0f);
    }

    // Here to save all the vitals
    public void SaveVitals()
    {
        // Clear the save data
        SavingManager.SaveFile.vitals.Clear();
        // Add all the vital values
        for (int i = 0; i < vitals.Length; i++) { SavingManager.SaveFile.vitals.Add(vitals[i].amount); }
    }

    // Called to load the vitals
    public void LoadVitals()
    {
        // Loop through all vitals and load em up!
        for (int i = 0; i < SavingManager.SaveFile.vitals.Count; i++) 
        {
            // Set the vital
            vitals[i].amount = SavingManager.SaveFile.vitals[i];
            // Update the vital
            ModifyVitalAmount(i, 0);
        }
    }
}

[System.Serializable]
public class VitalStruct
{
    public float amount = 1f;

    public float subtract = 0.0002f;
    public float hit = 0.001f;

    public Image bar;
    public Image warning;
}
