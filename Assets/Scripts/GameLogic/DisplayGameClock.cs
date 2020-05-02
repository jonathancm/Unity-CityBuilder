using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CityBuilder
{
    public class DisplayGameClock : MonoBehaviour
    {
        //
        // Configurable Parameters
        //
        public TextMeshProUGUI labelTimeOfDay = null;
        public TextMeshProUGUI labelPeriodOfDay = null;

        //
        // Cached References
        //
        private TimeKeeperSystem timeController = null;

        void Start()
        {
            timeController = FindObjectOfType<TimeKeeperSystem>();
        }

        void Update()
        {
            if (timeController == null)
                return;

            int timeOfDay = timeController.GetCurrentDayTime();
            int hours = timeOfDay / TimeKeeperSystem.SECONDS_PER_HOUR;
            int minutes = (timeOfDay % TimeKeeperSystem.SECONDS_PER_HOUR) / TimeKeeperSystem.SECONDS_PER_MINUTE;
            labelTimeOfDay.text = hours.ToString("D2") + " : " + minutes.ToString("D2");
            labelPeriodOfDay.text = timeController.GetDayPeriod().ToString();
        }
    }
}
