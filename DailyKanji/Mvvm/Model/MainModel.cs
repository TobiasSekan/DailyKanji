using DailyKanjiLogic.Helper;
using System;
using System.Timers;

#nullable enable

namespace DailyKanji.Mvvm.Model
{
    public sealed class MainModel : PropertyChangedHelper
    {
        #region Public Properties

        /// <summary>
        /// Return the version and target framework of this program
        /// </summary>
        public string GetProgramVersion
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

        /// <summary>
        /// Indicate that the correct counter is highlighted
        /// </summary>
        public bool HighlightCorrectCounter
        {
            get => _highlightCorrectCounter;
            set
            {
                if(_highlightCorrectCounter == value)
                {
                    return;
                }

                _highlightCorrectCounter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that the wrong counter is highlighted
        /// </summary>
        public bool HighlightWrongCounter
        {
            get => _highlightWrongCounter;
            set
            {
                if(_highlightWrongCounter == value)
                {
                    return;
                }

                _highlightWrongCounter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that the answer time is highlighted
        /// </summary>
        public bool HighlightAnswerTime
        {
            get => _highlightAnswerTime;
            set
            {
                if(_highlightAnswerTime == value)
                {
                    return;
                }

                _highlightAnswerTime = value;
                OnPropertyChanged();
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

        /// <summary>
        /// Backing-field for <see cref="HighlightCorrectCounter"/>
        /// </summary>
        private bool _highlightCorrectCounter;

        /// <summary>
        /// Backing-field for <see cref="HighlightWrongCounter"/>
        /// </summary>
        private bool _highlightWrongCounter;

        /// <summary>
        /// Backing-field for <see cref="HighlightAnswerTime"/>
        /// </summary>
        private bool _highlightAnswerTime;

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
