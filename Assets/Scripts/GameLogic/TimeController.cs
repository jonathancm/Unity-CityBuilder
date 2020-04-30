using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeController : MonoBehaviour
{
    public enum DayPeriod
    {
        Night,
        Day
    }

    //
    // Public Constants
    //
    public static int SECONDS_PER_HOUR = 3600;
    public static int SECONDS_PER_DAY = 24 * 3600;
    public static int TIME_MIDNIGHT = 0;
    public static int TIME_MIDDAY = 12 * 3600;

    //
    // Configurable Parameters
    //
    public float gameSecondsPerLifeSeconds = 60;
    [Range(0.0f, 3.0f)] public float timeDilation = 1.0f;

    //
    // Events
    //
    public UnityEvent eventSunset = new UnityEvent();
    public UnityEvent eventSunrise = new UnityEvent();

    //
    // Internal Variables
    //
    private int daysSinceStart = 0;
    private int currentDayTime = 0;
    private DayPeriod dayPeriod = DayPeriod.Night;

    private void Update()
    {
        //
        // Update Current DayTime
        //
        currentDayTime += (int)(gameSecondsPerLifeSeconds * Time.deltaTime * timeDilation);
        if (currentDayTime > SECONDS_PER_DAY)
        {
            daysSinceStart++;
            currentDayTime = 0;
        }

        //
        // Update Day Period
        //
        if (currentDayTime > (6 * SECONDS_PER_HOUR) && currentDayTime < (20 * SECONDS_PER_HOUR))
        {
            dayPeriod = DayPeriod.Day;
            if(eventSunrise != null)
                eventSunrise.Invoke();
        }
        else
        {
            dayPeriod = DayPeriod.Night;
            if (eventSunset != null)
                eventSunset.Invoke();
        }
    }

    public int GetCurrentDayTime()
    {
        return currentDayTime;
    }

    public DayPeriod GetDayPeriod()
    {
        return dayPeriod;
    }
}
