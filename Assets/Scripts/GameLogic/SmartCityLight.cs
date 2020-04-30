using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartCityLight : MonoBehaviour
{
    //
    // Cached References
    //
    TimeController timeController = null;
    MeshRenderer meshRenderer = null;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        timeController = FindObjectOfType<TimeController>();

        timeController.eventSunrise.AddListener(OnSunrise);
        timeController.eventSunset.AddListener(OnSunset);
        if (timeController.GetDayPeriod() == TimeController.DayPeriod.Day)
            OnSunrise();
        else
            OnSunset();
    }

    void OnSunrise()
    {
        meshRenderer.enabled = false;
    }

    void OnSunset()
    {
        meshRenderer.enabled = true;
    }
}
