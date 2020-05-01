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
        private TimeController timeController = null;

        void Start()
        {
            timeController = FindObjectOfType<TimeController>();
        }

        void Update()
        {
            if (timeController == null)
                return;

            int timeOfDay = timeController.GetCurrentDayTime();
            int hours = timeOfDay / TimeController.SECONDS_PER_HOUR;
            int minutes = (timeOfDay % TimeController.SECONDS_PER_HOUR) / TimeController.SECONDS_PER_MINUTE;
            labelTimeOfDay.text = hours.ToString("D2") + " : " + minutes.ToString("D2");
            labelPeriodOfDay.text = timeController.GetDayPeriod().ToString();
        }
    }
}
