using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartCityLight : MonoBehaviour
{
    //
    // Configurable Parameters
    //
    public float switchDelayMin = 0;
    public float switchDelayMax = 3;


    //
    // Cached References
    //
    TimeKeeperSystem timeController = null;
    MeshRenderer meshRenderer = null;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        timeController = FindObjectOfType<TimeKeeperSystem>();

        timeController.eventSunrise.AddListener(OnSunrise);
        timeController.eventSunset.AddListener(OnSunset);
        if (timeController.GetDayPeriod() == TimeKeeperSystem.DayPeriod.Day)
            TurnLightOff();
        else
            TurnLightOn();
    }

    private void TurnLightOn()
    {
        meshRenderer.enabled = true;
    }

    private void TurnLightOff()
    {
        meshRenderer.enabled = false;
    }

    private void OnSunrise()
    {
        Invoke("TurnLightOff", Random.Range(switchDelayMin, switchDelayMax));
    }

    private void OnSunset()
    {
        Invoke("TurnLightOn", Random.Range(switchDelayMin, switchDelayMax));
    }
}
