﻿using DailyKanjiLogic.Helper;
using System;
using System.Timers;

namespace DailyKanji.Mvvm.Model
{
    public sealed class MainModel : PropertyChangedHelper
    {
        #region Public Properties

        /// <summary>
        /// Return the version and target framework of this program
        /// </summary>
        public string GetVersion
            => $"{AssemblyHelper.GetAssemblyVersion(this)} ({AssemblyHelper.GetTargetFramework(this)})";

        /// <summary>
        /// The refresh interval for the visual progress bar (Note: The progress bar show the running timer)
        /// </summary>
        public TimeSpan ProgressPrefreshInterval
        {
            get => _progressPrefreshInterval;
            set
            {
                if(_progressPrefreshInterval == value)
                {
                    return;
                }

                _progressPrefreshInterval = value;
                OnPropertyChanged();

                if(TestTimer is null)
                {
                    return;
                }

                TestTimer.Interval = _progressPrefreshInterval.Milliseconds;
            }
        }

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// Running timer for each test
        /// </summary>
        internal Timer TestTimer { get; set; }

        #endregion Internal Properties

        #region Private Backing-fields

        /// <summary>
        /// Backing-field for <see cref="ProgressPrefreshInterval"/>
        /// </summary>
        private TimeSpan _progressPrefreshInterval;

        #endregion Private Backing-fields

        #region Internal Constructors

        /// <summary>
        /// Create a new <see cref="MainModel"/> with default values
        /// </summary>
        internal MainModel()
        {
            ProgressPrefreshInterval = new TimeSpan(0, 0, 0, 0, 15);
            TestTimer                = new Timer(ProgressPrefreshInterval.TotalMilliseconds);
        }

        #endregion Internal Constructors
    }
}
