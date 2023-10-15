using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using System.Timers;

using static MiniTimeLogger.Support.ExceptionHandling;

namespace MiniTimeLogger.Data
{
    public class TimeLogEntry : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public static Type ThisStaticType = typeof(TimeLogEntry);
        
        private DateTime _startTime = DateTime.MinValue;
        private DateTime _endTime = DateTime.MaxValue;
        private Timer _logTimer = null;
        private TimeSpan _elapsedTime;
        
        public CategoryItem Parent { get; private set; }
        public DateTime StartTime => _startTime;
        public DateTime EndTime => _endTime;
        public bool IsFinished => _endTime < DateTime.MaxValue;
        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            private set
            {
                if (_elapsedTime != value)
                {
                    _elapsedTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeLogEntry()
        {

        }

        public static TimeLogEntry CreateTimeLogEntry(CategoryItem parent, bool startNow = false)
        {
            LogDebug($"{ThisStaticType}::{GetCaller()}");
            
            TimeLogEntry entry = new TimeLogEntry()
            {
                Parent = parent
            };

            if (startNow)
                entry.StartLogging();

            return entry;
        }

        public static void LoadTimeLogEntry(CategoryItem parent, XElement element)
        {

        }

        public void StartLogging()
        {
            LogDebug($"{ThisStaticType}::{GetCaller()}");

            if (_logTimer == null)
            {
                _startTime = DateTime.Now;
                _logTimer = new Timer()
                {
                    AutoReset = true,
                    Interval = 1000,
                };
                _logTimer.Elapsed += RefreshElapsedTime;
                _logTimer.Start();
            }
            else if (_endTime < DateTime.MaxValue)
                MsgBox(MessageSeverityTypes.MS_ERROR, "Timer already finished.");
            else
                MsgBox(MessageSeverityTypes.MS_WARNING, "Timer already running.");
        }

        public void EndLogging()
        {
            LogDebug($"{ThisStaticType}::{GetCaller()}");

            if (_logTimer == null)
                MsgBox(MessageSeverityTypes.MS_ERROR, "Timer not started.");
            else if (_endTime < DateTime.MaxValue)
                MsgBox(MessageSeverityTypes.MS_WARNING, "Timer already finished.");
            else
            {
                _logTimer.Stop();
                _endTime = DateTime.Now;
            }
        }

        private void RefreshElapsedTime(object sender, ElapsedEventArgs e)
        {
            ElapsedTime = e.SignalTime - _startTime;
        }
    }
}
