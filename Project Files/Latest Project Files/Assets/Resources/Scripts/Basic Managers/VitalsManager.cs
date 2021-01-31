using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class VitalsManager : MonoBehaviour
{

    [Header("Player vitals")]
    public VitalStruct[] vitals;
    [Header("UI components")]
    public GameObject bloodPanel;
    public GameObject deathScreen;

    public static VitalsManager instance;
    bool isDying;

    void Awake()
    {
        instance = this;

        SavingManager.SaveGameCallback += SaveVitals;
        LoadVitals();

        ModifyVitalAmount(0, 0);

        for (int i = 0; i < vitals.Length; i++) { if (vitals[i].subtract != 0) { StartCoroutine(ModifyVitals(vitals[i])); } }
    }

    IEnumerator ModifyVitals(VitalStruct vital)
    {
        while (true)
        {
            ModifyVitalAmount(vital, -vital.subtract);
            yield return new WaitForSeconds(1f);
        }
    }

    public void ModifyVitalAmount(int vital, float change) { ModifyVitalAmount(vitals[vital], change); }
    public void ModifyVitalAmount(VitalStruct vital, float change)
    {
        vital.amount += change;
        vital.bar.fillAmount = vital.amount;

        if (vital.warning != null && vital.amount <= 0.2f) { vital.warning.gameObject.SetActive(true); } else { vital.warning.gameObject.SetActive(false); }
        if (vital.amount > 1f) { vital.amount = 1f; }
        if (vital.amount < 0f && vital != vitals[0]) { ModifyVitalAmount(0, -vitals[0].hit); vital.amount = 0f; }
        if (vital == vitals[0] && change < 0) { StartCoroutine(HurtPlayerVisual()); }

        if (vitals[0].amount <= 0)
        {
            ResetVitals();
            InputManager.instance.ToggleUISectionsInt(0);
            StartCoroutine(Death());
        }
    }

    public void ResetVitals() { for (int i = 0; i < vitals.Length; i++) { vitals[i].amount = 1f; } }
    public void LoadVitals() { for (int i = 0; i < SavingManager.SaveFile.vitals.Count; i++) { vitals[i].amount = SavingManager.SaveFile.vitals[i]; ModifyVitalAmount(i, 0); } }
    public void SaveVitals() { SavingManager.SaveFile.vitals.Clear(); for (int i = 0; i < vitals.Length; i++) { SavingManager.SaveFile.vitals.Add(vitals[i].amount); } }

    public IEnumerator HurtPlayerVisual()
    {
        if (!isDying)
        {
            isDying = true;
            TweeningLibrary.FadeIn(bloodPanel, 0.5f);
            yield return new WaitForSeconds(1f);
            TweeningLibrary.FadeOut(bloodPanel, 0.5f);
            yield return new WaitForSeconds(3f);
            isDying = false;
        }
    }

    public IEnumerator Death()
    {
        TweeningLibrary.FadeIn(deathScreen, 0.2f);
        yield return new WaitForSeconds(5.5f);
        TweeningLibrary.FadeOut(deathScreen, 0.2f);

        ModifyVitalAmount(0, 0f);
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
