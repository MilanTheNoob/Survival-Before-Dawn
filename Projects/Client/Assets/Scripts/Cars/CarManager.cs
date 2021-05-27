using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    #region Singleton

    public static CarManager instance;
    void Awake() { instance = this; }

    #endregion

    public Button Exit;
    public SteeringWheel Wheel;
    public Slider Accelerator;

    [Space]

    public GameObject PlayerUI;
    public GameObject CarUI;

    static WheelVehicle currentCar;

    void Start()
    {
        TweeningLibrary.FadeIn(instance.PlayerUI, 0.2f);
        TweeningLibrary.FadeOut(instance.CarUI, 0.2f);
    }

    void FixedUpdate()
    {
        if (currentCar != null)
        {
            currentCar.steering = Wheel.OutPut * 50;
            currentCar.throttle = Accelerator.value;
            print(Accelerator.value);

            if (Exit.onClicked)
            {
                SavingManager.player.SetActive(true);
                SavingManager.player.transform.parent = null;

                TweeningLibrary.FadeIn(instance.PlayerUI, 0.2f);
                TweeningLibrary.FadeOut(instance.CarUI, 0.2f);

                currentCar.Handbrake = true;
                currentCar = null;
            }
        }
    }

    public static void EnterCar(WheelVehicle car)
    {
        currentCar = car;
        currentCar.Handbrake = false;

        SavingManager.player.SetActive(false);
        SavingManager.player.transform.parent = currentCar.transform;
        SavingManager.player.transform.position = new Vector3(SavingManager.player.transform.position.x, SavingManager.player.transform.position.y + 5f, SavingManager.player.transform.position.z);

        TweeningLibrary.FadeOut(instance.PlayerUI, 0.2f);
        TweeningLibrary.FadeIn(instance.CarUI, 0.2f);
    }
}
