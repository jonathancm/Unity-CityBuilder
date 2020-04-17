using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public enum DayPeriod
    {
        Night,
        Day
    }

    //
    // Constants
    //
    public static int SecsPerHour = 3600;
    public static int SecsPerDay = 24 * 3600;
    public static int MidNight = 0;
    public static int MidDay = 12 * 3600;

    //
    // Configurable Parameters
    //
    public float gameSecondsPerLifeSeconds = 60;
    [Range(0.0f, 3.0f)] public float timeDilation = 1.0f;

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
        if (currentDayTime > SecsPerDay)
        {
            daysSinceStart++;
            currentDayTime = 0;
        }

        //
        // Update Day Period
        //
        if (currentDayTime > (6 * SecsPerHour) && currentDayTime < (20 * SecsPerHour))
            dayPeriod = DayPeriod.Day;
        else
            dayPeriod = DayPeriod.Night;
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
