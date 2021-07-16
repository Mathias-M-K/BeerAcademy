using System;
using TMPro;
using UnityEngine;

namespace SupportClasses
{
    public class Timer : MonoBehaviour
    {
        [Header("Time Structure")]
        [SerializeField] private TextMeshPro days;
        [SerializeField] private TextMeshProUGUI hours;
        [SerializeField] private TextMeshProUGUI minutes;
        [SerializeField] private TextMeshProUGUI seconds;
        [SerializeField] private TextMeshProUGUI milliseconds;

        private bool _isDaysNotNull;
        private bool _isHoursNotNull;
        private bool _isMinutesNotNull;
        private bool _isSecondsNotNull;
        private bool _isMillisecondsNotNull;

        private void Start()
        {
            _isMillisecondsNotNull = milliseconds != null;
            _isSecondsNotNull = seconds != null;
            _isMinutesNotNull = minutes != null;
            _isHoursNotNull = hours != null;
            _isDaysNotNull = days != null;
        }

        /// <summary>
        /// Converts the timespan to the provided time text-fields
        /// </summary>
        /// <param name="time"></param>
        public void SetTimer(TimeSpan time)
        {
            if (_isDaysNotNull) days.text = $"{time.Days:000}";
            if (_isHoursNotNull) hours.text = $"{time.Hours:00}";
            if (_isMinutesNotNull) minutes.text = $"{time.Minutes:00}";
            if (_isSecondsNotNull) seconds.text = $"{time.Seconds:00}";
            if (_isMillisecondsNotNull) milliseconds.text = $"{time.Milliseconds:000}";
        }
        
    }
}